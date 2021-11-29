using MessagePack;
using UnityEngine;

namespace EsperFightersCup.Net
{
    [MessagePackObject]
    public readonly struct GameEffectPlayEvent : IGameEvent
    {
        [Key(0)] public string Id { get; }
        [Key(1)] public Vector3 Position { get; }
        [Key(2)] public Vector3 Rotation { get; }

        public GameEffectPlayEvent(string id, Vector3 position, Vector3 rotation)
        {
            Id = id;
            Position = position;
            Rotation = rotation;
        }

        public byte GetEventCode()
        {
            return EventCode.PlayEffect;
        }
    }
}
