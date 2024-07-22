public static class NetworkCredits
{
    public static uint PlayerId { get; set; }
    public static bool IsIdSet { get; set; }

    public static void SetPlayerId(uint playerId)
    {
        PlayerId = playerId;
        IsIdSet = true;

        Net_ConnectionHandler.Instance.ConnectToUDPServer();
    }
}