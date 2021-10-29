using MessagePack;

namespace EsperFightersCup.Net
{
    public enum GameMatchResults
    {
        Success,
        Fail
    }

    [MessagePackObject]
    public readonly struct GameMatchPacket : IPacket
    {
        [IgnoreMember] public byte EventCode => GameProtocol.GameMatchEvent;

        [Key(0)] public GameMatchResults MatchResult { get; }

        [SerializationConstructor]
        public GameMatchPacket(GameMatchResults result)
        {
            MatchResult = result;
        }
    }
}
