using System;
using System.ComponentModel.DataAnnotations;

#pragma warning disable 1591

namespace OcraServer.Models.ViewModels
{
    public class DiscountViewModel
    {
        [Required]
        public int ID { get; set; }

        [Required]
        public int RestaurantID { get; set; }
       
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(2000)]
        public string Description { get; set; }

        [Required]
        public string ImgLink { get; set; }

        public int? DiscountSize { get; set; }
        public int? NewPrice { get; set; }

        public DateTime? Deadline { get; set; }

        [Required]
        public string RestoName { get; set; }

        public string RestoLogoLink { get; set; }
    }
}
