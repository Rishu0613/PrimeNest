using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PrimeNest.Models;

namespace PrimeNest.DataAccess.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<State> States { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<PropertyType> PropertiesType { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Property> Properties { get; set; }
        public DbSet<PropertyVideo> propertyVideos { get; set; }
        public DbSet<PropertyImage> propertyImages { get; set; }
       
        public DbSet<BookingAppointment>BookingAppointments { get; set; }
    }
}
