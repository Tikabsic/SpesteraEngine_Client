using System.Diagnostics;
public static class NetworkCredits
{
    public static uint PlayerId { get; set; }
    public static string SecretKey {  get; set; }
    public static bool IsIdSet { get; set; }

    public static void SetPlayerId(uint playerId)
    {
        PlayerId = playerId;
        IsIdSet = true;
    }
}