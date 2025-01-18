using Avalonia.Markup.Xaml;

namespace GpxViewer2.MarkupExtensions;

public class IsNonMacOsExtension : MarkupExtension
{
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return !OperatingSystem.IsMacOS();
    }
}
