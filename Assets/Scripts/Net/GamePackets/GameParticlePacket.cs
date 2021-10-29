using MessagePack;

using UnityEngine;

namespace EsperFightersCup.Net
{
    [MessagePackObject]
    public readonly struct GameParticlePacket : IPacket
    {
        [IgnoreMember] public byte EventCode => GameProtocol.GameParticleEvent;

        [Key(0)] public string Name { get; }
        [Key(1)] public Vector3 Position { get; }
        [Key(2)] public Quaternion Angle { get; }

        [SerializationConstructor]
        public GameParticlePacket(string name, Vector3 pos, Quaternion angle)
        {
            Name = name;
            Position = pos;
            Angle = angle;
        }
    }
}
