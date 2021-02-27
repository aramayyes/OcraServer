using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using OcraServer.Models.EntityFrameworkModels;

namespace OcraServer.Models.ViewModels
{
	public class RestaurantDataForReservation
	{
        [JsonIgnore]
        [Required]
        public int ID { get; set; }

        [Required]
		public int TableCount { get; set; }
        public virtual IEnumerable<RestaurantPanorama> Panoramas { get; set; } = new HashSet<RestaurantPanorama>();

		[Required]
		public bool IsOpen24 { get; set; } = false;

        [Required]
        public string RestaurantName { get; set; }

		public TimeSpan? OpeningTime { get; set; }

		public TimeSpan? ClosingTime { get; set; }

		public TimeSpan? AdditionalOpeningTime { get; set; }

		public TimeSpan? AdditionalClosingTime { get; set; }
	}
}
