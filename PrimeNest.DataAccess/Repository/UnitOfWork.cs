using PrimeNest.DataAccess.Data;
using PrimeNest.DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrimeNest.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            UserRepo=new ApplicationUserRepo(_context);
            PropertyRepo=new PropertyRepo(_context);
            TypeRepo=new PropertyTypeRepo(_context);
            CityRepo=new CityRepo(_context);
            StateRepo = new StateRepo(_context);
            PropertyImageRepo=new PropertyImageRepo(_context);
            PropertyVideoRepo = new PropertyVideoRepo(_context);
            ReviewRepo = new ReviewRepo(_context);
            Booking=new BookingAppointmentRepo(_context);
        }
        public IApplicationUserRepo UserRepo { private set; get; }

        public IPropertyRepo PropertyRepo {  private set; get; }

        public IPropertyTypeRepo TypeRepo { private set; get; }

        public ICityRepo CityRepo { private set; get; }

        public IStateRepo StateRepo { private set; get; }

        public IPropertyImageRepo PropertyImageRepo { private set; get; }

        public IPropertyVideoRepo PropertyVideoRepo { private set; get; }

        public IReviewRepo ReviewRepo { private set; get; }

        public IBookingAppointmentRepo Booking { private set; get; }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
