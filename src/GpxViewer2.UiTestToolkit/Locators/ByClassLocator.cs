using Avalonia;

namespace GpxViewer2.UiTestToolkit.Locators;

public class ByClassLocator(ILocator source, string className, LocatorOptions? options) : ILocator
{
    public IEnumerable<Visual> LocateAll()
    {
        foreach (var actVisual in LocatorUtil.QueryChildVisualsDeep(source.LocateAll(), options ?? LocatorOptions.Default))
        {
            if (actVisual.Classes.Contains(className))
            {
                yield return actVisual;
            }
        }
    }
}

public static class ByClassLocatorExtensions
{
    public static ILocator ThenByClass(this ILocator locator, string className, LocatorOptions? options = null)
        => new ByClassLocator(locator, className, options);
    
    public static ILocator LocateByClass(this Visual visual, string className, LocatorOptions? options = null)
        => new ByClassLocator(new ThisVisualLocator(visual), className, options);
}