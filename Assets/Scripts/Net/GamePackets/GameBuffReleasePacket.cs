using MessagePack;

namespace EsperFightersCup.Net
{
    [MessagePackObject]
    public readonly struct GameBuffReleasePacket : IPacket, IPhotonViewPacket
    {
        [IgnoreMember] public byte EventCode => GameProtocol.GameBuffReleaseEvent;

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
