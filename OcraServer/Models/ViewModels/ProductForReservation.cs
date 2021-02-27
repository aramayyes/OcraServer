using System;
using System.ComponentModel.DataAnnotations;

namespace OcraServer.Models.ViewModels
{
    public class ProductForReservation
    {
		[Required]
		public int ID { get; set; }

        [Required]
        public int Price { get; set; }
    }
}
