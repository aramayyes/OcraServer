using System.ComponentModel.DataAnnotations;

namespace OcraServer.Models.ViewModels
{
    public class ProductViewModel
    {
		[Required]
		public int ID { get; set; }

		[Required]
		public int RestaurantID { get; set; }

		[Required]
		[StringLength(100)]
		public string Name { get; set; }

		[StringLength(2000)]
		public string Description { get; set; }

		[Required]
		public int CategoryID { get; set; }

		[Required]
		public int Price { get; set; }

		[Required]
		[StringLength(445)]
		public string ImgLink { get; set; }
    }
}
