using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace OcraServer.Models.EntityFrameworkModels
{
    [Table("RestaurantProductCategory")]
    public class RestaurantMenuCategory
    {
		[Key, Required, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int ID { get; set; }

        [Required]
        public string NameArm { get; set; }

        [Required]
        public string NameRus { get; set; }

        [Required]
        public string NameEng { get; set; }

		[Required]
		public int RestaurantID { get; set; }

		[JsonIgnore]
		[ForeignKey(nameof(RestaurantID))]
		public virtual Restaurant Restaurant { get; set; }

        [JsonIgnore]
        public virtual ICollection<Product> Products { get; set; }
    }
}
