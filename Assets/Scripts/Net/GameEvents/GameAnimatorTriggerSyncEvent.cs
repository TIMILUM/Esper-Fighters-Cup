using MessagePack;

namespace EsperFightersCup.Net
{
    [MessagePackObject]
    public readonly struct GameAnimatorTriggerSyncEvent : IGameEvent
    {
        [Key("actor")] public int ActorViewID { get; }
        [Key("name")] public string Name { get; }

        [SerializationConstructor]
        public GameAnimatorTriggerSyncEvent(int actor, string name)
        {
            ActorViewID = actor;
            Name = name;
        }

        public byte EventCode()
        {
            return GameProtocol.AnimatorTriggerSync;
        }
    }
}
