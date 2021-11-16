using MessagePack;

using UnityEngine;

namespace EsperFightersCup.Net
{
    [MessagePackObject]
    public readonly struct GameBuffGenerateEvent : IGameEvent
    {
        [Key(0)] public int TargetViewID { get; }
        [Key(1)] public int Type { get; }
        [Key(2)] public string BuffId { get; }
        [Key(3)] public float Duration { get; }
        [Key(4)] public float[] ValueFloat { get; }
        [Key(5)] public Vector3[] ValueVector3 { get; }
        [Key(6)] public bool AllowDuplicates { get; }
        [Key(7)] public float Damage { get; }
        [Key(8)] public bool IsOnlyOnce { get; }

        [SerializationConstructor]
        public GameBuffGenerateEvent(int target, int type, string id, float duration,
            float[] vfloat, Vector3[] vvec3, bool dup, float damage, bool onlyonce)
        {
            TargetViewID = target;
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
