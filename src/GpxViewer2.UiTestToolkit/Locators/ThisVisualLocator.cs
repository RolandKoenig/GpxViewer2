using Avalonia;

namespace GpxViewer2.UiTestToolkit.Locators;

public class ThisVisualLocator : ILocator
{
    private readonly Visual _visual;
    
    public ThisVisualLocator(Visual visual)
    {
        _visual = visual;
    }
    
    public IEnumerable<Visual> LocateAll()
    {
        yield return _visual;
    }
}