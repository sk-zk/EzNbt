using NUnit.Framework;
using EzNbt;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace EzNbtTests
{
    [TestFixture()]
    public class NbtReaderTests
    {
        [Test()]
        public void EverythingTest()
        {
            var nbt = NbtReader.Open("TestFiles/Everything.nbt");
            Assert.IsNotNull(nbt);

            Assert.IsTrue(nbt.ContainsKey("primitives"));
            Assert.AreEqual(-1, nbt["primitives"]["byte"]);
            Assert.AreEqual(3456, nbt["primitives"]["short"]);
            Assert.AreEqual(12345678, nbt["primitives"]["int"]);
            Assert.AreEqual(-98765432123456789, nbt["primitives"]["long"]);
            Assert.AreEqual(42.175f, nbt["primitives"]["float"]);
            Assert.AreEqual(4242.4242, nbt["primitives"]["double"]);
            Assert.AreEqual("test string", nbt["primitives"]["string"]);

            Assert.IsTrue(nbt.ContainsKey("arrays"));
            Assert.IsTrue(nbt["arrays"]["bytearray"][2] is sbyte b && b == -1);
            Assert.IsTrue(nbt["arrays"]["intarray"][2] is int i && i == 33333333);
            Assert.IsTrue(nbt["arrays"]["longarray"][2] is long l && l == 3333333333333333L);

            Assert.IsTrue(nbt.ContainsKey("lists"));
            Assert.AreEqual(-1234.56, nbt["lists"]["compoundlist"][1]["anotherlist"][1]);
            Assert.AreEqual(-99.9f, nbt["lists"]["listlist"][1][1] );
            Assert.AreEqual("ä ö ü Ä Ö Ü ß", nbt["lists"]["stringlist"][1]);
        }

        [Test()]
        public void ReadByteTagTest()
        {
            using var testFile = new FileStream("TestFiles/SingleTagByte.nbt", FileMode.Open);
            var reader = new BigEndianBinaryReader(testFile);
            var tag = NbtReader.ReadTag(reader);
            Assert.IsTrue(tag.Key == "byte");
            Assert.IsTrue(tag.Value is sbyte);
            Assert.IsTrue(tag.Value == (sbyte)-64);
        }

        [Test()]
        public void ReadDoubleTagTest()
        {
            using var testFile = new FileStream("TestFiles/SingleTagDouble.nbt", FileMode.Open);
            var reader = new BigEndianBinaryReader(testFile);
            var tag = NbtReader.ReadTag(reader);
            Assert.IsTrue(tag.Key == "double");
            Assert.IsTrue(tag.Value is double);
            Assert.IsTrue(tag.Value == 4242.4242);
        }

        [Test()]
        public void ReadStringTagTest()
        {
            using var testFile = new FileStream("TestFiles/SingleTagString.nbt", FileMode.Open);
            var reader = new BigEndianBinaryReader(testFile);
            var tag = NbtReader.ReadTag(reader);
            Assert.IsTrue(tag.Key == "string");
            Assert.IsTrue(tag.Value is string);
            Assert.IsTrue(tag.Value == "test string");
        }

        [Test()]
        public void ReadIntArrayTagTest()
        {
            using var testFile = new FileStream("TestFiles/SingleTagIntArray.nbt", FileMode.Open);
            var reader = new BigEndianBinaryReader(testFile);
            var tag = NbtReader.ReadTag(reader);
            Assert.IsTrue(tag.Key == "intarray");
            Assert.IsTrue(tag.Value is int[]);
            Assert.IsTrue(tag.Value[2] == 500);
        }

        [Test()]
        public void ReadIntListTagTest()
        {
            using var testFile = new FileStream("TestFiles/SingleTagIntList.nbt", FileMode.Open);
            var reader = new BigEndianBinaryReader(testFile);
            var tag = NbtReader.ReadTag(reader);
            Assert.IsTrue(tag.Key == "list");
            Assert.IsTrue(tag.Value[2] is int);
            Assert.IsTrue(tag.Value[2] == 500);
        }
    }
}