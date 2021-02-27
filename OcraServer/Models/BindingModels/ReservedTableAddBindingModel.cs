using System;
using System.ComponentModel.DataAnnotations;

namespace OcraServer.Models.BindingModels
{
    public class ReservedTableAddBindingModel
    {
        [Required]
        public int TableNumber { get; set; }

        [Required]
        public DateTime ReservationDateTime { get; set; }
    }
}
