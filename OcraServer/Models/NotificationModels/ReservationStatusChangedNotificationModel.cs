using System;
using OcraServer.Enums;

namespace OcraServer.Models.NotificationModels
{
    public class ReservationStatusChangedNotificationModel
    {
        public int ReservationID { get; set; }

        public string RestaurantNameArm { get; set; }
        public string RestaurantNameRus { get; set; }
        public string RestaurantNameEng { get; set; }

        public DateTime ReservationDateTime { get; set; }
        public ReservationStatus Status { get; set; }
    }
}
