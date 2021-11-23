using MessagePack;
using UnityEngine;

namespace EsperFightersCup.Net
{
    [MessagePackObject]
    public readonly struct BuffGenerateArguments : IGameEvent
    {
        [Key(0)] public int Type { get; }
        [Key(1)] public string BuffId { get; }
        [Key(2)] public float Duration { get; }
        [Key(3)] public float[] ValueFloat { get; }
        [Key(4)] public Vector3[] ValueVector3 { get; }
        [Key(5)] public bool AllowDuplicates { get; }
        [Key(6)] public float Damage { get; }
        [Key(7)] public bool IsOnlyOnce { get; }

        [SerializationConstructor]
        public BuffGenerateArguments(int type, string id, float duration,
            float[] vfloat, Vector3[] vvec3, bool dup, float damage, bool onlyonce)
        {
            Type = type;
            BuffId = id;
            Duration = duration;
            ValueFloat = vfloat;
            ValueVector3 = vvec3;
            AllowDuplicates = dup;
            Damage = damage;
            IsOnlyOnce = onlyonce;
        }

        public byte GetEventCode()
        {
            return EventCode.BuffGenerate;
        }
    }
}
