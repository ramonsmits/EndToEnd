namespace Common.Tests.TestCases.Types
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    [Serializable]
    public class MessageWithLists
    {
        public List<bool> Bools { get; set; }
        public List<byte> Bytes { get; set; }
        public List<char> Chars { get; set; }
        public List<decimal> Decimals { get; set; }
        public List<double> Doubles { get; set; }
        public List<DateTimeStyles> Enums { get; set; }
        public List<float> Floats { get; set; }
        public List<int> Ints { get; set; }
        public List<long> Longs { get; set; }
        public List<sbyte> SBytes { get; set; }
        public List<short> Shorts { get; set; }
        public List<uint> UInts { get; set; }
        public List<ulong> ULongs { get; set; }
        public List<ushort> UShorts { get; set; }
        public List<string> Strings { get; set; }
    }
}