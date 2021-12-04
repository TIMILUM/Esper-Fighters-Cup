using MessagePack;

namespace EsperFightersCup.Net
{
    [MessagePackObject]
    public readonly struct GameParticlePlayAttachedArguments : IGameEvent
    {
        [Key(0)] public string Name { get; }
        [Key(1)] public int AttachIndex { get; }

        [SerializationConstructor]
        public GameParticlePlayAttachedArguments(string name, int index)
        {
            Name = name;
            AttachIndex = index;
        }

        public byte GetEventCode()
        {
            return EventCode.PlayParticleAttached;
        }
    }
}
