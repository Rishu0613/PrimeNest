using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrimeNest.Models
{
    public class PropertyVideo
    {
        public int Id { get; set; }
        public int PropertyId { get; set; }
        public string VideoUrl { get; set; }  // Stores the file path

        [ForeignKey("PropertyId")]
        public Property Property { get; set; }
    }
}
