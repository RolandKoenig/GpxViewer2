using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GpxViewer2.Messages;
using GpxViewer2.Services;
using GpxViewer2.Services.RecentlyOpened;
using GpxViewer2.UseCases;
using GpxViewer2.Util;
using NSubstitute;
using RolandK.AvaloniaExtensions.ViewServices;

namespace GpxViewer2;

public partial class MainWindowViewModel : OwnViewModelBase
{
    public static readonly MainWindowViewModel EmptyViewModel = new(
        Substitute.For<IRecentlyOpenedService>(),
        Substitute.For<IGpxFileRepositoryService>(),
        Substitute.For<IStartupArgumentsContainer>());

    private readonly IRecentlyOpenedService _srvRecentlyOpened;
    private readonly IGpxFileRepositoryService _srvGpxFileRepository;
    private readonly IStartupArgumentsContainer _srvStartupArgumentsContainer;

    private bool _closeAllowed = false;
    private bool _isInitialLoad = true;

    [ObservableProperty]
    private IReadOnlyList<RecentlyOpenedFileOrDirectoryModel> _recentlyOpenedEntries = [];

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Title))]
    private bool _anyDataChanged = false;
    
    [ObservableProperty]
    private double _zoomLevel = 1.0;

    public string Title
    {
        get
        {
            var strBuilder = new StringBuilder(128);
            strBuilder.Append("RolandK GPXviewer 2");

            if (this.AnyDataChanged)
            {
                strBuilder.Append('*');
            }

            return strBuilder.ToString();
        }
    }

    public MainWindowViewModel(
        IRecentlyOpenedService srvRecentlyOpened,
        IGpxFileRepositoryService srvGpxFileRepository,
        IStartupArgumentsContainer srvStartupArgumentsContainer)
    {
        _srvRecentlyOpened = srvRecentlyOpened;
        _srvGpxFileRepository = srvGpxFileRepository;
        _srvStartupArgumentsContainer = srvStartupArgumentsContainer;
    }

    [RelayCommand]
    private async Task LoadFileAsync()
    {
        await this.WrapWithErrorHandlingAsync(async () =>
        {
            using var scope = this.GetScopedService(out LoadGpxFileUseCase useCase);
            await useCase.LoadGpxFileAsync(
                this.GetViewService<IOpenFileViewService>());
        });
    }

    [RelayCommand]
    private async Task LoadDirectoryAsync()
    {
        await this.WrapWithErrorHandlingAsync(async () =>
        {
            using var scope = this.GetScopedService(out LoadGpxDirectoryUseCase useCase);
            await useCase.LoadGpxDirectoryAsync(
                this.GetViewService<IOpenDirectoryViewService>());
        });
    }

    [RelayCommand]
    private async Task LoadRecentlyOpenedAsync(RecentlyOpenedFileOrDirectoryModel recentlyOpenedEntry)
    {
        switch (recentlyOpenedEntry.Type)
        {
            case RecentlyOpenedType.Directory:
                {
                    using var scope = this.GetScopedService(out LoadGpxDirectoryUseCase useCase);
                    await useCase.LoadGpxDirectoryAsync(recentlyOpenedEntry.FullPath);
                }
                break;

            case RecentlyOpenedType.File:
                {
                    using var scope = this.GetScopedService(out LoadGpxFileUseCase useCase);
                    await useCase.LoadGpxFileAsync(recentlyOpenedEntry.FullPath);
                }
                break;
        }
    }

    [RelayCommand]
    private async Task SaveAllAsync()
    {
        await this.WrapWithErrorHandlingAsync(async () =>
        {
            using var scope = this.GetScopedService(out SaveNodeChangesUseCase useCase);

            var allNodes = _srvGpxFileRepository.GetAllLoadedNodes();
            await useCase.SaveChangesAsync(allNodes);
        });
    }

    [RelayCommand]
    private void Exit()
    {
        this.CloseHostWindow();
    }

    public bool NotifyWindowClosing()
    {
        if (_closeAllowed)
        { return true; }

        var allNodes = _srvGpxFileRepository.GetAllLoadedNodes();
        var anyContentsChanged = allNodes.Any(x => x.ContentsChanged);
        if (!anyContentsChanged)
        {
            return true;
        }

        this.TriggerSaveBeforeExit();

        return false;
    }
    
    [RelayCommand]
    public void SetZoomLevel(double zoomLevel)
    {
        this.ZoomLevel = zoomLevel;
    }

    private async void TriggerSaveBeforeExit()
    {
        await this.WrapWithErrorHandlingAsync(async () =>
        {
            var srvMessageBox = base.GetViewService<IMessageBoxViewService>();
            var result = await srvMessageBox.ShowAsync(
                "Unsaved Changes",
                "There are unsaved changes. Do you want to save them before closing?",
                MessageBoxButtons.YesNoCancel);

            switch (result)
            {
                case MessageBoxResult.Yes:
                    await SaveAllAsync();

                    _closeAllowed = true;
                    this.CloseHostWindow();
                    break;

                case MessageBoxResult.No:
                    _closeAllowed = true;
                    this.CloseHostWindow();
                    break;

                case MessageBoxResult.Cancel:
                    return;
            }
        });
    }

    private async void OnMessageReceived(GpxFileRepositoryNodesLoadedMessage message)
    {
        await this.WrapWithErrorHandlingAsync(async () =>
        {
            this.RecentlyOpenedEntries =
                await _srvRecentlyOpened.GetAllRecentlyOpenedAsync();
        });
    }

    private void OnMessageReceived(TourConfigurationStateChangedMessage message)
    {
        if (message.DataChanged)
        {
            _closeAllowed = false;
        }

        var allNodes = _srvGpxFileRepository.GetAllLoadedNodes();
        this.AnyDataChanged = allNodes.Any(x => x.ContentsChanged);
    }

    /// <inheritdoc />
    protected override async void OnAssociatedViewChanged(object? associatedView)
    {
        base.OnAssociatedViewChanged(associatedView);

        if (associatedView != null)
        {
            await this.WrapWithErrorHandlingAsync(async () =>
            {
                this.RecentlyOpenedEntries =
                    await _srvRecentlyOpened.GetAllRecentlyOpenedAsync();
            });

            if ((!string.IsNullOrEmpty(_srvStartupArgumentsContainer.InitialFile)) &&
                (_isInitialLoad))
            {
                _isInitialLoad = false;
                await this.WrapWithErrorHandlingAsync(async () =>
                {
                    // Workaround: At this point, not all views are loaded
                    await Task.Delay(100);

                    using var scope = this.GetScopedService(out LoadGpxFileUseCase useCase);
                    await useCase.LoadGpxFileAsync(_srvStartupArgumentsContainer.InitialFile);
                });
            }
        }
    }
}
