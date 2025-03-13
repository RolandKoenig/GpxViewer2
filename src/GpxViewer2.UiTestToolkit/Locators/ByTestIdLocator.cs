using Avalonia;

namespace GpxViewer2.UiTestToolkit.Locators;

public class ByTestIdLocator(ILocator source, string testId, LocatorOptions? options) : ILocator
{
    public IEnumerable<Visual> LocateAll()
    {
        foreach (var actVisual in LocatorUtil.QueryChildVisualsDeep(source.LocateAll(), options ?? LocatorOptions.Default))
        {
            if (actVisual.GetValue(TestProperties.TestIdProperty) == testId)
            {
                yield return actVisual;
            }
        }
    }
}

public static class ByTestIdLocatorExtensions
{
    public static ILocator ThenByTestId(this ILocator locator, string testId, LocatorOptions? options = null)
        => new ByTestIdLocator(locator, testId, options);
    
    public static ILocator LocateByTestId(this Visual visual, string testId, LocatorOptions? options = null)
        => new ByTestIdLocator(new ThisVisualLocator(visual), testId, options);
}