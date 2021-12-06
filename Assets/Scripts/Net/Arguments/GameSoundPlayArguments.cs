using MessagePack;

using UnityEngine;

namespace EsperFightersCup.Net
{
    [MessagePackObject]
    public readonly struct GameSoundPlayArguments : IGameEvent
    {
        [Key(0)] public string Name { get; }
        [Key(1)] public Vector3 Position { get; }
        [Key(2)] public string InitParameter { get; }
        [Key(3)] public float ParameterValue { get; }

        [SerializationConstructor]
        public GameSoundPlayArguments(string id, Vector3 pos, string initParam, float value)
        {
            Name = id;
            Position = pos;
            InitParameter = initParam;
            ParameterValue = value;
        }

        public byte GetEventCode()
        {
            return EventCode.PlaySound;
        }
    }
}
