using MessagePack;

namespace EsperFightersCup.Net
{
    [MessagePackObject]
    public readonly struct GameAnimatorTriggerSyncEvent : IGameEvent
    {
        [Key(0)] public int ActorViewID { get; }
        [Key(1)] public string Name { get; }

        [SerializationConstructor]
        public GameAnimatorTriggerSyncEvent(int actor, string name)
        {
            ActorViewID = actor;
            Name = name;
        }

        public byte GetEventCode()
        {
            return EventCode.AnimatorTriggerSync;
        }
    }
}
