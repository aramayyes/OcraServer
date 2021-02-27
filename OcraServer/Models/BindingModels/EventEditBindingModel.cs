using System;
using System.ComponentModel.DataAnnotations;

namespace OcraServer.Models.BindingModels
{
    public class EventEditBindingModel
    {
        [Required]
        public int ID { get; set; }

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
        public string DescriptionRus { get; set; }

        [StringLength(2000)]
        public string DescriptionEng { get; set; }

        public int? AdditionalPrice { get; set; }

        [Required]
        [StringLength(445)]
        public string ImgLink { get; set; }
         
        [Required]
        public DateTime EventDateTime { get; set; }
    }
}
