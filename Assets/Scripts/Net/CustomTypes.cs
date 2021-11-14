using ExitGames.Client.Photon;
using MessagePack;
using MessagePack.Resolvers;
using MessagePack.Unity;
using MessagePack.Unity.Extension;
using UnityEngine;

namespace EsperFightersCup.Net
{
    public class CustomTypes
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void RegisterTypes()
        {
            InitMessagePack();

            RegisterEvent<GameBuffGenerateEvent>(GameProtocol.BuffGenerate);
            RegisterEvent<GameBuffReleaseEvent>(GameProtocol.BuffRelease);
            RegisterEvent<GameFragmentAreaGenEvent>(GameProtocol.FragmentAreaGen);
            RegisterEvent<GameParticlePlayEvent>(GameProtocol.ParticlePlay);
            RegisterEvent<GameAnimatorTriggerSyncEvent>(GameProtocol.AnimatorTriggerSync);
        }

        private static void InitMessagePack()
        {
            StaticCompositeResolver.Instance.Register(
                UnityResolver.Instance,
                UnityBlitWithPrimitiveArrayResolver.Instance,
                StandardResolver.Instance);

            var options = MessagePackSerializerOptions
                .Standard
                .WithResolver(StaticCompositeResolver.Instance)
                .WithSecurity(MessagePackSecurity.TrustedData);

            MessagePackSerializer.DefaultOptions = options;
        }

        private static void RegisterEvent<T>(byte code) where T : IGameEvent
        {
            PhotonPeer.RegisterType(typeof(T), code, EventSerializer.Serialize<T>, EventSerializer.Deserialize<T>);
        }
    }
}
