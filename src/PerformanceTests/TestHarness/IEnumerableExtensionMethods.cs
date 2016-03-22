using System;
using System.Collections.Generic;

public static class IEnumerableExtensionMethods
{
    public static double StandardDeviation(this IEnumerable<double> list)
    {
        var M = 0.0;
        var S = 0.0;
        var k = 1;
        foreach (var value in list)
        {
            var tmpM = M;
            M += (value - tmpM)/k;
            S += (value - tmpM)*(value - M);
            k++;
        }
        return Math.Sqrt(S/(k - 2));
    }
}
