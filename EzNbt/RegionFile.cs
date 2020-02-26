using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Diagnostics;
using Ionic.Zlib;

namespace EzNbt
{
    public class RegionFile : IDisposable
    {
        private BigEndianBinaryReader r;

        private const int Dimension = 32; // n*n chunks in a region

        private const int LocationTableSize = Dimension * Dimension;
        private Location[] LocationTable = new Location[LocationTableSize];

        private const int TimestampTableSize = Dimension * Dimension;
        private int[] TimestampTable = new int[TimestampTableSize];

        /// <summary>
        /// Parses a region file.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static RegionFile Open(string path)
        {
            var region = new RegionFile();

            var stream = StreamCreator.Create(path);
            region.r = new BigEndianBinaryReader(stream);
            region.ReadLocationTable();
            region.ReadTimestampTable();
            return region;
        }

        /// <summary>
        /// Checks if a chunk exists in the region.
        /// </summary>
        /// <param name="localX"></param>
        /// <param name="localZ"></param>
        /// <returns></returns>
        public bool ChunkExists(int localX, int localZ)
        {
            var locationIdx = XZToLocationTableIndex(localX, localZ);
            return LocationTable[locationIdx].Offset != 0;
        }

        /// <summary>
        /// Returns the NBT data of a chunk.
        /// </summary>
        /// <param name="localX"></param>
        /// <param name="localZ"></param>
        /// <returns></returns>
        public Dictionary<string, dynamic> GetChunk(int localX, int localZ)
        {
            var locationIdx = XZToLocationTableIndex(localX, localZ);
            var chunkOffset = LocationTable[locationIdx].Offset;

            // if offset is 0, the chunk doesn't exist in the file
            if (chunkOffset == 0)
                throw new ArgumentOutOfRangeException("Chunk doesn't exist.");

            return GetChunk(chunkOffset);
        }

        private Dictionary<string, dynamic> GetChunk(long chunkOffset)
        {
            r.BaseStream.Position = chunkOffset;

            var chunkLength = r.ReadInt32();
            var compression = (CompressionType)r.ReadByte();

            var bytes = r.ReadBytes(chunkLength);
            return NbtReader.FromMemory(bytes);
        }

        public List<Dictionary<string, dynamic>> GetAllChunks()
        {
            var list = new List<Dictionary<string, dynamic>>();
            foreach (var entry in LocationTable)
            {
                if (entry.Offset == 0) continue;

                list.Add(GetChunk(entry.Offset));
            }
            return list;
        }

        private void ReadTimestampTable()
        {
            for (int i = 0; i < TimestampTableSize; i++)
            {
                TimestampTable[i] = r.ReadInt32();
            }
        }

        private void ReadLocationTable()
        {
            const int factor = 4096;
            for (int i = 0; i < LocationTableSize; i++)
            {
                // there's just nothing quite like
                // reading a 3-byte big endian int
                var oBytes = r.ReadBytes(3);
                var oBytes4 = new byte[4];
                oBytes.CopyTo(oBytes4, 1);
                Array.Reverse(oBytes4);
                var chunkOffset = BitConverter.ToInt32(oBytes4, 0) 
                    * (long)factor;

                var length = r.ReadByte() * factor;

                LocationTable[i] = new Location(chunkOffset, length);
            }
        }

        private static int XZToLocationTableIndex(int localX, int localZ)
        {
            return ((localX % Dimension) + (localZ % Dimension) * Dimension) * 4;
        }

        public void Dispose()
        {
            r.Dispose();
        }
    }

    [DebuggerDisplay("Pos:{Offset}   Len:{Length}")]
    struct Location
    {
        /// <summary>
        /// Position of the chunk data in the file.
        /// </summary>
        public long Offset;

        /// <summary>
        /// Approximate length of the chunk.
        /// </summary>
        public int Length;

        public Location(long offset, int length)
        {
            Offset = offset;
            Length = length;
        }
    }
}
