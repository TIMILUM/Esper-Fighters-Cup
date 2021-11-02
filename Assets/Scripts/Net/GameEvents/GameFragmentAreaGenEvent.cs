using MessagePack;

using UnityEngine;

namespace EsperFightersCup.Net
{
    [MessagePackObject]
    public readonly struct GameFragmentAreaGenEvent : IGameEvent
    {
        [Key("author")] public int FragmentAuthorViewID { get; }
        [Key("pos")] public Vector3 Position { get; }
        [Key("range")] public float Range { get; }

        [SerializationConstructor]
        public GameFragmentAreaGenEvent(int author, Vector3 pos, float range)
        {
            FragmentAuthorViewID = author;
            Position = pos;
            Range = range;
        }

        public byte EventCode()
        {
            return GameProtocol.FragmentAreaGen;
        }
    }
}
