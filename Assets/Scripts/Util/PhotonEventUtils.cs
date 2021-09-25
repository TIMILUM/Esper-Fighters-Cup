using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;

internal static class PhotonEventUtils
{
    /// <summary>
    /// 이벤트를 자신을 포함한 룸의 모든 플레이어에게 보냅니다.
    /// </summary>
    /// <param name="eventCode">이벤트 코드</param>
    /// <param name="data">이벤트 데이터</param>
    /// <param name="sendOption">이벤트를 보내는 옵션. Reliable or UnReliable</param>
    /// <param name="isCaching">이벤트를 캐싱하여 수신하지 못한 플레이어가 나중에 받을 수 있도록 합니다.</param>
    /// <returns></returns>
    public static bool BroadcastEvent(byte eventCode, object data, SendOptions sendOption, bool isCaching = false)
    {
        var option = new RaiseEventOptions
        {
            Receivers = ReceiverGroup.All,
            CachingOption = isCaching ? EventCaching.AddToRoomCache : EventCaching.DoNotCache
        };

        return PhotonNetwork.RaiseEvent(eventCode, data, option, sendOption);
    }
}
