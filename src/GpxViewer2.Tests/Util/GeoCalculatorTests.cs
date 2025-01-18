using GpxViewer2.Util;
using RolandK.Formats.Gpx;

namespace GpxViewer2.Tests.Util;

public class GeoCalculatorTests
{
    [Fact]
    public void CalculateDistanceMeters_ShouldReturnProperDistance_WhenTwoPointsAreGiven()
    {
        // Arrange
        var point1 = new GpxWaypoint // New York
        {
            Latitude = 40.7128,
            Longitude = -74.0060
        };
        var point2 = new GpxWaypoint // Los Angeles
        {
            Latitude = 34.0522,
            Longitude = -118.2437
        };

        // Assume we know the distance between these two cities
        var expectedDistance = 3940000;

        // Act
        var result = GeoCalculator.CalculateDistanceMeters(point1, point2);

        // Assert
        Assert.True(result.EqualsWithTolerance(expectedDistance, 50000)); // Accept a bit of deviation due to method accuracy
    }
}
