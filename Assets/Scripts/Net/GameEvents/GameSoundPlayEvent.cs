using MessagePack;

using UnityEngine;

namespace EsperFightersCup.Net
{
    [MessagePackObject]
    public readonly struct GameSoundPlayEvent : IGameEvent
    {
        [Key(0)] public string Id { get; }
        [Key(1)] public Vector3 Position { get; }

        [SerializationConstructor]
        public GameSoundPlayEvent(string id, Vector3 pos)
        {
            Id = id;
            Position = pos;
        }

        public byte GetEventCode()
        {
            return EventCode.PlaySound;
        }
    }
}
