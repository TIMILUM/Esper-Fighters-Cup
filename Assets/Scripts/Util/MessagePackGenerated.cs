// <auto-generated>
// THIS (.cs) FILE IS GENERATED BY MPC(MessagePack-CSharp). DO NOT CHANGE IT.
// </auto-generated>

#pragma warning disable 618
#pragma warning disable 612
#pragma warning disable 414
#pragma warning disable 168

#pragma warning disable SA1200 // Using directives should be placed correctly
#pragma warning disable SA1312 // Variable names should begin with lower-case letter
#pragma warning disable SA1649 // File name should match first type name

namespace EsperFightersCup.Resolvers
{
    using System;

    public class GeneratedResolver : global::MessagePack.IFormatterResolver
    {
        public static readonly global::MessagePack.IFormatterResolver Instance = new GeneratedResolver();

        private GeneratedResolver()
        {
        }

        public global::MessagePack.Formatters.IMessagePackFormatter<T> GetFormatter<T>()
        {
            return FormatterCache<T>.Formatter;
        }

        private static class FormatterCache<T>
        {
            internal static readonly global::MessagePack.Formatters.IMessagePackFormatter<T> Formatter;

            static FormatterCache()
            {
                var f = GeneratedResolverGetFormatterHelper.GetFormatter(typeof(T));
                if (f != null)
                {
                    Formatter = (global::MessagePack.Formatters.IMessagePackFormatter<T>)f;
                }
            }
        }
    }

    internal static class GeneratedResolverGetFormatterHelper
    {
        private static readonly global::System.Collections.Generic.Dictionary<Type, int> lookup;

        static GeneratedResolverGetFormatterHelper()
        {
            lookup = new global::System.Collections.Generic.Dictionary<Type, int>(3)
            {
                { typeof(global::EsperFightersCup.Net.GameEffectPlayEvent), 0 },
                { typeof(global::EsperFightersCup.Net.GameParticlePlayEvent), 1 },
                { typeof(global::EsperFightersCup.Net.GameSoundPlayEvent), 2 },
            };
        }

        internal static object GetFormatter(Type t)
        {
            int key;
            if (!lookup.TryGetValue(t, out key))
            {
                return null;
            }

            switch (key)
            {
                case 0: return new EsperFightersCup.Formatters.EsperFightersCup.Net.GameEffectPlayEventFormatter();
                case 1: return new EsperFightersCup.Formatters.EsperFightersCup.Net.GameParticlePlayEventFormatter();
                case 2: return new EsperFightersCup.Formatters.EsperFightersCup.Net.GameSoundPlayEventFormatter();
                default: return null;
            }
        }
    }
}

#pragma warning restore 168
#pragma warning restore 414
#pragma warning restore 618
#pragma warning restore 612

#pragma warning restore SA1312 // Variable names should begin with lower-case letter
#pragma warning restore SA1200 // Using directives should be placed correctly
#pragma warning restore SA1649 // File name should match first type name




// <auto-generated>
// THIS (.cs) FILE IS GENERATED BY MPC(MessagePack-CSharp). DO NOT CHANGE IT.
// </auto-generated>

#pragma warning disable 618
#pragma warning disable 612
#pragma warning disable 414
#pragma warning disable 168

#pragma warning disable SA1129 // Do not use default value type constructor
#pragma warning disable SA1200 // Using directives should be placed correctly
#pragma warning disable SA1309 // Field names should not begin with underscore
#pragma warning disable SA1312 // Variable names should begin with lower-case letter
#pragma warning disable SA1403 // File may only contain a single namespace
#pragma warning disable SA1649 // File name should match first type name

namespace EsperFightersCup.Formatters.EsperFightersCup.Net
{
    using global::System.Buffers;
    using global::MessagePack;
    using UnityEngine;

