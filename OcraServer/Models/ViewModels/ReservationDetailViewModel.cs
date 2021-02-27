using OcraServer.Models.EntityFrameworkModels;
using System;
using System.Collections.Generic;
using OcraServer.Enums;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace OcraServer.Models.ViewModels
{
    public class ReservationDetailViewModel
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
        public int? PeopleCount { get; set; }

        [StringLength(500)]
        public string Note { get; set; }

        [Required]
        public ReservationStatus Status { get; set; }

		[JsonIgnore]
		[Required]
		public bool IsActive { get; set; } = true;
     
        [Required]
        public string RestoName { get; set; }

		[Required]
		public string RestoImgLink { get; set; }

		[Required]
		public string RestoAddress { get; set; }

        [JsonIgnore]
        public Restaurant Rest { get; set; }

        public virtual IEnumerable<ReservationProductViewModel> OrderedProducts { get; set; } = new HashSet<ReservationProductViewModel>();
    }
}
