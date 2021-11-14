using MessagePack;

using UnityEngine;

namespace EsperFightersCup.Net
{
    [MessagePackObject]
    public readonly struct GameParticlePlayEvent : IGameEvent
    {
        public byte EventCode => GameProtocol.ParticlePlay;

        [Key(0)] public string Name { get; }
        [Key(1)] public Vector3 Position { get; }
        [Key(2)] public Vector3 Angle { get; }

        [SerializationConstructor]
        public GameParticlePlayEvent(string name, Vector3 pos, Vector3 angle)
        {
            Name = name;
            Position = pos;
            Angle = angle;
        }
    }
}
