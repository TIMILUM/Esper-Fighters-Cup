using MessagePack;

namespace EsperFightersCup.Net
{
    public interface IPacket
    {
        [IgnoreMember] byte EventCode { get; }
    }

    public interface IPhotonViewPacket
    {
        /// <summary>
        /// 이벤트를 발생시킨 오브젝트의 ViewId입니다.
        /// </summary>
        int ViewID { get; }
    }
}
