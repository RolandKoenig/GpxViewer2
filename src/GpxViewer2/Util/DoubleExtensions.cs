using System;

namespace GpxViewer2.Util;

public static class DoubleExtensions
{
    public static bool EqualsWithTolerance(this double left, double right)
    {
        return Math.Abs(left - right) < 0.001;
    }
}
