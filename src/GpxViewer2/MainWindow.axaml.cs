using Avalonia.Controls;
using GpxViewer2.Controls;
using GpxViewer2.Services.RecentlyOpened;
using GpxViewer2.ViewServices;
using RolandK.AvaloniaExtensions.Mvvm;
using RolandK.AvaloniaExtensions.Mvvm.Controls;

namespace GpxViewer2;

public partial class MainWindow : MvvmWindow
{
    public MainWindow()
    {
        this.InitializeComponent();

        this.ViewServices.Add(new ServiceProviderViewService(this));
        this.ViewServices.Add(new ErrorReportingViewService(this));
    }

    private void UpdateMenuBars()
    {
        IReadOnlyList<RecentlyOpenedFileOrDirectoryModel> recentlyOpenedEntries = [];
        IReadOnlyList<FullApplicationZoomItem> zoomLevels = [];
        MainWindowViewModel? viewModel = null;
        if (this.DataContext is MainWindowViewModel vm)
        {
            viewModel = vm;
            recentlyOpenedEntries = viewModel.RecentlyOpenedEntries;
            zoomLevels = viewModel.FullApplicationZoomLevels;
        }

        // Update main menu
        this.MnuRecentlyOpened.Items.Clear();
        foreach (var actRecentlyOpenedEntry in recentlyOpenedEntries)
        {
            var actDisplayName = Path.GetFileName(actRecentlyOpenedEntry.FullPath);
            this.MnuRecentlyOpened.Items.Add(new MenuItem()
            {
                Header = actDisplayName,
                Command = viewModel?.LoadRecentlyOpenedCommand,
                CommandParameter = actRecentlyOpenedEntry
            });
        }
        this.MnuRecentlyOpened.IsEnabled = this.MnuRecentlyOpened.Items.Count > 0;
        
        this.MnuZoomLevels.Items.Clear();
        foreach (var actZoomLevel in zoomLevels)
        {
            this.MnuZoomLevels.Items.Add(new MenuItem()
            {
                Header = actZoomLevel.DisplayString,
                Command = viewModel?.SetFullApplicationZoomLevelCommand,
                CommandParameter = actZoomLevel
            });
        }

        // Update native menu
        var nativeMenuRecentlyOpened = FindTaggedItemInNativeMenu(NativeMenu.GetMenu(this)!, "RecentlyOpened");
        if ((nativeMenuRecentlyOpened != null) &&
            (nativeMenuRecentlyOpened.Menu != null))
        {
            var newChildNativeMenu = nativeMenuRecentlyOpened.Menu;
            newChildNativeMenu.Items.Clear();

            foreach (var actRecentlyOpenedEntry in recentlyOpenedEntries)
            {
                var actDisplayName = Path.GetFileName(actRecentlyOpenedEntry.FullPath);
                newChildNativeMenu.Items.Add(new NativeMenuItem()
                {
                    Header = actDisplayName,
                    Command = viewModel?.LoadRecentlyOpenedCommand,
                    CommandParameter = actRecentlyOpenedEntry
                });
            }
            nativeMenuRecentlyOpened.IsEnabled = newChildNativeMenu.Items.Count > 0;
        }
        
        var nativeMenuZoomLevels = FindTaggedItemInNativeMenu(NativeMenu.GetMenu(this)!, "ZoomLevels");
        if ((nativeMenuZoomLevels != null) &&
            (nativeMenuZoomLevels.Menu != null))
        {
            var newChildNativeMenu = nativeMenuZoomLevels.Menu;
            newChildNativeMenu.Items.Clear();

            foreach (var actZoomLevel in zoomLevels)
            {
                newChildNativeMenu.Items.Add(new NativeMenuItem()
                {
                    Header = actZoomLevel.DisplayString,
                    Command = viewModel?.SetFullApplicationZoomLevelCommand,
                    CommandParameter = actZoomLevel,
                    IsEnabled = viewModel?.CanSetFullApplicationZoomLevel(actZoomLevel) ?? false
                });
            }
        }
    }

    private static NativeMenuTaggedItem? FindTaggedItemInNativeMenu(NativeMenu menu, string tag)
    {
        foreach (var actItem in menu.Items)
        {
            if ((actItem is NativeMenuTaggedItem taggedItem) &&
               (taggedItem.Tag.Equals(tag, StringComparison.CurrentCultureIgnoreCase)))
            {
                return taggedItem;
            }

            if ((actItem is NativeMenuItem nativeMenuItem) &&
                (nativeMenuItem.Menu != null))
            {
                var childResult = FindTaggedItemInNativeMenu(nativeMenuItem.Menu, tag);
                if (childResult != null)
                {
                    return childResult;
                }
            }
        }

        return null;
    }

    protected override void OnViewModelAttached(ViewModelAttachedEventArgs args)
    {
        base.OnViewModelAttached(args);

        this.UpdateMenuBars();
    }

    protected override void OnViewModelPropertyChanged(ViewModelPropertyChangedEventArgs args)
    {
        base.OnViewModelPropertyChanged(args);

        switch (args.PropertyName)
        {
            case nameof(MainWindowViewModel.RecentlyOpenedEntries):
            case nameof(MainWindowViewModel.SetFullApplicationZoomLevelCommand):    
                this.UpdateMenuBars();
                break;
        }
    }

    /// <inheritdoc />
    protected override void OnClosing(WindowClosingEventArgs e)
    {
        base.OnClosing(e);

        if (this.DataContext is MainWindowViewModel viewModel)
        {
            var doContinue = viewModel.NotifyWindowClosing();
            if (!doContinue)
            {
                e.Cancel = true;
            }
        }
    }
}
