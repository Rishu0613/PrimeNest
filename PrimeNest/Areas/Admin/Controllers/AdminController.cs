using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrimeNest.DataAccess.Data;
using PrimeNest.DataAccess.Repository.IRepository;
using PrimeNest.Models;
using PrimeNest.Utility;
using System.Security.Claims;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace PrimeNest.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IUnitOfWork _unitOfWork;
        public AdminController(ApplicationDbContext context,IUnitOfWork unitOfWork)
        {
            _context = context;
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
           
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.IsInRole("Admin") ? "Admin" : "Agent"; 

            IEnumerable<Property> properties;

            if (userRole == "Admin")
            {
                // Admin gets all properties
                properties = _unitOfWork.PropertyRepo.GetAll(includeProperties: "City,User,PropertyType,PropertyImages,PropertyVideos");
            }
            else
            {
                // Agent gets only their own properties
                properties = _unitOfWork.PropertyRepo.GetAll(
                    p => p.UserId == userId,
                    includeProperties: "City,User,PropertyType,PropertyImages,PropertyVideos"
                );
            }

            return View(properties);
        }


        public IActionResult Agents()
        {
            return View();
        }
        public IActionResult Client()
        {
            return View();
        }

        #region Apis
        public IActionResult GetAll()
        {
            var individuals = _context.ApplicationUsers.ToList();
            var agents = _context.ApplicationUsers.ToList();
            var roles = _context.Roles.ToList();
            var userRoles = _context.UserRoles.ToList();

            foreach (var user in individuals)
            {
                var roleId = userRoles.FirstOrDefault(r => r.UserId == user.Id)?.RoleId;
                user.Role = roles.FirstOrDefault(r => r.Id == roleId)?.Name;
            }

            foreach (var user in agents)
            {
                var roleId = userRoles.FirstOrDefault(r => r.UserId == user.Id)?.RoleId;
                user.Role = roles.FirstOrDefault(r => r.Id == roleId)?.Name;
            }

            var filteredIndividuals = individuals.Where(u => u.Role != SD.Role_Admin && u.Role != SD.Role_Agent).ToList().ToList();
            var filteredAgents = agents.Where(u => u.Role != SD.Role_Admin && u.Role != SD.Role_Individual).ToList().ToList();

            return Json(new { individuals = filteredIndividuals, agents = filteredAgents });
        }

        [HttpPost]
        public IActionResult lockUnlock([FromBody] string id)
        {
            bool isLocked = false;
            var userInDb = _context.ApplicationUsers.FirstOrDefault(u => u.Id == id);
            if (userInDb == null)
            {
                return Json(new { success = false, message = "Something went wrong while lock or unlock the user" });
            }
            if (userInDb != null && userInDb.LockoutEnd > DateTime.Now)
            {
                userInDb.LockoutEnd = DateTime.Now;
                isLocked = false;
            }
            else
            {
                userInDb.LockoutEnd = DateTime.Now.AddYears(100);
                isLocked = true;
            }
            _context.SaveChanges();
            return Json(new { success = true, message = isLocked == true ? "User successfully locked" : "User successfully unlocked" });
        }
        public IActionResult GetallState()
        {
            var states = _unitOfWork.StateRepo.GetAll().ToList();

            return Json(new { data = states});
        }
        public IActionResult GetAllCity()
        {
            var city = _unitOfWork.CityRepo.GetAll(includeProperties: "State").ToList();
            return Json(new { data = city });
        }

        public IActionResult GetReview()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            // Get all properties posted by the agent
            var agentProperties = _unitOfWork.PropertyRepo
                                    .GetAll(p => p.UserId == userId)
                                    .Select(p => p.Id)
                                    .ToList();

            if (!agentProperties.Any())
            {
                return NotFound(new { message = "No properties found for this agent." });
            }

            // Get reviews related to the agent's properties
            var reviews = _unitOfWork.ReviewRepo
                .GetAll(r => agentProperties.Contains(r.propertyId), includeProperties: "User,Property")
                .Select(r => new
                {
                    Id = r.Id,
                    Comment = r.Comment ?? "No comment", // Avoid null reference
                    Rating = r.Stars,
                    UserName = r.User != null ? r.User.Name : "Unknown User", // Handle null User
                    PropertyTitle = r.Property != null ? r.Property.Title : "Unknown Property" // Handle null Property
                })
                .ToList();

            // If no reviews exist, return a proper empty response
            if (!reviews.Any())
            {
                return Json(new { data = new List<object>(), message = "No reviews found for any property." });
            }

            return Json(new { data = reviews });
        }



        #endregion
        public IActionResult State(int id)
        {
            State state = new State();
            if (id == 0) return View(state);
            state = _unitOfWork.StateRepo.Get(id);
            return View(state);
        }
        [HttpPost]
        public IActionResult State(State state)
        {
            if (state == null) return BadRequest();
            if (!ModelState.IsValid) return View(state);
            if (state.Id == 0)
                _unitOfWork.StateRepo.Add(state);
            else
                _unitOfWork.StateRepo.Update(state);
            _unitOfWork.Save();
            return RedirectToAction("State");
        }
        public IActionResult City(int id)
        {
            City city = new City();
        
            ViewBag.State = _unitOfWork.StateRepo.GetAll().ToList();
            if (id == 0)
            {
                return View(city);
            }
            city = _unitOfWork.CityRepo.Get(id);
            return View(city);
        }
        [HttpPost]
        public IActionResult City(City city) {
            if (city == null) return BadRequest();
            if (ModelState.IsValid) return View(city);
            if (city.Id == 0)
                _unitOfWork.CityRepo.Add(city);
            else
                _unitOfWork.CityRepo.Update(city);
            _unitOfWork.Save();
            return RedirectToAction("City");
        }
        
        public IActionResult Review()
        {
            return View();
        }
        public IActionResult AllBooking()
        {
            return View();
        }
    }
}
