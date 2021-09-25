public static class GameProtocol
{
    public const byte GameMatchEvent = 0x01;
    public const byte GamePlayerEvent = 0x02;

    public enum GameMatch
    {
        Success,
        Fail
    }

    public enum GamePlayer
    {
        Join,
        Ready,
        Leave
    }
}
