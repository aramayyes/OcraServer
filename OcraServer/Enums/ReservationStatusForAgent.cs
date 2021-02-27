using System;
namespace OcraServer.Enums
{
    public enum ReservationStatusForAgent
    {
		WaitingForAcceptance,
		Accepted,
		Done,
        Rejected,
        CancelledByUser
    }
}
