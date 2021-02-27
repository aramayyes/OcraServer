using System;
using System.ComponentModel.DataAnnotations;

namespace OcraServer.Models.ViewModels
{
    public class ProductInReservationViewModel
    {
		[Required]
		public int ID { get; set; }

		[Required]
		[StringLength(100)]
		public string Name { get; set; }

		[StringLength(2000)]
		public string Description { get; set; }

		[Required]
		[StringLength(445)]
		public string ImgLink { get; set; }
    }
}
