using MessagePack;

using UnityEngine;

namespace EsperFightersCup.Net
{
    [MessagePackObject]
    public readonly struct GameBuffGeneratePacket : IPacket, IPhotonViewPacket
    {
        [IgnoreMember] public byte EventCode => GameProtocol.GameBuffGenerateEvent;

        [Key(0)] public int ViewID { get; }
        [Key(1)] public BuffObject.Type Type { get; }
        [Key(2)] public string BuffId { get; }
        [Key(3)] public float Duration { get; }
        [Key(4)] public float[] ValueFloat { get; }
        [Key(5)] public Vector3[] ValueVector3 { get; }
        [Key(6)] public bool AllowDuplicates { get; }
        [Key(7)] public float Damage { get; }
        [Key(8)] public bool IsOnlyOnce { get; }

        [SerializationConstructor]
        public GameBuffGeneratePacket(int viewID, BuffObject.Type type, string buffId, float duration, float[] valueFloat,
            Vector3[] valueVector3, bool allowDuplicates, float damage, bool isOnlyOnce)
        {
            ViewID = viewID;
            Type = type;
            BuffId = buffId;
            Duration = duration;
            ValueFloat = valueFloat;
            ValueVector3 = valueVector3;
            AllowDuplicates = allowDuplicates;
            Damage = damage;
            IsOnlyOnce = isOnlyOnce;
        }

        public GameBuffGeneratePacket(int viewID, string buffId, BuffObject.BuffStruct buff)
        {
            ViewID = viewID;
            Type = buff.Type;
            BuffId = buffId;
            Duration = buff.Duration;
            ValueFloat = buff.ValueFloat;
            ValueVector3 = buff.ValueVector3;
            AllowDuplicates = buff.AllowDuplicates;
            Damage = buff.Damage;
            IsOnlyOnce = buff.IsOnlyOnce;
        }

        public static explicit operator BuffObject.BuffStruct(GameBuffGeneratePacket packet)
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
    }
}
