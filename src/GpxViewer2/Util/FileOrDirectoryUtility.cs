namespace GpxViewer2.Util;

public static class FileOrDirectoryUtility
{
    public static string GetSourcePathDisplayNameForRecentlyOpened(string fullPath)
    {
        var fullPathSpan = fullPath.AsSpan();
        if ((fullPathSpan.Length > 1) && (fullPathSpan.EndsWith('/') || fullPathSpan.EndsWith('\\')))
        {
            fullPathSpan = fullPathSpan[..^1];
        }
        
        return Path.GetFileName(fullPathSpan).ToString();
    }
}