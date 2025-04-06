using Avalonia;
using Avalonia.Headless;
using GpxViewer2.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NSubstitute;
using NSubstitute.ClearExtensions;
using RolandK.AvaloniaExtensions.DependencyInjection;

[assembly: AvaloniaTestApplication(
    typeof(GpxViewer2.UiTests.TestApp))]

namespace GpxViewer2.UiTests;

public static class TestApp
{
    public static IOsChecker OsCheckerMock { get; } = Substitute.For<IOsChecker>();
    
    public static IRecentlyOpenedService RecentlyOpenedServiceMock { get; } = Substitute.For<IRecentlyOpenedService>();

    public static void Reset()
    {
        // Reset mocks
        OsCheckerMock.ClearSubstitute();
    }
    
    public static AppBuilder BuildAvaloniaApp() => Program.BuildAvaloniaApp()
        .UseDependencyInjection(services =>
        {
            Reset();
            
            services.Replace(ServiceDescriptor.Singleton(RecentlyOpenedServiceMock));
            services.Replace(ServiceDescriptor.Singleton(OsCheckerMock));
        })
        .UseHeadless(new AvaloniaHeadlessPlatformOptions());
}