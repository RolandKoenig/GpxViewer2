using Avalonia;
using Avalonia.Controls;

namespace GpxViewer2.UiTestToolkit.Locators;

public class ByTextLocator(ILocator source, string text, LocatorOptions? options) : ILocator
{
    public IEnumerable<Visual> LocateAll()
    {
        foreach (var actVisual in LocatorUtil.QueryChildVisualsDeep(source.LocateAll(), options ?? LocatorOptions.Default))
        {
            if ((actVisual is TextBlock textBlock && textBlock.Text == text) ||
                (actVisual is TextBox textBox && textBox.Text == text))
            {
                yield return actVisual;
            }
        }
    }
}

public static class ByTextLocatorExtensions
{
    public static ILocator ThenByText(this ILocator locator, string text, LocatorOptions? options = null)
        => new ByTextLocator(locator, text, options);
    
    public static ILocator LocateByText(this Visual visual, string text, LocatorOptions? options = null)
        => new ByTextLocator(new ThisVisualLocator(visual), text, options);
}