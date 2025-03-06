using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using PrimeNest.DataAccess.Repository.IRepository;
using PrimeNest.Models;
using PrimeNest.Models.ViewModels;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PrimeNest.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class BookingController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailSender _emailSender;

        public BookingController(IUnitOfWork unitOfWork, IEmailSender emailSender)
        {
            _unitOfWork = unitOfWork;
            _emailSender = emailSender;
        }

        public IActionResult Index()
        {
           return View();
        }
        #region Api's
        [HttpGet]
        public IActionResult GetAll()
        {
            // Get the logged-in user's email and UserId
            var userEmail = User.Identity.Name;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; // Assuming UserId is stored in claims
  
            var userRole = User.IsInRole("Agent");
            if (string.IsNullOrEmpty(userEmail) && string.IsNullOrEmpty(userId))
            {
                return Json(new { data = new List<BookingAppointment>() }); // Return empty if no user is logged in
            }



            if (userRole)
            {
                // Fetch all bookings for properties added by the agent, using userId to find the agent's properties
                var agentBookings = _unitOfWork.Booking.GetAll(
                    b => b.Property.UserId == userId, // Assuming Property has an AgentUserId field linking to the agent
                    includeProperties: "Property"
                ).Select(b => new
                {
                    b.CustomerName,
                    BookingDate = b.BookingDate.ToString("yyyy-MM-dd"), // Format Date
                    BookingTime = DateTime.Today.Add(b.BookingTime).ToString("hh:mm tt"), // Convert TimeSpan to 12-hour format
                    PropertyName = b.PropertyId != null ? b.Property.Title : "N/A"
                }).ToList();

                return Json(new { data = agentBookings });
            }
            else
            {
                // If the user is not an agent, fetch bookings using the user's email
                var userBookings = _unitOfWork.Booking.GetAll(
                    b => b.Email == userEmail, // Use the Email to filter bookings for individual user
                    includeProperties: "Property"
                ).Select(b => new
                {
                    b.CustomerName,
                    BookingDate = b.BookingDate.ToString("yyyy-MM-dd"), // Format Date
                    BookingTime = DateTime.Today.Add(b.BookingTime).ToString("hh:mm tt"), // Convert TimeSpan to 12-hour format
                    PropertyName = b.PropertyId != null ? b.Property.Title : "N/A"
                }).ToList();

                return Json(new { data = userBookings });
            }
        }




        #endregion

        public IActionResult Create(int? id)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);

            if (claims == null)
            {
                return Unauthorized();
            }

            string userId = claims.Value;

            // Fetch logged-in user details
            var user = _unitOfWork.UserRepo.GetAll()
                        .Where(u => u.Id == userId)
                        .Select(u => new ApplicationUser
                        {
                            Id = u.Id,
                            Name = u.Name,
                            Email = u.Email
                        })
                        .FirstOrDefault();

            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Fetch properties
            var properties = _unitOfWork.PropertyRepo.GetAll().ToList();

            // Fetch property details (if ID is provided)
            Property selectedProperty = id.HasValue
                ? properties.FirstOrDefault(p => p.Id == id.Value)
                : null;

            if (id.HasValue && selectedProperty == null)
            {
                return NotFound("Property not found.");
            }

            // Initialize BookingAppointment
            var bookingAppointment = new BookingAppointment
            {
                CustomerName = user.Name,
                Email = user.Email,
                PropertyId = selectedProperty?.Id ?? 0,
                BookingDate = DateTime.Now.Date
            };

            // Prepare ViewModel
            var bookingVM = new BookingVM
            {
                Booking = bookingAppointment,
                Properties = properties,
                applicationUsers = new List<ApplicationUser> { user } // Only the logged-in user
            };

            return View(bookingVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookingVM model, int PropertyId)
        {
            if (ModelState.IsValid)
            {
                // Return JSON with errors for client-side handling
                var errors = ModelState.Values
                    .SelectMany(x => x.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return Json(new { success = false, errors = errors });
            }

            // Get user information from Claims
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);

            if (claims == null)
            {
                return Unauthorized(); // Unauthorized if no user is found
            }

            string userId = claims.Value;

            // Fetch logged-in user details
            var user = _unitOfWork.UserRepo.GetAll()
                .Where(u => u.Id == userId)
                .Select(u => new ApplicationUser
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email
                })
                .FirstOrDefault();

            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Create new booking
            var booking = new BookingAppointment
            {
                PropertyId = PropertyId,
                CustomerName = model.Booking.CustomerName ?? user.Name,
                Email = user.Email,
                PhoneNumber = model.Booking.PhoneNumber,
                BookingDate = model.Booking.BookingDate,
                BookingTime = model.Booking.BookingTime
            };

            _unitOfWork.Booking.Add(booking);
            _unitOfWork.Save();

            // Fetch the property details safely
            var property = _unitOfWork.PropertyRepo.FirstOrDefault(p => p.Id == PropertyId);
            if (property == null)
            {
                return NotFound("Property not found.");
            }

            // Fetch the agent safely
            var agent = _unitOfWork.UserRepo.FirstOrDefault(u => u.Id == property.UserId);
            if (agent != null && !string.IsNullOrEmpty(agent.Email))
            {
                // Prepare the email content
                var subject = "New Appointment Booked";
                var htmlMessage = $@"
                    <h3>You have a new appointment!</h3>
                    <p><strong>Booked by:</strong> {booking.CustomerName}</p>
                    <p><strong>Appointment Date & Time:</strong> {booking.BookingDate:yyyy-MM-dd} {booking.BookingTime}</p>
                    <p><strong>Message:</strong> I want to get details</p>
                    <p>Please log in to your account to view more details.</p>";

                // Send the email to the agent
                await _emailSender.SendEmailAsync(agent.Email, subject, htmlMessage);
            }

            return Json(new { success = true });
        }
    }
}
