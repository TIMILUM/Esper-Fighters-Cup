using MessagePack;

using UnityEngine;

namespace EsperFightersCup.Net
{
    [MessagePackObject]
    public readonly struct GameParticlePlayEvent : IGameEvent
    {
        [Key("name")] public string Name { get; }
        [Key("pos")] public Vector3 Position { get; }
        [Key("angle")] public Vector3 Angle { get; }

        [SerializationConstructor]
        public GameParticlePlayEvent(string name, Vector3 pos, Vector3 angle)
        {
            Name = name;
            Position = pos;
            Angle = angle;
        }

        public byte EventCode()
        {
            return GameProtocol.ParticlePlay;
        }
    }
}
