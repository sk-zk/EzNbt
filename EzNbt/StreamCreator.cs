using System;
using System.Collections.Generic;
using System.IO;
using Ionic.Zlib;
using System.Text;

namespace EzNbt
{
    /// <summary>
    /// Creates a MemoryStream.
    /// </summary>
    internal static class StreamCreator
    {
        public static MemoryStream Create(string path)
        {
            var bytes = File.ReadAllBytes(path);
            return Create(bytes);
        }

        public static MemoryStream Create(byte[] bytes)
        {
            // check zlib
            try
            { 
                bytes = ZlibStream.UncompressBuffer(bytes);
            }
            catch // check gzip
            {
                if (bytes[0] == 0x1f && bytes[1] == 0x8b)
                {
                    bytes = GZipStream.UncompressBuffer(bytes);
                }
            }
            // otherwise, assume uncompressed
            return new MemoryStream(bytes);
        }

    }
}
