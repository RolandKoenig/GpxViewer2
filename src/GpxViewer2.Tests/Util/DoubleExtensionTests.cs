using GpxViewer2.Util;

namespace GpxViewer2.Tests.Util;

public class DoubleExtensionTests
{
    [Theory]
    [InlineData(10, 10, true)]
    [InlineData(0.12342462, 0.12392523, true)]
    [InlineData(0.1234, 0.1234, true)]
    [InlineData(0.1231, 0.123, true)]
    [InlineData(0.1230, 0.123, true)]
    [InlineData(0.1229, 0.122, true)]
    [InlineData(10, 9, false)]
    [InlineData(0.1231, 0.122, false)]
    [InlineData(0.1231, 0.121, false)]
    public void Equals3DigitPrecision_EqualityTests(double left, double right, bool expectedResult)
    {
        var result = left.EqualsWithTolerance(right, 0.001);

        Assert.Equal(expectedResult, result);
    }
}
