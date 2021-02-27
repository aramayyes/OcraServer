using System;
using System.ComponentModel.DataAnnotations;

namespace OcraServer.Models.ViewModels
{
    public class EventViewModel
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
     
        [Required]
        public DateTime EventDateTime { get; set; }

        public int? AdditionalPrice { get; set; }

        [Required]
        public string RestoName { get; set; }
        public string RestoLogoLink { get; set; }
    }
}
