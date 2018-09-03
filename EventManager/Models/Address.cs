using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EventManager.Models
{
    public class Address
    {
        [Required]
        [Display(Name = "Address Line 1")]
        public string AddressLine1 { get; set; }
        [Display(Name = "Address Line 2")]
        public string AddressLine2 { get; set; }
        [Required]
        public string Town { get; set; }
        [Required]
        public string Country { get; set; }
        [Required]
        public string Postcode { get; set; }
    }
}