using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OcraServer.Models.EntityFrameworkModels
{
    [Table("RestaurantImg")]
    public class RestaurantImg
    {
        [JsonIgnore]
        [Key, Required, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [JsonIgnore]
        [Required]
        public int RestaurantID { get; set; }

        [Required]
        [StringLength(445)]
        public string ImgLink { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(RestaurantID))]
        public virtual Restaurant Restaurant { get; set; }
    }
}
