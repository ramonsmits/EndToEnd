using System;
using System.Linq;

public class StringGenerator
{
    static Random random = new Random(0);
    static string availableCharacters = "abcdefghijhklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    public static string GenerateRandomString(int length = 0)
    {
        if (length == 0)
        {
            return string.Empty;
        }

        return new string(Enumerable.Repeat(availableCharacters, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}