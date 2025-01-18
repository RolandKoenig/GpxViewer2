using GpxViewer2.Messages;
using GpxViewer2.Services;
using GpxViewer2.Services.RecentlyOpened;
using GpxViewer2.ValueObjects;
using RolandK.AvaloniaExtensions.ViewServices;
using RolandK.InProcessMessaging;

namespace GpxViewer2.UseCases;

public class LoadGpxDirectoryUseCase(
    IGpxFileRepositoryService srvGpxFileRepository,
    IInProcessMessagePublisher srvMessagePublisher,
    IRecentlyOpenedService srvRecentlyOpened)
{
    public async Task LoadGpxDirectoryAsync(
        IOpenDirectoryViewService srvOpenDirectoryDialog)
    {
        var directoryToOpen = await srvOpenDirectoryDialog.ShowOpenDirectoryDialogAsync(
            "Load Directory");
        if (string.IsNullOrEmpty(directoryToOpen))
        {
            return;
        }

        await this.LoadGpxDirectoryAsync(directoryToOpen);
    }

    public async Task LoadGpxDirectoryAsync(
        string directoryToOpen)
    {
        var source = new FileOrDirectoryPath(directoryToOpen);

        var repositoryNode = srvGpxFileRepository.TryGetExistingNode(source);
        var newNodeLoaded = false;
        if (repositoryNode == null)
        {
            repositoryNode = srvGpxFileRepository.LoadDirectoryNode(source);
            newNodeLoaded = true;
        }

        var loadedGpxTours = repositoryNode
            .GetAssociatedToursDeep()
            .ToArray();

        await srvRecentlyOpened.AddOpenedAsync(
            directoryToOpen,
            RecentlyOpenedType.Directory);

        if (newNodeLoaded)
        {
            srvMessagePublisher.Publish(new GpxFileRepositoryNodesLoadedMessage([repositoryNode]));
        }
        srvMessagePublisher.Publish(new GpxToursSelectedMessage(loadedGpxTours));
        srvMessagePublisher.Publish(new ZoomToGpxToursRequestMessage(loadedGpxTours));
    }
}
