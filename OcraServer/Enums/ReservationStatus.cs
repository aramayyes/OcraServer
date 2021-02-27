namespace OcraServer.Enums
{
    public enum ReservationStatus
    {
        WaitingForAcceptance,
        CancelledByUser,
        Accepted,
        Rejected,
        CancelledByUserAfterAcceptance,
        CancelledByAgentAfterAcceptance,
        Done
    }
}
