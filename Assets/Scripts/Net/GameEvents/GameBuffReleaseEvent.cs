using MessagePack;

namespace EsperFightersCup.Net
{
    [MessagePackObject]
    public readonly struct GameBuffReleaseEvent : IGameEvent
    {
        [Key(0)] public int TargetViewID { get; }
        [Key(1)] public string BuffId { get; }

        [SerializationConstructor]
        public GameBuffReleaseEvent(int target, string id)
        {
            TargetViewID = target;
            BuffId = id;
        }

        public byte GetEventCode()
        {
            return EventCode.BuffRelease;
        }
    }
}
