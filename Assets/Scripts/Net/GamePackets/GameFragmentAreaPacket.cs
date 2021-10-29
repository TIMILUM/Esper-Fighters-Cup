using MessagePack;

using UnityEngine;

namespace EsperFightersCup.Net
{
    [MessagePackObject]
    public readonly struct GameFragmentAreaPacket : IPacket, IPhotonViewPacket
    {
        [IgnoreMember] public byte EventCode => GameProtocol.GameFragmentAreaEvent;

        [Key(0)] public int ViewID { get; }
        [Key(1)] public Vector3 Position { get; }
        [Key(2)] public float Range { get; }

        public GameFragmentAreaPacket(int viewID, Vector3 position, float range)
        {
            ViewID = viewID;
            Position = position;
            Range = range;
        }
    }
}
