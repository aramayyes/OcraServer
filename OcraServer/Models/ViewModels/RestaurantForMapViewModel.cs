using System.ComponentModel.DataAnnotations;
using OcraServer.Enums;

namespace OcraServer.Models.ViewModels
{
    public class RestaurantForMapViewModel
    {
		[Required]
		public int ID { get; set; }

		[Required]
		[StringLength(100)]
		public string Name { get; set; }

		[StringLength(445)]
		public string LogoLink { get; set; }

		[Required]
		[StringLength(100)]
		public string Address { get; set; }

		[Required]
		public double Latitude { get; set; }

		[Required]
		public double Longitude { get; set; }

		[Required]
		public RestaurantCategory Category { get; set; }

        [Required]
		public double Rating { get; set; }
    }
}
