using GpxViewer2.Services;
using Microsoft.Extensions.DependencyInjection;
using RolandK.AvaloniaExtensions.DependencyInjection.Markup;

namespace GpxViewer2.MarkupExtensions;

public class IsNonMacOsExtension : MarkupExtensionWithDependencyInjection
{
    protected override object ProvideDefaultValue(IServiceProvider xamlServiceProvider)
    {
        return !OperatingSystem.IsMacOS();
    }

    protected override object ProvideValue(IServiceProvider xamlServiceProvider, IServiceProvider appServiceProvider)
    {
        var osChecker = appServiceProvider.GetRequiredService<IOsChecker>();
        return !osChecker.IsOnMacOS();
    }
}
