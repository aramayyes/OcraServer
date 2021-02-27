using System;
using System.ComponentModel.DataAnnotations;

namespace OcraServer.Models.ViewModels
{
    public class RestaurantViewModel
    {
        [Required]
        public int ID { get; set; }

        [Required]
		[StringLength(100)]
        public string Name { get; set; }

		[StringLength(445)]
        public string LogoLink { get; set; }

        [Required]
		[StringLength(445)]
        public string MainImgLink { get; set; }

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
        public bool IsOpen24 { get; set; }

		public TimeSpan? OpeningTime { get; set; }

		public TimeSpan? ClosingTime { get; set; }

		public TimeSpan? AdditionalOpeningTime { get; set; }

		public TimeSpan? AdditionalClosingTime { get; set; }

        [Required]
        public bool IsOpen { get; set; }

        [Required]
        public int RatedCount { get; set; }

        [Required]
        public double Rating { get; set; }
    }
}
