using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrimeNest.Models
{
    public class State
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
