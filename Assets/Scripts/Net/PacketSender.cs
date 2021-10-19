using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;

namespace EsperFightersCup.Net
{
    /// <summary>
    /// 패킷을 보내는 유틸 클래스입니다.
    /// </summary>
    public static class PacketSender
    {
        private static readonly RaiseEventOptions s_broadcastOption = new RaiseEventOptions
        {
            Receivers = ReceiverGroup.All,
            CachingOption = EventCaching.DoNotCache
        };

        private static readonly RaiseEventOptions s_broadcastOptionCached = new RaiseEventOptions
        {
            Receivers = ReceiverGroup.All,
            CachingOption = EventCaching.AddToRoomCache
        };

        /// <summary>
        /// 이벤트를 자신을 포함한 룸의 모든 플레이어에게 보냅니다.
        /// </summary>
        /// <param name="eventCode">이벤트 코드</param>
        /// <param name="buffer">이벤트 데이터</param>
        /// <param name="sendOption">이벤트를 보내는 옵션. Reliable or UnReliable</param>
        /// <param name="isCaching">이벤트를 캐싱하여 수신하지 못한 플레이어가 나중에 받을 수 있도록 합니다.</param>
        /// <returns></returns>
        public static bool Broadcast<T>(in T packet, SendOptions sendOption, bool isCaching = false) where T : IPacket
        {
            var option = isCaching ? s_broadcastOptionCached : s_broadcastOption;
            var buffer = PacketSerializer.Serialize(in packet);
            return PhotonNetwork.RaiseEvent(packet.EventCode, buffer, option, sendOption);
        }
    }
}
