namespace Common.Tests.TestCases.Types
{
    using System;

    [Serializable]
    public class TestMessageWithChar
    {
        public char InvalidCharacter { get; set; }
        public char ValidCharacter { get; set; }
    }
}