using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace OcraServer.Models.EntityFrameworkModels
{
    [Table("Product")]
    public class Product
    {
        [Key, Required, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        public int RestaurantID { get; set; }

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
		public int CategoryID { get; set; }

        [Required]
        public int Price { get; set; }

        [Required]
        [StringLength(445)]
        public string ImgLink { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;


		[JsonIgnore]
        [ForeignKey(nameof(CategoryID))]
		public virtual RestaurantMenuCategory Category { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(RestaurantID))]
        public virtual Restaurant Restaurant { get; set; }

        [JsonIgnore]
        public virtual ICollection<ReservationProduct> ReservationProducts { get; set; } = new HashSet<ReservationProduct>();
    }
}
