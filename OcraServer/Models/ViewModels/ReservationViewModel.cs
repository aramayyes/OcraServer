using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using OcraServer.Enums;

namespace OcraServer.Models.ViewModels
{
    public class ReservationViewModel
    {
        [Required]
        public int ID { get; set; }

        [Required]
        public string UserID { get; set; }

        [Required]
        public int RestaurantID { get; set; }

        [Required]
        public DateTime ReservationDateTime { get; set; }

        public int? SumPrice { get; set; }
        public int? TableNumber { get; set; }

        [Required]
        public ReservationStatus Status { get; set; }

		[JsonIgnore]
		[Required]
		public bool IsActive { get; set; } = true;

		[Required]
        public string RestoName { get; set; }

		[Required]
		public string RestoImgLink { get; set; }
    }
}
