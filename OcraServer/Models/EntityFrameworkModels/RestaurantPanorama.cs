using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OcraServer.Models.EntityFrameworkModels
{
    [Table("RestaurantPanorama")]
    public class RestaurantPanorama
    {
        [JsonIgnore]
        [Key, Required, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        public string PanoramaLink { get; set; }

        [JsonIgnore]
        [Required]
        public int RestaurantID { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(RestaurantID))]
        public virtual Restaurant Restaurant { get; set; }
    }
}
