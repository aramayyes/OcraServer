using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace OcraServer.Models.EntityFrameworkModels
{
    [Table("ExternalReservation")]
    public class ExternalReservation
    {
        [Key, Required, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        public int RestaurantID { get; set; }

        public int? TableNumber { get; set; }
        public int? PeopleCount { get; set; }

        [Required]
        public DateTime ReservationDateTime { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(RestaurantID))]
        public virtual Restaurant Restaurant { get; set; }
    }
}
