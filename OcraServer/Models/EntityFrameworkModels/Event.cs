using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OcraServer.Models.EntityFrameworkModels
{
    [Table("Event")]
    public class Event
    {
        [Key, Required, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        public int RestaurantID { get; set; }

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

        [Required]
        [StringLength(445)]
        public string ImgLink { get; set; }

        [Required]
        public DateTime EventDateTime { get; set; }

        public int? AdditionalPrice { get; set; }

		[Required]
		[JsonIgnore]
		public bool IsActive { get; set; } = true;

        [JsonIgnore]
        [ForeignKey(nameof(RestaurantID))]
        public virtual Restaurant Restaurant { get; set; }
    }
}
