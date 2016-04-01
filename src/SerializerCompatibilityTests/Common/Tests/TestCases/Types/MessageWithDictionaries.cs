namespace Common.Tests.TestCases.Types
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    [Serializable]
    public class MessageWithDictionaries
    {
        public Dictionary<bool, bool> Bools { get; set; }
        public Dictionary<byte, byte> Bytes { get; set; }
        public Dictionary<char, char> Chars { get; set; }
        public Dictionary<decimal, decimal> Decimals { get; set; }
        public Dictionary<double, double> Doubles { get; set; }
        public Dictionary<DateTimeStyles, DateTimeKind> Enums { get; set; }
        public Dictionary<float, float> Floats { get; set; }
        public Dictionary<int, int> Ints { get; set; }
        public Dictionary<long, long> Longs { get; set; }
        public Dictionary<sbyte, sbyte> SBytes { get; set; }
        public Dictionary<short, short> Shorts { get; set; }
        public Dictionary<uint, uint> UInts { get; set; }
        public Dictionary<ulong, ulong> ULongs { get; set; }
        public Dictionary<ushort, ushort> UShorts { get; set; }
        public Dictionary<string, string> Strings { get; set; }
    }
}