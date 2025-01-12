using System.Web;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Platform;
using GpxViewer2.UseCases;
using Microsoft.Extensions.DependencyInjection;
using RolandK.AvaloniaExtensions.DependencyInjection;

namespace GpxViewer2;

public partial class App : Application
{
    public App()
    {
        var feature = this.TryGetFeature<IActivatableLifetime>();
        if (feature is not null)
        {
            feature.Activated += OnActivated;
        }
    }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow();
        }

        base.OnFrameworkInitializationCompleted();
    }

    private async void OnActivated(object? sender, ActivatedEventArgs e)
    {
        if ((e.Kind == ActivationKind.File) &&
            (e is FileActivatedEventArgs fileActivatedEventArgs))
        {
            if (fileActivatedEventArgs.Files.Count == 0)
            {
                return;
            }

            try
            {
                var fileUrl = fileActivatedEventArgs.Files.First().Path;
                var filePath = HttpUtility.UrlDecode(fileUrl.AbsolutePath);
                if (!File.Exists(filePath))
                {
                    return;
                }

                var serviceProvider = this.GetServiceProvider();
                using var scope = serviceProvider.CreateScope();

                var useCaseLoadFile = scope.ServiceProvider.GetRequiredService<LoadGpxFileUseCase>();
                await useCaseLoadFile.LoadGpxFileAsync(filePath);
            }
            catch
            {
                // TODO: Log error somewhere
            }
        }
    }
}
