namespace GpxViewer2.Services.OsChecker;

public class DefaultOsChecker : IOsChecker
{
    public bool IsOnMacOS()
    {
        return OperatingSystem.IsMacOS();
    }
}