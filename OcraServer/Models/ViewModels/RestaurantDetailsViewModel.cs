using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using OcraServer.Enums;
using OcraServer.Models.EntityFrameworkModels;

namespace OcraServer.Models.ViewModels
{
    public class RestaurantDetailsViewModel
    {
        [Required]
		public int ID { get; set; }

		[Required]
		[StringLength(100)]
		public string Name { get; set; }

		[Required]
		[StringLength(2000)]
		public string Description { get; set; }

		[StringLength(445)]
		public string LogoLink { get; set; }

		[Required]
		[StringLength(445)]
		public string MainImgLink { get; set; }

		[Required]
		public RestaurantCategory Category { get; set; }

		[Required]
		[StringLength(100)]
		public string Cuisine { get; set; }

		[Required]
		[StringLength(100)]
		public string Cost { get; set; }

		[Required]
		[StringLength(100)]
		public string City { get; set; }

		[Required]
		[StringLength(100)]
		public string Address { get; set; }

		[Required]
		[StringLength(20)]
		public string PhoneNumber { get; set; }

		[StringLength(20)]
		public string AdditionalPhoneNumber { get; set; }

		[Required]
		public double Latitude { get; set; }

		[Required]
		public double Longitude { get; set; }

		[StringLength(50)]
		public string WebSite { get; set; }

		[StringLength(20)]
		public string Email { get; set; }

		[StringLength(80)]
		public string Facebook { get; set; }

        [Required]
		public int RatedCount { get; set; }
		[Required]
        public double Rating { get; set; }

        public string ServePrice { get; set; }

		[Required]
		public bool IsOpen24 { get; set; } = false;

		public TimeSpan? OpeningTime { get; set; }

		public TimeSpan? ClosingTime { get; set; }

		public TimeSpan? AdditionalOpeningTime { get; set; }

		public TimeSpan? AdditionalClosingTime { get; set; }

		[Required]
		public bool IsMain { get; set; } = true;

		[Required]
		[StringLength(100)]
		public string Brand { get; set; }

        [Required]
		public virtual RestaurantFeatures Features { get; set; }

        public virtual IEnumerable<RestaurantImg> Images { get; set; } = new HashSet<RestaurantImg>();

		public bool? IsInFavorites { get; set; }

        public IEnumerable<RestaurantViewModel> SubRestaurants { get; set; } = new List<RestaurantViewModel>();
    }
}
