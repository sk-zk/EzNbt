using NUnit.Framework;
using EzNbt;
using System;
using System.Collections.Generic;
using System.Text;

namespace EzNbtTests
{
    [TestFixture()]
    public class RegionFileTests
    {
        [Test()]
        public void OpenTest()
        {
            Assert.DoesNotThrow(() => RegionFile.Open("TestFiles/r.0.0.mca"));
        }

        [TestCase(0, 0, true)]
        [TestCase(7, 11, true)]
        [TestCase(14, 14, false)]
        public void ChunkExistsTest(int localX, int localZ, bool result)
        {
            var region = RegionFile.Open("TestFiles/r.0.0.mca");
            Assert.AreEqual(result, region.ChunkExists(localX, localZ));
        }

        [Test()]
        public void ChunkExistsRangeCheckTest()
        {
            var region = RegionFile.Open("TestFiles/r.0.0.mca");
            Assert.Throws<ArgumentOutOfRangeException>(
                () => region.ChunkExists(50, 0));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => region.ChunkExists(0, -50));
            Assert.DoesNotThrow(() => region.ChunkExists(5, 5));
        }

        [Test()]
        public void GetChunkTest()
        {
            var region = RegionFile.Open("TestFiles/r.0.0.mca");
            var chunk = region.GetChunk(4, 2)["Level"];
            Assert.AreEqual(4, chunk["xPos"]);
            Assert.AreEqual(2, chunk["zPos"]);
        }

        [Test()]
        public void GetChunkNotFoundTest()
        {
            var region = RegionFile.Open("TestFiles/r.0.0.mca");
            Assert.Throws<ArgumentOutOfRangeException>(
                () => region.GetChunk(30, 30));
        }

        [Test()]
        public void GetChunkRangeCheckTest()
        {
            var region = RegionFile.Open("TestFiles/r.0.0.mca");
            Assert.Throws<ArgumentOutOfRangeException>(
                () => region.GetChunk(50, 0));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => region.GetChunk(0, -50));
        }

        [Test()]
        public void GetAllChunksTest()
        {
            var region = RegionFile.Open("TestFiles/r.0.0.mca");
            var allChunks = region.GetAllChunks();
            Assert.AreEqual(96, allChunks.Count);
        }

    }
}