using OcraServer.Enums;
using System;

namespace OcraServer.Models.NotificationModels
{
    public class UserAddedReservationNotificationModel
    {
        public int ReservationID { get; set; }

        public DateTime ReservationDateTime { get; set; }
        public int? SumPrice { get; set; }
        public int? TableNumber { get; set; }
        public int? PeopleCount { get; set; }
        public string Note { get; set; }
        public ReservationStatus Status { get; set; }
    }
}
