using System;
using System.ComponentModel.DataAnnotations;

namespace OcraServer.Models.ViewModels
{
    public class ReservationProductViewModel
    {
		[Required]
		public int ReservationID { get; set; }

		[Required]
		public int Count { get; set; }

		[Required]
		public int Price { get; set; }

        [Required]
		public virtual ProductInReservationViewModel Product { get; set; }
    }
}
