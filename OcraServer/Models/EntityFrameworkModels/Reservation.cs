using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using OcraServer.Enums;

namespace OcraServer.Models.EntityFrameworkModels
{
    [Table("Reservation")]
    public class Reservation
    {
        [Key, Required, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        public string UserID { get; set; }

        [Required]
        public int RestaurantID { get; set; }

        [Required]
        public DateTime Created { get; set; }

        [Required]
        public DateTime ReservationDateTime { get; set; }

        public int? SumPrice { get; set; }

        public int? TableNumber { get; set; }
        public int? PeopleCount { get; set; }
       
        [StringLength(500)]
        public string Note { get; set; }

        [Required]
        public ReservationStatus Status { get; set; }

        [JsonIgnore]
        [Required]
        public bool IsActive { get; set; } = true;

        [JsonIgnore]
        [ForeignKey(nameof(UserID))]
        public virtual ApplicationUser User { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(RestaurantID))]
        public virtual Restaurant Restaurant { get; set; }

        public virtual ICollection<ReservationProduct> OrderedProducts { get; set; } = new HashSet<ReservationProduct>();
    }
}
