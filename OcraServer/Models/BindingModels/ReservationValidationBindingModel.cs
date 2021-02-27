using System;

namespace OcraServer.Models.BindingModels
{
    public class ReservationValidationBindingModel
    {
        public int RestID { get; set; }
        public int TableNumber { get; set; }
        public DateTime ReservationDateTime { get; set; }
    }
}
