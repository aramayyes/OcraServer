using System.ComponentModel.DataAnnotations;

namespace OcraServer.Models.ViewModels
{
    public class RestaurantSearchResultViewModel
    {
        [Required]
        public int ID { get; set; }

        [Required]
        public string Name { get; set; }
   
        [Required]
        public string MainImgLink { get; set; }

        [Required]
        public string Address { get; set; }
    
        [Required]
        public double Rating { get; set; }
    }
}
