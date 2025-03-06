using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrimeNest.Models.ViewModels
{
    public class PropertyVM
    {
        public Property Property { get; set; }
        public IEnumerable<SelectListItem> PropertyType { get; set; }
        public IEnumerable<SelectListItem> City { get; set; }

        // Change to collections to store multiple images and videos
        public IEnumerable<PropertyImage> PropertyImages { get; set; }
        public IEnumerable<PropertyVideo> PropertyVideos { get; set; }

        // Ensure this is defined as shown:
        public Review Review { get; set; }  // A single review (probably used for submitting a new review)
        public IEnumerable<Review> Reviews { get; set; }  // A collection of reviews

    }
}
