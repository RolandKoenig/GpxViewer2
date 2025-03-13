using Avalonia;
using Avalonia.Controls;

namespace GpxViewer2.UiTestToolkit.Locators;

public class BySelectionLocator(ILocator source, LocatorOptions? options) : ILocator
{
    public IEnumerable<Visual> LocateAll()
    {
        foreach (var actVisual in LocatorUtil.QueryChildVisualsDeep(source.LocateAll(), options ?? LocatorOptions.Default))
        {
            if ((actVisual is ListBoxItem { IsSelected: true}) ||
                (actVisual is MenuItem { IsSelected: true}) ||
                (actVisual is TabItem { IsSelected: true}) ||
                (actVisual is ComboBoxItem { IsSelected: true}) ||
                (actVisual is TreeViewItem { IsSelected: true}) ||
                (actVisual is DataGridRow { IsSelected: true}))
                // There may be many more...
            {
                yield return actVisual;
            }
        }
    }
}

public static class BySelectionLocatorExtensions
{
    public static ILocator ThenBySelection(this ILocator locator, LocatorOptions? options = null)
        => new BySelectionLocator(locator, options);
    
    public static ILocator LocateBySelection(this Visual visual, LocatorOptions? options = null)
        => new BySelectionLocator(new ThisVisualLocator(visual), options);
}