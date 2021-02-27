using System;
using System.ComponentModel.DataAnnotations;

namespace OcraServer.Models.BindingModels
{
    public class DiscountAddBindingModel
    {
        [Required]
		[StringLength(100)]
		public string NameArm { get; set; }
		[StringLength(100)]
		public string NameRus { get; set; }
		[StringLength(100)]
		public string NameEng { get; set; }

        [Required]
		[StringLength(2000)]
		public string DescriptionArm { get; set; }

        [StringLength(2000)]
        public string DescriptionEng { get; set; }

        [StringLength(2000)]
        public string DescriptionRus { get; set; }

        [Required]
        [StringLength(445)]
        public string ImgLink { get; set; }

        public int? NewPrice { get; set; }
        public int? DiscountSize { get; set; }

        public DateTime? Deadline { get; set; }
    }
}
