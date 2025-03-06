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
    public class StateRepo:Repository<State>,IStateRepo
    {
        private ApplicationDbContext _context;
        public StateRepo(ApplicationDbContext context):base(context)
        {
            _context = context;
        }
    }
}