    public sealed class GameEffectPlayEventFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::EsperFightersCup.Net.GameEffectPlayEvent>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::EsperFightersCup.Net.GameEffectPlayEvent value, global::MessagePack.MessagePackSerializerOptions options)
        {
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(3);
            formatterResolver.GetFormatterWithVerify<string>().Serialize(ref writer, value.Id, options);
            formatterResolver.GetFormatterWithVerify<Vector3>().Serialize(ref writer, value.Position, options);
            formatterResolver.GetFormatterWithVerify<Vector3>().Serialize(ref writer, value.Rotation, options);
        }

        public global::EsperFightersCup.Net.GameEffectPlayEvent Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                throw new global::System.InvalidOperationException("typecode is null, struct not supported");
            }

            options.Security.DepthStep(ref reader);
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var __Id__ = default(string);
            var __Position__ = default(Vector3);
            var __Rotation__ = default(Vector3);

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        __Id__ = formatterResolver.GetFormatterWithVerify<string>().Deserialize(ref reader, options);
                        break;
                    case 1:
                        __Position__ = formatterResolver.GetFormatterWithVerify<Vector3>().Deserialize(ref reader, options);
                        break;
                    case 2:
                        __Rotation__ = formatterResolver.GetFormatterWithVerify<Vector3>().Deserialize(ref reader, options);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            var ____result = new global::EsperFightersCup.Net.GameEffectPlayEvent(__Id__, __Position__, __Rotation__);
            reader.Depth--;
            return ____result;
        }
    }

    public sealed class GameParticlePlayEventFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::EsperFightersCup.Net.GameParticlePlayEvent>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::EsperFightersCup.Net.GameParticlePlayEvent value, global::MessagePack.MessagePackSerializerOptions options)
        {
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(3);
            formatterResolver.GetFormatterWithVerify<string>().Serialize(ref writer, value.Name, options);
            formatterResolver.GetFormatterWithVerify<Vector3>().Serialize(ref writer, value.Position, options);
            formatterResolver.GetFormatterWithVerify<Vector3>().Serialize(ref writer, value.Angle, options);
        }

        public global::EsperFightersCup.Net.GameParticlePlayEvent Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                throw new global::System.InvalidOperationException("typecode is null, struct not supported");
            }

            options.Security.DepthStep(ref reader);
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var __Name__ = default(string);
            var __Position__ = default(Vector3);
            var __Angle__ = default(Vector3);

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        __Name__ = formatterResolver.GetFormatterWithVerify<string>().Deserialize(ref reader, options);
                        break;
                    case 1:
                        __Position__ = formatterResolver.GetFormatterWithVerify<Vector3>().Deserialize(ref reader, options);
                        break;
                    case 2:
                        __Angle__ = formatterResolver.GetFormatterWithVerify<Vector3>().Deserialize(ref reader, options);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            var ____result = new global::EsperFightersCup.Net.GameParticlePlayEvent(__Name__, __Position__, __Angle__);
            reader.Depth--;
            return ____result;
        }
    }

    public sealed class GameSoundPlayEventFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::EsperFightersCup.Net.GameSoundPlayEvent>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::EsperFightersCup.Net.GameSoundPlayEvent value, global::MessagePack.MessagePackSerializerOptions options)
        {
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(2);
            formatterResolver.GetFormatterWithVerify<string>().Serialize(ref writer, value.Id, options);
            formatterResolver.GetFormatterWithVerify<Vector3>().Serialize(ref writer, value.Position, options);
        }

        public global::EsperFightersCup.Net.GameSoundPlayEvent Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                throw new global::System.InvalidOperationException("typecode is null, struct not supported");
            }

            options.Security.DepthStep(ref reader);
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var __Id__ = default(string);
            var __Position__ = default(Vector3);

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        __Id__ = formatterResolver.GetFormatterWithVerify<string>().Deserialize(ref reader, options);
                        break;
                    case 1:
                        __Position__ = formatterResolver.GetFormatterWithVerify<Vector3>().Deserialize(ref reader, options);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            var ____result = new global::EsperFightersCup.Net.GameSoundPlayEvent(__Id__, __Position__);
            reader.Depth--;
            return ____result;
        }
    }
}

#pragma warning restore 168
#pragma warning restore 414
#pragma warning restore 618
#pragma warning restore 612

#pragma warning restore SA1129 // Do not use default value type constructor
#pragma warning restore SA1200 // Using directives should be placed correctly
#pragma warning restore SA1309 // Field names should not begin with underscore
#pragma warning restore SA1312 // Variable names should begin with lower-case letter
#pragma warning restore SA1403 // File may only contain a single namespace
#pragma warning restore SA1649 // File name should match first type name
