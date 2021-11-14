using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace EsperFightersCup.Net
{
    public static class EventSendOptions
    {
        public static readonly RaiseEventOptions Broadcast = new RaiseEventOptions
        {
            Receivers = ReceiverGroup.All,
            CachingOption = EventCaching.DoNotCache
        };

        public static readonly RaiseEventOptions BroadcastCached = new RaiseEventOptions
        {
            Receivers = ReceiverGroup.All,
            CachingOption = EventCaching.AddToRoomCache
        };

        public static readonly RaiseEventOptions SendOthers = new RaiseEventOptions
        {
            Receivers = ReceiverGroup.Others,
            CachingOption = EventCaching.DoNotCache
        };

        public static readonly RaiseEventOptions SendOthersCached = new RaiseEventOptions
        {
            Receivers = ReceiverGroup.Others,
            CachingOption = EventCaching.AddToRoomCache
        };
    }

    /// <summary>
    /// 패킷을 보내는 유틸 클래스입니다.
    /// </summary>
    public static class EventSender
    {
        /// <summary>
        /// 이벤트를 자신을 포함한 룸의 모든 플레이어에게 보냅니다.
        /// </summary>
        /// <returns></returns>
        public static bool Broadcast<T>(in T eventData, SendOptions sendOption, RaiseEventOptions eventOption = null) where T : IGameEvent
        {
            eventOption ??= EventSendOptions.Broadcast;
            Debug.Log($"<color=grey>[Packet Check] send: {eventData.EventCode}, {sendOption.DeliveryMode}, sender is {PhotonNetwork.LocalPlayer.ActorNumber}</color>");
            return PhotonNetwork.RaiseEvent(eventData.EventCode, eventData, eventOption, sendOption);
        }
    }
}
