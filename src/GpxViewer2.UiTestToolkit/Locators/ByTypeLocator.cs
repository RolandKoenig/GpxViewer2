using Avalonia;

namespace GpxViewer2.UiTestToolkit.Locators;

public class ByTypeLocator<T>(ILocator source, LocatorOptions? options) : ILocator
    where T : Visual
{
    public IEnumerable<Visual> LocateAll()
    {
        foreach (var actVisual in LocatorUtil.QueryChildVisualsDeep(source.LocateAll(), options ?? LocatorOptions.Default))
        {
            if (actVisual is T textBlock)
            {
                yield return actVisual;
            }
        }
    }
}

public static class ByTypeLocatorExtensions
{
    public static ILocator ThenByType<T>(this ILocator locator, LocatorOptions? options = null)
        where T : Visual
        => new ByTypeLocator<T>(locator, options);
    
    public static ILocator LocateByType<T>(this Visual visual, LocatorOptions? options = null)
        where T : Visual
        => new ByTypeLocator<T>(new ThisVisualLocator(visual), options);
}