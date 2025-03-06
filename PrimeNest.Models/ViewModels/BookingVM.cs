using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrimeNest.Models.ViewModels
{
    public class BookingVM
    {
        public BookingAppointment Booking { get; set; }
       public IEnumerable<Property> Properties { get; set; }
        public IEnumerable<ApplicationUser> applicationUsers { get; set; }

        
    }
}
