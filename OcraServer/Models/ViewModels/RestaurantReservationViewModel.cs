﻿using System;
using System.ComponentModel.DataAnnotations;
using OcraServer.Enums;

namespace OcraServer.Models.ViewModels
{
    public class RestaurantReservationViewModel
    {
		[Required]
		public int ID { get; set; }

		[Required]
		public DateTime ReservationDateTime { get; set; }

		public int? SumPrice { get; set; }
		public int? TableNumber { get; set; }
        public int? PeopleCount { get; set; }

		[StringLength(500)]
		public string Note { get; set; }

		[Required]
		public ReservationStatus Status { get; set; }

        [Required]
        public string UserFirstName { get; set; }

		[Required]
		public string UserLastName { get; set; }

		[Required]
		public string UserPhoneNumber { get; set; }

    }
}
