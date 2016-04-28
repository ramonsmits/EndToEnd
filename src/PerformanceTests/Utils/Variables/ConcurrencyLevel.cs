namespace Variables
{
    public enum ConcurrencyLevel
    {
        Sequential = 0,
        EnvCores = 1,
        EnvCores2x = 2,
        EnvCores4x = 4,
        EnvCores8x = 8,
        EnvCores16x = 16,
        EnvCores32x = 32,
        EnvCores64x = 64,
    }
}