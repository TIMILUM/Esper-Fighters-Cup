using MessagePack;

namespace EsperFightersCup.Net
{
    /// <summary>
    /// 게임패킷을 직렬화/역직렬화해주는 유틸 클래스입니다.
    /// </summary>
    public static class PacketSerializer
    {
        /// <summary>
        /// 매개변수로 받은 패킷을 직렬화하여 byte 배열로 반환합니다.
        /// </summary>
        /// <typeparam name="T">게임패킷 타입</typeparam>
        /// <param name="eventData">직렬화할 패킷</param>
        /// <returns>직렬화된 byte 배열</returns>
        public static byte[] Serialize<T>(in T eventData) where T : IGameEvent
        {
            return MessagePackSerializer.Serialize<IGameEvent>(eventData);
        }

        /// <summary>
        /// byte 배열을 역직렬화하여 반환합니다.
        /// </summary>
        /// <typeparam name="T">게임패킷 타입</typeparam>
        /// <param name="buffer">역직렬화할 byte 배열</param>
        /// <returns>역직렬화된 게임패킷</returns>
        public static IGameEvent Deserialize(byte[] buffer)
        {
            return MessagePackSerializer.Deserialize<IGameEvent>(buffer);
        }
    }
}