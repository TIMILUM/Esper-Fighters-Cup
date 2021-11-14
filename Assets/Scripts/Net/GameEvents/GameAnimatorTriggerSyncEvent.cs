using MessagePack;

namespace EsperFightersCup.Net
{
    [MessagePackObject]
    public readonly struct GameAnimatorTriggerSyncEvent : IGameEvent
    {
        public byte EventCode => GameProtocol.AnimatorTriggerSync;

        [Key(0)] public int ActorViewID { get; }
        [Key(1)] public string Name { get; }

        [SerializationConstructor]
        public GameAnimatorTriggerSyncEvent(int actor, string name)
        {
            ActorViewID = actor;
            Name = name;
        }
    }
}
