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
    public class PropertyTypeRepo:Repository<PropertyType>,IPropertyTypeRepo
    {
        private readonly ApplicationDbContext _context;
        public PropertyTypeRepo(ApplicationDbContext context):base(context) 
        {
            _context = context;
        }
    }
}
