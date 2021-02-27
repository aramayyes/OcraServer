using System;
using System.ComponentModel.DataAnnotations;

namespace OcraServer.Models.ViewModels
{
    public class RestaurantMenuCategoryViewModel
    {
		[Required]
		public int ID { get; set; }

		[Required]
		public string Name { get; set; }

		[Required]
		public int RestaurantID { get; set; }
    }
}
