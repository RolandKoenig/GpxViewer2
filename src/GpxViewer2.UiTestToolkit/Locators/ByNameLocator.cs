using Avalonia;

namespace GpxViewer2.UiTestToolkit.Locators;

public class ByNameLocator(ILocator source, string name, LocatorOptions? options) : ILocator
{
    public IEnumerable<Visual> LocateAll()
    {
        foreach (var actVisual in LocatorUtil.QueryChildVisualsDeep(source.LocateAll(), options ?? LocatorOptions.Default))
        {
            if (actVisual.Name == name)
            {
                yield return actVisual;
            }
        }
    }
}

public static class ByNameLocatorExtensions
{
    public static ILocator ThenByName(this ILocator locator, string name, LocatorOptions? options = null)
        => new ByNameLocator(locator, name, options);
    
    public static ILocator LocateByName(this Visual visual, string name, LocatorOptions? options = null)
        => new ByNameLocator(new ThisVisualLocator(visual), name, options);
}