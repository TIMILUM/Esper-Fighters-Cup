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

            RegisterEvent<GameUIPlayArguments>(EventCode.PlayGameUI);
            RegisterEvent<GameParticlePlayArguments>(EventCode.PlayParticle);
            RegisterEvent<GameParticlePlayAttachedArguments>(EventCode.PlayParticleAttached);
            RegisterEvent<GameSoundPlayArguments>(EventCode.PlaySound);
            RegisterEvent<BuffGenerateArguments>(EventCode.BuffGenerate);
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
            PhotonPeer.RegisterType(typeof(T), code, Serialize<T>, Deserialize<T>);
        }

        private static byte[] Serialize<T>(object customObjcet) where T : IGameEvent
        {
            var targetObject = (T)customObjcet;
            var bytes = MessagePackSerializer.Serialize(targetObject);
            return bytes;
        }

        private static object Deserialize<T>(byte[] buffer) where T : IGameEvent
        {
            var deserialized = MessagePackSerializer.Deserialize<T>(buffer);
            return deserialized;
        }
    }
}
