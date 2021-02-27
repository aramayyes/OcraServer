using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using OcraServer.Enums;
using OcraServer.Models.EntityFrameworkModels;

namespace OcraServer.Models.ViewModels
{
    public class RestaurantReservationDetailViewModel
    {
		[Required]
		public int ID { get; set; }

		[Required]
		public int RestaurantID { get; set; }

		[Required]
		public DateTime Created { get; set; }

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
		public virtual IEnumerable<ReservationProduct> OrdererdProds { get; set; } = new HashSet<ReservationProduct>();

		public virtual IEnumerable<ReservationProductViewModel> OrderedProducts { get; set; } = new HashSet<ReservationProductViewModel>();
    }
}
