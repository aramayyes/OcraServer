using System.ComponentModel.DataAnnotations;
using OcraServer.Enums;

namespace OcraServer.Models.BindingModels
{
    public class ProductEditBindingModel
    {
		[Required]
		public int ID { get; set; }

		[Required]
		[StringLength(100)]
		public string NameEng { get; set; }

		[Required]
		[StringLength(100)]
		public string NameRus { get; set; }

		[Required]
		[StringLength(100)]
		public string NameArm { get; set; }

		[StringLength(2000)]
		public string DescriptionEng { get; set; }

		[StringLength(2000)]
		public string DescriptionRus { get; set; }

		[StringLength(2000)]
		public string DescriptionArm { get; set; }

		[Required]
		public int Price { get; set; }

		[Required]
		[StringLength(445)]
		public string ImgLink { get; set; }
    }
}
