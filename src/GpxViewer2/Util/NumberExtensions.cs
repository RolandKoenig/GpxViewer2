using System.Numerics;

namespace GpxViewer2.Util;

public static class NumberExtensions
{
    public static bool EqualsWithTolerance<T>(this T value1, T value2, T tolerance) 
        where T : INumber<T>
    {
        if (tolerance < T.Zero) { throw new ArgumentOutOfRangeException(nameof(tolerance), "Tolerance must be non-negative"); }
        
        T difference = T.Abs(value1 - value2);
        return difference <= tolerance;
    }
}
