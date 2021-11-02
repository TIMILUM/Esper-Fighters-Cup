using MessagePack;

namespace EsperFightersCup.Net
{
    [Union(GameProtocol.GameMatch, typeof(GameMatchEvent))]
    [Union(GameProtocol.BuffGenerate, typeof(GameBuffGenerateEvent))]
    [Union(GameProtocol.BuffRelease, typeof(GameBuffReleaseEvent))]
    [Union(GameProtocol.ParticlePlay, typeof(GameParticlePlayEvent))]
    [Union(GameProtocol.FragmentAreaGen, typeof(GameFragmentAreaGenEvent))]
    [Union(GameProtocol.AnimatorTriggerSync, typeof(GameAnimatorTriggerSyncEvent))]
    public interface IGameEvent
    {
        byte EventCode();
    }
}
