using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrimeNest.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        IApplicationUserRepo UserRepo { get; }
        IPropertyRepo PropertyRepo { get; }
        IPropertyTypeRepo TypeRepo { get; }
        ICityRepo CityRepo { get; }
        IStateRepo StateRepo { get; }
        IPropertyImageRepo PropertyImageRepo { get; }
        IPropertyVideoRepo PropertyVideoRepo { get; }
        IReviewRepo ReviewRepo { get; }
        IBookingAppointmentRepo Booking {  get; }

        void Save();
    }
}
