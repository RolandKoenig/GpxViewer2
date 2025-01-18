using GpxViewer2.Messages;
using GpxViewer2.Services;
using GpxViewer2.Services.GpxFileStore;
using GpxViewer2.Services.RecentlyOpened;
using GpxViewer2.ValueObjects;
using RolandK.AvaloniaExtensions.ViewServices;
using RolandK.InProcessMessaging;

namespace GpxViewer2.UseCases;

public class LoadGpxFileUseCase(
    IGpxFileRepositoryService srvGpxFileRepository,
    IInProcessMessagePublisher srvMessagePublisher,
    IRecentlyOpenedService srvRecentlyOpened)
{
    public async Task LoadGpxFileAsync(
        IOpenFileViewService srvOpenFileDialog)
    {
        var filesToOpen = await srvOpenFileDialog.ShowOpenMultipleFilesDialogAsync(
            [new FileDialogFilter("GPX-Files (*.gpx)", ["*.gpx"])],
            "Load GPX-File");
        if (filesToOpen is null or [])
        {
            return;
        }

        await this.LoadGpxFileAsync(filesToOpen);
    }

    public async Task LoadGpxFileAsync(
        params string[] filesToOpen)
    {
        var allNodes = new List<GpxFileRepositoryNode>(filesToOpen.Length);
        var newNodes = new List<GpxFileRepositoryNode>(filesToOpen.Length);
        var openedPaths = new List<string>(filesToOpen.Length);
        foreach (var actFileToOpen in filesToOpen)
        {
            var source = new FileOrDirectoryPath(actFileToOpen);

            var repositoryNode = srvGpxFileRepository.TryGetExistingNode(source);
            if (repositoryNode == null)
            {
                repositoryNode = srvGpxFileRepository.LoadFileNode(source);
                newNodes.Add(repositoryNode);
                openedPaths.Add(actFileToOpen);
            }
            allNodes.Add(repositoryNode);
        }

        if (openedPaths.Count > 0)
        {
            await srvRecentlyOpened.AddOpenedAsync(openedPaths, RecentlyOpenedType.File);
        }

        var loadedGpxTours = allNodes
            .SelectMany(x => x.GetAssociatedToursDeep())
            .Distinct()
            .ToArray();

        if (newNodes.Count > 0)
        {
            srvMessagePublisher.Publish(new GpxFileRepositoryNodesLoadedMessage(newNodes));
        }
        srvMessagePublisher.Publish(new GpxToursSelectedMessage(loadedGpxTours));
        srvMessagePublisher.Publish(new ZoomToGpxToursRequestMessage(loadedGpxTours));
    }
}
