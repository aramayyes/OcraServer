using System;

namespace OcraServer.Models.ViewModels
{
    public class ReservedTableViewModel
    {
        public int ID { get; set; }

        public int? TableNumber { get; set; }

        public DateTime ReservationDateTime { get; set; }
    }
}
