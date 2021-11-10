using MessagePack;

namespace EsperFightersCup.Net
{
    [MessagePackObject]
    public readonly struct GameMatchEvent : IGameEvent
    {
        [Key("matched")] public bool IsMatched { get; }

        [SerializationConstructor]
        public GameMatchEvent(bool matched)
        {
            IsMatched = matched;
        }

        public byte EventCode()
        {
            return GameProtocol.Match;
        }
    }
}
