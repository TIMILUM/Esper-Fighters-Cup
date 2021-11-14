using MessagePack;

using UnityEngine;

namespace EsperFightersCup.Net
{
    [MessagePackObject]
    public readonly struct GameFragmentAreaGenEvent : IGameEvent
    {
        public byte EventCode => GameProtocol.FragmentAreaGen;

        [Key(0)] public int FragmentAuthorViewID { get; }
        [Key(1)] public Vector3 Position { get; }
        [Key(2)] public float Range { get; }

        [SerializationConstructor]
        public GameFragmentAreaGenEvent(int author, Vector3 pos, float range)
        {
            FragmentAuthorViewID = author;
            Position = pos;
            Range = range;
        }
    }
}
