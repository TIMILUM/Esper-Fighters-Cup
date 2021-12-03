using MessagePack;

using UnityEngine;

namespace EsperFightersCup.Net
{
    [MessagePackObject]
    public readonly struct GameParticlePlayArguments : IGameEvent
    {
        [Key(0)] public string Name { get; }
        [Key(1)] public Vector3 Position { get; }
        [Key(2)] public Vector3 Angle { get; }

        [SerializationConstructor]
        public GameParticlePlayArguments(string name, Vector3 pos, Vector3 angle)
        {
            Name = name;
            Position = pos;
            Angle = angle;
        }

        public byte GetEventCode()
        {
            return EventCode.PlayParticle;
        }
    }
}
