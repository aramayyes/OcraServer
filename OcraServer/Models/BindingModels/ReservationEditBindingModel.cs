using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OcraServer.Models.BindingModels
{
    public class ReservationEditBindingModel
    {
        [Required]
        public int ID { get; set; }

		[Required]
		public DateTime ReservationDateTime { get; set; }

		public int? PeopleCount { get; set; }

		[StringLength(500)]
		public string Note { get; set; }

        public virtual ICollection<ReservationProductBindingModel> OrderedProducts { get; set; } = new HashSet<ReservationProductBindingModel>();
    }
}
