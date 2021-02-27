using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#pragma warning disable 1591

namespace OcraServer.Models.EntityFrameworkModels
{
    [Table("Discount")]
    public class Discount
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
        public string DescriptionEng { get; set; }

        [StringLength(2000)]
        public string DescriptionRus { get; set; }

        [Required]
        [StringLength(445)]
        public string ImgLink { get; set; }

        public int? NewPrice { get; set; }
        public int? DiscountSize { get; set; }

        public DateTime? Deadline { get; set; }

        [Required]
        [JsonIgnore]
        public bool IsActive { get; set; } = true;

        [JsonIgnore]
        [ForeignKey(nameof(RestaurantID))]
        public virtual Restaurant Restaurant { get; set; }
    }
}
