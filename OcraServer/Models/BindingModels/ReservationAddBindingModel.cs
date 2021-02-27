using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OcraServer.Models.BindingModels
{
    public class ReservationAddBindingModel
    {
        [Required]
        public int RestaurantID { get; set; }

        [Required]
        public DateTime ReservationDateTime { get; set; }

        public int? TableNumber { get; set; }
        public int? PeopleCount { get; set; }

        [StringLength(500)]
        public string Note { get; set; }

        [Required]
        public virtual ICollection<ReservationProductBindingModel> OrderedProducts { get; set; } = new HashSet<ReservationProductBindingModel>();
    }

    public class ReservationProductBindingModel
    {
        [Required]
        public int ProductID { get; set; }

        [Required]
        public int Count { get; set; }
    }
}
