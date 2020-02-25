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
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public bool ChunkExists(int x, int z)
        {
            var locationIdx = XZToLocationTableIndex(x, z);
            return LocationTable[locationIdx].Offset != 0;
        }

        /// <summary>
        /// Returns the NBT data of a chunk.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public KeyValuePair<string, dynamic> GetChunk(int x, int z)
        {
            var locationIdx = XZToLocationTableIndex(x, z);
            var chunkOffset = LocationTable[locationIdx].Offset;

            // if offset is 0, the chunk doesn't exist in the file
            if (chunkOffset == 0)
                throw new ArgumentOutOfRangeException("Chunk doesn't exist.");

            r.BaseStream.Position = chunkOffset;

            var chunkLength = r.ReadInt32();
            var compression = (CompressionType)r.ReadByte();

            var bytes = r.ReadBytes(chunkLength);
            return NbtReader.FromMemory(bytes);
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

        private static int XZToLocationTableIndex(int x, int z)
        {
            return ((x % Dimension) + (z % Dimension) * Dimension) * 4;
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
