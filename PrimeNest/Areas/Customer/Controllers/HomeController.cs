 using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using PrimeNest.DataAccess.Repository.IRepository;
using PrimeNest.Models;
using PrimeNest.Models.ViewModels;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace PrimeNest.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        public HomeController(ILogger<HomeController> logger,IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public IActionResult Index(string? query)
        {
            IEnumerable<Property> filteredProperty = new List<Property>();

            if (!string.IsNullOrWhiteSpace(query))
            {
                filteredProperty = _unitOfWork.PropertyRepo.GetAll(
                filter: property => property.City.Name.Contains(query) ||
                                        property.City.ZipCode.ToString().Contains(query)|| property.Title.Contains(query),
                    includeProperties: "City,User,PropertyType,PropertyImages,PropertyVideos"
                );
                return View(filteredProperty);

            }
            else
            {
                var properties = _unitOfWork.PropertyRepo.GetAll(includeProperties: "City,User,PropertyType,PropertyImages,PropertyVideos");
                return View(properties);
            }
        }
        
        [Authorize] // Ensure only logged-in users can access
        public IActionResult DeleteReview(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get logged-in user's ID

            if (id == 0 || string.IsNullOrEmpty(userId))
            {
                return NotFound();
            }

            var review = _unitOfWork.ReviewRepo.FirstOrDefault(r => r.Id == id);
            int propertyId = review.propertyId; // Assuming the review has a PropertyId field


            if (review == null)
            {
                return NotFound(); // If no review found with the given ID
            }

            if (review.userId != userId)
            {
                return RedirectToAction("Detail", "Home", new { id = propertyId }); // Redirect with Property ID

            }


            _unitOfWork.ReviewRepo.Remove(review);
            _unitOfWork.Save();

            return RedirectToAction("Detail", "Home", new { id = propertyId }); // Redirect with Property ID
        }


        public IActionResult Detail(int? id)
        {
            // Check if the id is null
            if (id == null)
            {
                return BadRequest("Property ID is required.");
            }

            // Retrieve property with related entities
            var prop = _unitOfWork.PropertyRepo.FirstOrDefault(
                p => p.Id == id,
                includeProperties: "City,User,PropertyType,PropertyImages,PropertyVideos,Reviews.User"  // Include Reviews.User
            );

            // If the property is not found, return a 404
            if (prop == null)
            {
                return NotFound();
            }

            // Ensure the agent information (User) is available for the view
            if (prop.User == null)
            {
                return NotFound("Agent not found.");
            }

            // Create a new PropertyVM instance and initialize Reviews collection
            var propertyVM = new PropertyVM
            {
                Property = prop,
                PropertyImages = prop.PropertyImages,  // Assuming PropertyImages is a navigation property
                PropertyVideos = prop.PropertyVideos,  // Assuming PropertyVideos is a navigation property
                Reviews = prop.Reviews ?? new List<Review>(),  // Ensure the Reviews collection is not null
                Review = new Review()  // Initialize a new Review object if necessary
            };

            // Pass the PropertyVM object to the view
            return View(propertyVM);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitReview(Review reviewModel)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);

            if (claims == null)
            {
                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }

            // Fetch the property associated with the review
            var product = _unitOfWork.PropertyRepo.FirstOrDefault(p => p.Id == reviewModel.propertyId);

            if (product == null)
            {
                return NotFound(); // Return 404 if the property does not exist
            }

            // Assign correct values to the review model
            reviewModel.userId = claims.Value;
            reviewModel.propertyId = product.Id;

            // Save the review to the database
            _unitOfWork.ReviewRepo.Add(reviewModel);
             _unitOfWork.Save(); // Ensure changes are persisted

            return RedirectToAction("Detail", "Home", new { id = product.Id }); // Redirect to property details page
        }


        public IActionResult ViewAllProperties()
        {
            var properties = _unitOfWork.PropertyRepo.GetAll(
                includeProperties: "City,User,PropertyType,PropertyImages,PropertyVideos"
            );

            return View(properties);
        }


        public IActionResult Services()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
