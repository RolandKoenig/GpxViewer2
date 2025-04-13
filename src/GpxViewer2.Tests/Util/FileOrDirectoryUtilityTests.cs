using GpxViewer2.Util;

namespace GpxViewer2.Tests.Util;

public class FileOrDirectoryUtilityTests
{
    [Theory]
    [InlineData("/Users/rolandkoenig/Gpx/Wanderungen/Deutschland Allgemein/Thueringer Wald", "Thueringer Wald")]
    [InlineData("/Users/rolandkoenig/Gpx/Wanderungen/Deutschland Allgemein/Thueringer Wald/", "Thueringer Wald")]
    public void GetSourcePathDisplayNameForRecentlyOpened_ReturnsCorrectValue(string fullPath, string expectedDisplayName)
    {
        var displayName = FileOrDirectoryUtility.GetSourcePathDisplayNameForRecentlyOpened(fullPath);
        Assert.Equal(expectedDisplayName, displayName);
    }
}