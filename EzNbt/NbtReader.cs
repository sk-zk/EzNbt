using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Ionic.Zlib;

namespace EzNbt
{
    /// <summary>
    /// A simple NBT reader.
    /// </summary>
    public static class NbtReader
    {
        /// <summary>
        /// Parses an NBT file.
        /// </summary>
        /// <param name="path"></param>
        /// <returns>The main compound.</returns>
        public static KeyValuePair<string, dynamic> Open(string path)
        {
            var stream = StreamCreator.Create(path);
            return ReadNbtStream(stream);
        }

        /// <summary>
        /// Parses NBT data from memory.
        /// </summary>
        /// <param name="nbtBytes"></param>
        /// <returns></returns>
        public static KeyValuePair<string, dynamic> FromMemory(byte[] nbtBytes)
        {
            var stream = StreamCreator.Create(nbtBytes);
            return ReadNbtStream(stream);
        }

        private static KeyValuePair<string, dynamic> ReadNbtStream(MemoryStream stream)
        {
            using (var r = new BigEndianBinaryReader(stream))
            {
                r.BaseStream.Position = 0;
                var mainCompound = ReadTag(r);
                stream.Dispose();
                return mainCompound;
            }
        }

        /// <summary>
        /// Parses a single tag.
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public static KeyValuePair<string, dynamic> ReadTag(BigEndianBinaryReader r)
        {
            var type = (TagType)r.ReadByte();
            var name = r.ReadString();
            dynamic data = null;
            data = ReadPayload(r, type);

            var nbtTag = new KeyValuePair<string, dynamic>(name, data);
            return nbtTag;
        }

        /// <summary>
        /// Parses the payload of a tag.
        /// </summary>
        /// <param name="r"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private static dynamic ReadPayload(BigEndianBinaryReader r, TagType type)
        {
            switch (type)
            {
                case TagType.Compound:
                    var tags = new Dictionary<string, dynamic>();
                    while (r.PeekChar() != (int)TagType.End)
                    {
                        var tag = ReadTag(r);
                        tags.Add(tag.Key, tag.Value);
                    }
                    r.ReadByte(); // consume End tag after last compound
                    return tags;
                case TagType.Byte:
                    return r.ReadSByte();
                case TagType.Short:
                    return r.ReadInt16();
                case TagType.Int:
                    return r.ReadInt32();
                case TagType.Long:
                    return r.ReadInt64();
                case TagType.Float:
                    return r.ReadSingle();
                case TagType.Double:
                    return r.ReadDouble();
                case TagType.ByteArray:
                    return ReadArray<sbyte>(r);
                case TagType.String:
                    return r.ReadString();
                case TagType.List:
                    // TODO: Improve this
                    var listType = (TagType)r.ReadByte();
                    var listLength = r.ReadInt32();
                    var list = new dynamic[listLength];
                    for(int i = 0; i < listLength; i++)
                    {
                        list[i] = ReadPayload(r, listType);
                    }
                    return list;
                case TagType.IntArray:
                    return ReadArray<int>(r);
                case TagType.LongArray:
                    return ReadArray<long>(r);
                default:
                    throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Reads an array of type T. Exists to simplify reading array types.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="r"></param>
        /// <returns></returns>
        private static dynamic ReadArray<T>(BigEndianBinaryReader r)
        {
            dynamic data;
            var length = r.ReadInt32();
            var arr = new T[length];
            for (int i = 0; i < length; i++)
            {
                arr[i] = (T)r.Read<T>();
            }
            data = arr;
            return data;
        }
    }

}
