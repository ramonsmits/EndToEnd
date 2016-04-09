using System;
using Variables;

static class ConcurrencyLevelConverter
{
    public static int Convert(ConcurrencyLevel value)
    {
        switch (value)
        {
            case ConcurrencyLevel.EnvCores:
                return Environment.ProcessorCount;
            case ConcurrencyLevel.EnvCores4x:
                return Environment.ProcessorCount * 4;
            case ConcurrencyLevel.Sequential:
                return 1;
            default:
                throw new NotSupportedException(value.ToString());
        }
    }
}
