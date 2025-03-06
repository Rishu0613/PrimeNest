using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrimeNest.Models
{
    public class Review
    {
        public int Id { get; set; }

        [Required]
        public int propertyId { get; set; }
        public Property Property { get; set; } 

        [Required]
        public string userId { get; set; }
        public ApplicationUser User { get; set; } 

        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5 stars.")]
        public int Stars { get; set; } 

        [Required]
        [MaxLength(1000)]
        public string Comment { get; set; }

        public DateTime DateCreated { get; set; } = DateTime.Now;
    }
}
