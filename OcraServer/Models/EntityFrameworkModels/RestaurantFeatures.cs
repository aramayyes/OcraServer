using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace OcraServer.Models.EntityFrameworkModels
{
    [Table("RestaurantFeatures")]
    public class RestaurantFeatures
    {
        [JsonIgnore]
		[Key]
		public int ID { get; set; }

        [Required]
        public bool HasCreditCard { get; set; }
        [Required]
        public bool HasWiFi { get; set; }
        [Required]
        public bool HasKidsZone { get; set; }
        [Required]
        public bool HasNoSmokingZone { get; set; }
        [Required]
        public bool HasTakeAway { get; set; }
        [Required]
        public bool HasOutsideZone { get; set; }

		[JsonIgnore]
		public virtual ICollection<Restaurant> Restaurants { get; set; }
    }
}
