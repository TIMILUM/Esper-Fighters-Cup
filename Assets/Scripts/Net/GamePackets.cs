using MessagePack;
using UnityEngine;

namespace EsperFightersCup.Net
{
    public interface IPacket
    {
    }

    public interface IPhotonViewPacket
    {
        /// <summary>
        /// 이벤트를 발생시킨 오브젝트의 ViewId입니다.
        /// </summary>
        int ViewID { get; }
    }

    /// <summary>
    /// 게임패킷을 직렬화/역직렬화해주는 유틸 클래스입니다.
    /// </summary>
    public static class PacketSerializer
    {
        /// <summary>
        /// 매개변수로 받은 패킷을 직렬화하여 byte 배열로 반환합니다.
        /// </summary>
        /// <typeparam name="T">게임패킷 타입</typeparam>
        /// <param name="packet">직렬화할 패킷</param>
        /// <returns>직렬화된 byte 배열</returns>
        public static byte[] Serialize<T>(in T packet) where T : IPacket
        {
            return MessagePackSerializer.Serialize(packet);
        }

        /// <summary>
        /// byte 배열을 역직렬화하여 반환합니다.
        /// </summary>
        /// <typeparam name="T">게임패킷 타입</typeparam>
        /// <param name="buffer">역직렬화할 byte 배열</param>
        /// <returns>역직렬화된 게임패킷</returns>
        public static T Deserialize<T>(byte[] buffer) where T : IPacket
        {
            return MessagePackSerializer.Deserialize<T>(buffer);
        }
    }

    public enum GameMatchResults
    {
        Success,
        Fail
    }

    [MessagePackObject]
    public readonly struct GameMatchPacket : IPacket
    {
        [Key(0)] public GameMatchResults MatchResult { get; }


        [SerializationConstructor]
        public GameMatchPacket(GameMatchResults result)
        {
            MatchResult = result;
        }
    }

    [MessagePackObject]
    public readonly struct GameBuffGeneratePacket : IPacket, IPhotonViewPacket
    {
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

    [MessagePackObject]
    public readonly struct GameBuffReleasePacket : IPacket, IPhotonViewPacket
    {
        [Key(0)] public int ViewID { get; }
        [Key(1)] public string BuffId { get; }

        [SerializationConstructor]
        public GameBuffReleasePacket(int viewID, string buffId)
        {
            ViewID = viewID;
            BuffId = buffId;
        }
    }
}
