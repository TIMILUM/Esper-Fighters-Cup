using MessagePack;

using UnityEngine;

namespace EsperFightersCup.Net
{
    [MessagePackObject]
    public readonly struct GameBuffGenerateEvent : IGameEvent
    {
        [Key("target")] public int TargetViewID { get; }
        [Key("type")] public BuffObject.Type Type { get; }
        [Key("id")] public string BuffId { get; }
        [Key("duration")] public float Duration { get; }
        [Key("vfloat")] public float[] ValueFloat { get; }
        [Key("vvec3")] public Vector3[] ValueVector3 { get; }
        [Key("dup")] public bool AllowDuplicates { get; }
        [Key("damage")] public float Damage { get; }
        [Key("onlyonce")] public bool IsOnlyOnce { get; }

        [SerializationConstructor]
        public GameBuffGenerateEvent(int target, BuffObject.Type type, string id, float duration,
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

        public GameBuffGenerateEvent(int viewID, string buffId, BuffObject.BuffStruct buff)
        {
            TargetViewID = viewID;
            Type = buff.Type;
            BuffId = buffId;
            Duration = buff.Duration;
            ValueFloat = buff.ValueFloat;
            ValueVector3 = buff.ValueVector3;
            AllowDuplicates = buff.AllowDuplicates;
            Damage = buff.Damage;
            IsOnlyOnce = buff.IsOnlyOnce;
        }

        public static explicit operator BuffObject.BuffStruct(GameBuffGenerateEvent packet)
        {
            return new BuffObject.BuffStruct
            {
                Type = packet.Type,
                Duration = packet.Duration,
                ValueFloat = packet.ValueFloat,
                ValueVector3 = packet.ValueVector3,
                AllowDuplicates = packet.AllowDuplicates,
                Damage = packet.Damage,
                IsOnlyOnce = packet.IsOnlyOnce
            };
        }

        public byte EventCode()
        {
            return GameProtocol.BuffGenerate;
        }
    }
}