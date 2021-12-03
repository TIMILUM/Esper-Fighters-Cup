using MessagePack;
using UnityEngine;

namespace EsperFightersCup.Net
{
    [MessagePackObject]
    public readonly struct GameUIPlayArguments : IGameEvent
    {
        [Key(0)] public string Name { get; }
        [Key(1)] public Vector2 Position { get; }
        [Key(2)] public float RotationY { get; }
        [Key(3)] public Vector2 Scale { get; }
        [Key(4)] public float Duration { get; }
        [Key(5)] public int ViewID { get; }

        public GameUIPlayArguments(string name, Vector2 position, float rotationY, Vector2 scale, float duration, int viewID)
        {
            Name = name;
            Position = position;
            RotationY = rotationY;
            Scale = scale;
            Duration = duration;
            ViewID = viewID;
        }

        public byte GetEventCode()
        {
            return EventCode.PlayGameUI;
        }
    }
}
