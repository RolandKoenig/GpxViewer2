using GpxViewer2.Messages;
using GpxViewer2.Services;
using GpxViewer2.Services.GpxFileStore;
using RolandK.InProcessMessaging;

namespace GpxViewer2.UseCases;

public class CloseFileOrDirectoryUseCase(
    IGpxFileRepositoryService srvGpxFileRepository,
    IInProcessMessagePublisher srvMessagePublisher)
{
    public void CloseFileOrDirectories(IReadOnlyList<GpxFileRepositoryNode> nodes)
    {
        foreach (var actNode in nodes)
        {
            srvGpxFileRepository.RemoveNode(actNode);
            srvMessagePublisher.Publish(new GpxFileRepositoryNodesRemovedMessage([actNode]));
        }
    }
}
