using GpxViewer2.ValueObjects;

namespace GpxViewer2.Tests.ValueObjects;

public class FileOrDirectoryPathTests
{
    [Fact]
    public void Equals_WhenCalledWithSamePath_ReturnsTrue()
    {
        var filePath1 = new FileOrDirectoryPath("/path/to/file1.txt");
        var filePath2 = new FileOrDirectoryPath("/path/to/file1.txt");

        Assert.Equal(filePath1, filePath2);
    }

    [Fact]
    public void Equals_WhenCalledWithDifferentPaths_ReturnsFalse()
    {
        var filePath1 = new FileOrDirectoryPath("/path/to/file1.txt");
        var filePath2 = new FileOrDirectoryPath("/path/to/file2.txt");

        Assert.NotEqual(filePath1, filePath2);
    }

    [Fact]
    public void GetHashCode_ReturnsSameHashCodeForSamePaths()
    {
        var filePath1 = new FileOrDirectoryPath("/path/to/file1.txt");
        var filePath2 = new FileOrDirectoryPath("/path/to/file1.txt");

        Assert.Equal(filePath1.GetHashCode(), filePath2.GetHashCode());
    }

    [Fact]
    public void GetHashCode_ReturnsDifferentHashCodesForDifferentPaths()
    {
        var filePath1 = new FileOrDirectoryPath("/path/to/file1.txt");
        var filePath2 = new FileOrDirectoryPath("/path/to/file2.txt");

        Assert.NotEqual(filePath1.GetHashCode(), filePath2.GetHashCode());
    }

    [Fact]
    public void ToString_ReturnsCorrectlyFormattedString()
    {
        var filePath = new FileOrDirectoryPath("/path/to/file.txt");
        var fullPath = Path.GetFullPath("/path/to/file.txt");

        Assert.Equal(
            fullPath, 
            filePath.ToString(), 
            ignoreCase: OperatingSystem.IsWindows());
    }

    [Fact]
    public void ToString_ReturnsAbsolutePath()
    {
        var filePath = new FileOrDirectoryPath("file.txt");

        Assert.NotEqual("file.txt", filePath.ToString());
        Assert.EndsWith("file.txt", filePath.ToString());
    }

    [Fact]
    public void OperatorEqual_ReturnsTrueForSamePaths()
    {
        var filePath1 = new FileOrDirectoryPath("/path/to/file1.txt");
        var filePath2 = new FileOrDirectoryPath("/path/to/file1.txt");

        Assert.True(filePath1 == filePath2);
    }

    [Fact]
    public void OperatorEqual_ReturnsFalseForDifferentPaths()
    {
        var filePath1 = new FileOrDirectoryPath("/path/to/file1.txt");
        var filePath2 = new FileOrDirectoryPath("/path/to/file2.txt");

        Assert.False(filePath1 == filePath2);
    }

    [Fact]
    public void OperatorNotEqual_ReturnsFalseForSamePaths()
    {
        var filePath1 = new FileOrDirectoryPath("/path/to/file1.txt");
        var filePath2 = new FileOrDirectoryPath("/path/to/file1.txt");

        Assert.False(filePath1 != filePath2);
    }

    [Fact]
    public void OperatorNotEqual_ReturnsTrueForDifferentPaths()
    {
        var filePath1 = new FileOrDirectoryPath("/path/to/file1.txt");
        var filePath2 = new FileOrDirectoryPath("/path/to/file2.txt");

        Assert.True(filePath1 != filePath2);
    }
}
