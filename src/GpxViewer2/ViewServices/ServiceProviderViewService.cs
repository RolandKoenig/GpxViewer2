using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using RolandK.AvaloniaExtensions.DependencyInjection;
using RolandK.AvaloniaExtensions.ViewServices.Base;

namespace GpxViewer2.ViewServices;

public class ServiceProviderViewService(IResourceHost resourceHost) : ViewServiceBase, IServiceProviderViewService
{
    public void GetService<TService>(out TService service) where TService : notnull
    {
        var serviceProvider = resourceHost.GetServiceProvider();
        service = serviceProvider.GetRequiredService<TService>();
    }

    public IDisposable GetScopedUseCase<TUseCase>(out TUseCase useCase)
        where TUseCase : notnull
    {
        var serviceProvider = resourceHost.GetServiceProvider();
        var scope = serviceProvider.CreateScope();

        useCase = serviceProvider.GetRequiredService<TUseCase>();

        return scope;
    }

    public IDisposable GetScopedUseCase<TUseCase1, TUseCase2>(out TUseCase1 useCase1, out TUseCase2 useCase2)
        where TUseCase1 : notnull
        where TUseCase2 : notnull
    {
        var serviceProvider = resourceHost.GetServiceProvider();
        var scope = serviceProvider.CreateScope();

        useCase1 = serviceProvider.GetRequiredService<TUseCase1>();
        useCase2 = serviceProvider.GetRequiredService<TUseCase2>();

        return scope;
    }
}
