using System;
using System.ComponentModel.DataAnnotations;

namespace OcraServer.Models.BindingModels
{
    public class ExternalReservationBindingModel
    {
		public int? TableNumber { get; set; }
		public int? PeopleCount { get; set; }

		[Required]
		public DateTime ReservationDateTime { get; set; }

	}
}
