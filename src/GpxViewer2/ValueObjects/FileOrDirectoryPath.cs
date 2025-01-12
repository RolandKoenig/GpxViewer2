using System;

namespace GpxViewer2.ValueObjects;

/// <summary>
/// This ValueType ensures that more paths to the same file can not differ from each other.
/// </summary>
public readonly struct FileOrDirectoryPath(string path) : IEquatable<FileOrDirectoryPath>
{
    public static readonly FileOrDirectoryPath Empty = new();

    private readonly string? _path = ConvertForOsCasingBehavior(System.IO.Path.GetFullPath(path));

    public string Path => _path ?? string.Empty;

    /// <inheritdoc />
    public override string ToString()
    {
        return _path ?? nameof(FileOrDirectoryPath);
    }

    public bool Equals(FileOrDirectoryPath other)
    {
        return this.Path == other.Path;
    }

    private static string ConvertForOsCasingBehavior(string path)
    {
        if (OperatingSystem.IsWindows())
        {
            return path.ToLower();
        }
        return path;
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is FileOrDirectoryPath other && this.Equals(other);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return this.Path.GetHashCode();
    }

    public static bool operator ==(FileOrDirectoryPath left, FileOrDirectoryPath right)
    {
        return left.Path == right.Path;
    }

    public static bool operator !=(FileOrDirectoryPath left, FileOrDirectoryPath right)
    {
        return left.Path != right.Path;
    }
}
