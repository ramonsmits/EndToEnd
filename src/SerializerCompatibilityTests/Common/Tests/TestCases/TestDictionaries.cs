namespace Common.Tests.TestCases
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using NUnit.Framework;
    using Types;

    public class TestDictionaries : TestCase
    {
        public override Type MessageType => typeof(MessageWithDictionaries);

        public override void Populate(object instance)
        {
            var dictionary = (MessageWithDictionaries)instance;

            dictionary.Bools = new Dictionary<bool, bool>
            {
                { true, true },
                { false, false }
            };
            dictionary.Chars = new Dictionary<char, char>
            {
                //{char.MinValue, char.MaxValue}, // doesn't work because we use UTF8
                { 'a', 'b' },
                { 'c', 'd' },
                { 'e', 'f' }
            };
            dictionary.Bytes = new Dictionary<byte, byte>
            {
                { byte.MinValue, byte.MaxValue },
                { 11, 1 },
                { 1, 0 }
            };
            dictionary.Ints = new Dictionary<int, int>
            {
                { int.MinValue, int.MaxValue },
                { 1, 2 },
                { 3, 4 },
                { 5, 6 }
            };
            dictionary.Decimals = new Dictionary<decimal, decimal>
            {
                { decimal.MinValue, decimal.MaxValue },
                { .2m, 4m },
                { .5m, .4234m }
            };
            dictionary.Doubles = new Dictionary<double, double>
            {
                //HINT: double.MinValue and double.MaxValue stored as keys in dictionary cannot be properly serialized by Json.Net
                { 0, double.MinValue },
                { 1, double.MaxValue },
                { .223d, 234d },
                { .513d, .4212334d }
            };
            dictionary.Floats = new Dictionary<float, float>
            {
                //HINT: float.MinValue and float.MaxValue stored as keys in dictioanry cannot be properly serialized by Json.Net
                { 0, float.MaxValue },
                { 1, float.MinValue },
                { .223f, 234f },
                { .513f, .4212334f }
            };
            dictionary.Enums = new Dictionary<DateTimeStyles, DateTimeKind>
            {
                { DateTimeStyles.AdjustToUniversal, DateTimeKind.Local },
                { DateTimeStyles.AllowLeadingWhite, DateTimeKind.Unspecified }
            };
            dictionary.Longs = new Dictionary<long, long>
            {
                { long.MaxValue, long.MinValue },
                { 34234, 234324 },
                { 45345345, 34534534565 }
            };
            dictionary.SBytes = new Dictionary<sbyte, sbyte>
            {
                { sbyte.MaxValue, sbyte.MaxValue },
                { 56, 13 }
            };
            dictionary.Shorts = new Dictionary<short, short>
            {
                { short.MinValue, short.MaxValue },
                { 5231, 6123 }
            };
            dictionary.Strings = new Dictionary<string, string>
            {
                { "Key1", "Value1" },
                { "Key2", "Value2" },
                { "Key3", "Value3" }
            };
            dictionary.UInts = new Dictionary<uint, uint>
            {
                { uint.MinValue, 23 },
                { uint.MaxValue, 34324 }
            };
            dictionary.ULongs = new Dictionary<ulong, ulong>
            {
                //HINT: unsigned ulong cannot be properly deserialized by Json.Net
                //      see: http://stackoverflow.com/questions/9355091/json-net-crashes-when-serializing-unsigned-integer-ulong-array
                //      looks like JSON format constraint
                //{ulong.MinValue, ulong.MaxValue},
                { 34324234, 3243243245 }
            };
            dictionary.UShorts = new Dictionary<ushort, ushort>
            {
                { ushort.MinValue, ushort.MaxValue },
                { 42324, 32 }
            };
        }

        public override void CheckIfAreEqual(object instanceA, object instanceB)
        {
            var expected = (MessageWithDictionaries)instanceA;
            var result = (MessageWithDictionaries)instanceB;

            CollectionAssert.AreEqual(expected.Bools, result.Bools);
            CollectionAssert.AreEqual(expected.Chars, result.Chars);
            CollectionAssert.AreEqual(expected.Bytes, result.Bytes);
            CollectionAssert.AreEqual(expected.Ints, result.Ints);
            CollectionAssert.AreEqual(expected.Decimals, result.Decimals);
            CollectionAssert.AreEqual(expected.Doubles, result.Doubles);
            CollectionAssert.AreEqual(expected.Floats, result.Floats);
            CollectionAssert.AreEqual(expected.Enums, result.Enums);
            CollectionAssert.AreEqual(expected.Longs, result.Longs);
            CollectionAssert.AreEqual(expected.SBytes, result.SBytes);
            CollectionAssert.AreEqual(expected.Shorts, result.Shorts);
            CollectionAssert.AreEqual(expected.Strings, result.Strings);
            CollectionAssert.AreEqual(expected.UInts, result.UInts);
            CollectionAssert.AreEqual(expected.ULongs, result.ULongs);
            CollectionAssert.AreEqual(expected.UShorts, result.UShorts);
        }
    }
}