using RolandK.AvaloniaExtensions.ViewServices.Base;

namespace GpxViewer2.ViewServices;

public interface IServiceProviderViewService : IViewService
{
    void GetService<TService>(out TService service)
        where TService : notnull;

    IDisposable GetScopedUseCase<TService>(out TService useCase)
        where TService : notnull;

    IDisposable GetScopedUseCase<TService1, TService2>(out TService1 useCase1, out TService2 useCase2)
        where TService1 : notnull
        where TService2 : notnull;
}
