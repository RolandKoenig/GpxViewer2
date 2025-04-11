using Avalonia;
using AvaloniaUI.DiagnosticsSupport;
using GpxViewer2.Model.GpxXmlExtensions;
using GpxViewer2.Services;
using GpxViewer2.Services.GpxFileStore;
using GpxViewer2.Services.OsChecker;
using GpxViewer2.Services.RecentlyOpened;
using GpxViewer2.Services.StartupArguments;
using GpxViewer2.UseCases;
using GpxViewer2.Views;
using Microsoft.Extensions.DependencyInjection;
using Projektanker.Icons.Avalonia;
using Projektanker.Icons.Avalonia.FontAwesome;
using RolandK.AvaloniaExtensions.DependencyInjection;
using RolandK.AvaloniaExtensions.ExceptionHandling;
using RolandK.Formats.Gpx;
using RolandK.InProcessMessaging;

namespace GpxViewer2;

public static class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static int Main(string[] args)
    {
        try
        {
            BuildAvaloniaApp(args)
                .StartWithClassicDesktopLifetime(args);
            return 0;
        }
        catch (Exception ex)
        {
            GlobalErrorReporting.TryShowBlockingGlobalExceptionDialogInAnotherProcess(
                ex,
                ".RKGpxViewer2",
                "GpxViewer2.ExceptionViewer");
            throw;
        }
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    // ReSharper disable once MemberCanBePrivate.Global
    public static AppBuilder BuildAvaloniaApp(params string[] args)
    {
        IconProvider.Current
            .Register<FontAwesomeIconProvider>();

        // Register GpxFile extensions
        GpxFile.RegisterExtensionType(typeof(TrackExtension));
        GpxFile.RegisterExtensionType(typeof(RouteExtension));
        GpxFile.RegisterNamespace("rkgpxv", "http://gpxviewer.rolandk.net/");

        return AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace()
            .WithDeveloperTools()
            .UseDependencyInjection(services =>
            {
                // Services
                var inProcessMessenger = new InProcessMessenger();
                services.AddSingleton<IInProcessMessageSubscriber>(_ => inProcessMessenger);
                services.AddSingleton<IInProcessMessagePublisher>(_ => inProcessMessenger);
                services.AddSingleton<IRecentlyOpenedService>(
                    _ => new RecentlyOpenedService(".RKGpxViewer2", 15));
                services.AddSingleton<IGpxFileRepositoryService, GpxFileRepositoryService>();
                services.AddSingleton<IOsChecker, DefaultOsChecker>();
                services.AddSingleton<IStartupArgumentsContainer>(_ => new StartupArgumentsContainer(args));
                
                // ViewModels
                services.AddTransient<MainWindowViewModel>();
                services.AddTransient<MapViewModel>();
                services.AddTransient<RouteDetailViewModel>();
                services.AddTransient<RouteSelectionViewModel>();
                services.AddTransient<ElevationProfileViewModel>();

                // UseCases
                services.AddScoped<CloseFileOrDirectoryUseCase>();
                services.AddScoped<LoadGpxFileUseCase>();
                services.AddScoped<LoadGpxDirectoryUseCase>();
                services.AddScoped<SaveNodeChangesUseCase>();
                services.AddScoped<SelectGpxToursUseCase>();
                services.AddScoped<UpdateTourPropertyUseCase>();
                services.AddScoped<ZoomToGpxToursUseCase>();
            });
    }
}
