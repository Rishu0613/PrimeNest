using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using PrimeNest.DataAccess.Repository.IRepository;
using PrimeNest.Models;
using PrimeNest.Models.ViewModels;
using PrimeNest.Utility;
using System.Security.Claims;

namespace PrimeNest.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PropertyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public PropertyController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            return View();
        }

        #region APIs
        public IActionResult Getall()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            IEnumerable<Property> properties;

            if (User.IsInRole(SD.Role_Admin))
            {
                properties = _unitOfWork.PropertyRepo.GetAll(includeProperties: "City,User");
            }
            else
            {
                properties = _unitOfWork.PropertyRepo.GetAll(p => p.UserId == claims.Value, includeProperties: "City,User");
            }

            return Json(new { data = properties });
        }
        #endregion
        public IActionResult Upsert(int? id)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            PropertyVM propertyVM = new PropertyVM()
            {
                Property = new Property()
                {
                    UserId = claims.Value
                },
                PropertyType = _unitOfWork.TypeRepo.GetAll().Select(pt => new SelectListItem()
                {
                    Text = pt.Name,
                    Value = pt.Id.ToString()
                }),
                City = _unitOfWork.CityRepo.GetAll().Select(c => new SelectListItem()
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                })
            };

            if (id != null)
            {
                // Get property data for editing
                propertyVM.Property = _unitOfWork.PropertyRepo.Get(id.GetValueOrDefault());

                // Get the associated images and videos for this property
                propertyVM.PropertyImages = _unitOfWork.PropertyImageRepo.GetAll()
                    .Where(pi => pi.PropertyId == id).ToList();

                propertyVM.PropertyVideos = _unitOfWork.PropertyVideoRepo.GetAll()
                    .Where(pv => pv.PropertyId == id).ToList();
            }

            return View(propertyVM); // Return to the view
        }




        [HttpPost]
        public IActionResult Upsert(PropertyVM propertyVM)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (propertyVM.Property.Id == 0)
            {
                if (claims != null)
                {
                    propertyVM.Property.UserId = claims.Value;
                }
            }
           

            if (!ModelState.IsValid)  
            {
                var webRootPath = _webHostEnvironment.WebRootPath;
                var files = HttpContext.Request.Form.Files;

                // Check if it's a new property or an update
                if (propertyVM.Property.Id == 0)
                {
                    _unitOfWork.PropertyRepo.Add(propertyVM.Property);
                    _unitOfWork.Save();  // Save here to generate Property ID
                }
                else
                {
                    var existingProperty = _unitOfWork.PropertyRepo.Get(propertyVM.Property.Id);
                    if (existingProperty == null)
                        return NotFound();

                    existingProperty.Title = propertyVM.Property.Title;
                    existingProperty.Description = propertyVM.Property.Description;
                    existingProperty.Price = propertyVM.Property.Price;
                    existingProperty.Address = propertyVM.Property.Address;
                    existingProperty.PropertyTypeId = propertyVM.Property.PropertyTypeId;
                    existingProperty.CityId = propertyVM.Property.CityId;
                    existingProperty.propertyFor = propertyVM.Property.propertyFor;
                    existingProperty.status = propertyVM.Property.status;

                    _unitOfWork.PropertyRepo.Update(existingProperty);
                }

                // ✅ Handling multiple image uploads
                if (files.Count > 0)
                {
                    var uploads = Path.Combine(webRootPath, @"images\Property");

                    foreach (var file in files)
                    {
                        if (file.ContentType.Contains("image")) // ✅ Only process image files
                        {
                            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                            var filePath = Path.Combine(uploads, fileName);

                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                file.CopyTo(fileStream);
                            }

                            var newImage = new PropertyImage()
                            {
                                PropertyId = propertyVM.Property.Id,
                                ImageUrl = @"\images\Property\" + fileName
                            };

                            _unitOfWork.PropertyImageRepo.Add(newImage);
                        }
                    }
                }

                // ✅ Handling multiple video uploads
                foreach (var file in files)
                {
                    if (file.ContentType.Contains("video")) // ✅ Only process video files
                    {
                        var uploads = Path.Combine(webRootPath, "videos");
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        var filePath = Path.Combine(uploads, fileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            file.CopyTo(fileStream);
                        }

                        var newVideo = new PropertyVideo
                        {
                            PropertyId = propertyVM.Property.Id,
                            VideoUrl = "/videos/" + fileName
                        };

                        _unitOfWork.PropertyVideoRepo.Add(newVideo);
                    }
                }

                _unitOfWork.Save(); // ✅ Save changes after processing images & videos

                return RedirectToAction(nameof(Index));
            }

            // If ModelState is invalid, reload dropdown lists
            propertyVM.PropertyType = _unitOfWork.TypeRepo.GetAll().Select(pt => new SelectListItem()
            {
                Text = pt.Name,
                Value = pt.Id.ToString()
            });

            propertyVM.City = _unitOfWork.CityRepo.GetAll().Select(c => new SelectListItem()
            {
                Text = c.Name,
                Value = c.Id.ToString()
            });

            return View(propertyVM);
        }


    }
}
