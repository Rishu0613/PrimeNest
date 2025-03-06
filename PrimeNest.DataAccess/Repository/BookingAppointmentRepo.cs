using PrimeNest.DataAccess.Data;
using PrimeNest.DataAccess.Repository.IRepository;
using PrimeNest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrimeNest.DataAccess.Repository
{
    public class BookingAppointmentRepo:Repository<BookingAppointment>,IBookingAppointmentRepo
    {
        private readonly ApplicationDbContext _context;
        public BookingAppointmentRepo(ApplicationDbContext context):base(context) 
        {
            _context = context;
        }
    }
}
