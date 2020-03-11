using NUnit.Framework;
using EzNbt;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;

namespace EzNbtTests
{
    [TestFixture]
    public class BigEndianBinaryReaderTests
    {
        private BigEndianBinaryReader CreateReader(byte[] bytes)
        {
            var reader = new BigEndianBinaryReader(new MemoryStream(bytes));
            return reader;
        }

        [Test()]
        public void ReadInt16Test()
        {
            var val = (short)-3456;
            var reader = CreateReader(BitConverter.GetBytes(val).Reverse().ToArray());
            Assert.AreEqual(val, reader.ReadInt16());
        }

        [Test()]
        public void ReadUInt16Test()
        {
            var val = (ushort)3456;
            var reader = CreateReader(BitConverter.GetBytes(val).Reverse().ToArray());
            Assert.AreEqual(val, reader.ReadUInt16());
        }

        [Test()]
        public void ReadInt32Test()
        {
            var val = -292202;
            var reader = CreateReader(BitConverter.GetBytes(val).Reverse().ToArray());
            Assert.AreEqual(val, reader.ReadInt32());
        }

        [Test()]
        public void ReadUInt32Test()
        {
            var val = (uint)292202;
            var reader = CreateReader(BitConverter.GetBytes(val).Reverse().ToArray());
            Assert.AreEqual(val, reader.ReadUInt32());
        }

        [Test()]
        public void ReadInt64Test()
        {
            var val = -98765432123456789L;
            var reader = CreateReader(BitConverter.GetBytes(val).Reverse().ToArray());
            Assert.AreEqual(val, reader.ReadInt64());
        }

        [Test()]
        public void ReadUInt64Test()
        {
            var val = 98765432123456789UL;
            var reader = CreateReader(BitConverter.GetBytes(val).Reverse().ToArray());
            Assert.AreEqual(val, reader.ReadUInt64());
        }

        [Test()]
        public void ReadSingleTest()
        {
            var val = 42.175f;
            var reader = CreateReader(BitConverter.GetBytes(val).Reverse().ToArray());
            Assert.AreEqual(val, reader.ReadSingle());
        }

        [Test()]
        public void ReadDoubleTest()
        {
            var val = 4242.4242;
            var reader = CreateReader(BitConverter.GetBytes(val).Reverse().ToArray());
            Assert.AreEqual(val, reader.ReadDouble());
        }

        [Test()]
        public void ReadStringTest()
        {
            var str = "helloÄÖÜß";
            var strBytes = Encoding.UTF8.GetBytes(str);
            var strLenBytes = BitConverter.GetBytes((ushort)strBytes.Length).Reverse();
            var reader = CreateReader(strLenBytes.Concat(strBytes).ToArray());
            Assert.AreEqual(str, reader.ReadString());
        }
    }
}