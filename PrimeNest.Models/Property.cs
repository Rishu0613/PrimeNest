using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrimeNest.Models
{
    public class Property
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        [Display(Name = "UserId")]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        [Display(Name = "PropertyType")]
        public int PropertyTypeId { get; set; }
        public PropertyType PropertyType { get; set; }
        [Required]
        [Display(Name = "City")]
        public int CityId { get; set; }
        public City City { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public int Price { get; set; }

        public enum PropertyFor {Rent,sales}
        public PropertyFor propertyFor { get; set; }

        public enum Status { Available,Selled}
        public Status status { get; set; }
        public DateTime DateCreated { get; set; }
        // Navigation property for multiple images
        public ICollection<PropertyImage> PropertyImages { get; set; } = new List<PropertyImage>();
        // Multiple videos
        public ICollection<PropertyVideo> PropertyVideos { get; set; } = new List<PropertyVideo>();
        // Reviews
        public ICollection<Review> Reviews { get; set; } = new List<Review>();


    }
}
