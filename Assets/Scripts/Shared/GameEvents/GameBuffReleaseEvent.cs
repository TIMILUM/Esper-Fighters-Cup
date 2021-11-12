using MessagePack;

namespace EsperFightersCup.Net
{
    [MessagePackObject]
    public readonly struct GameBuffReleaseEvent : IGameEvent
    {
        [Key("target")] public int TargetViewID { get; }
        [Key("id")] public string BuffId { get; }

        [SerializationConstructor]
        public GameBuffReleaseEvent(int target, string id)
        {
            TargetViewID = target;
            BuffId = id;
        }

        public byte EventCode()
        {
            return GameProtocol.BuffRelease;
        }
    }
}
