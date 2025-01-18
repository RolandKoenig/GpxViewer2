using GpxViewer2.Messages;
using GpxViewer2.Services.GpxFileStore;
using RolandK.InProcessMessaging;

namespace GpxViewer2.UseCases;

public class SaveNodeChangesUseCase(
    IInProcessMessagePublisher srvMessagePublisher)
{
    public async Task SaveChangesAsync(IReadOnlyList<GpxFileRepositoryNode> nodes)
    {
        var savedNodes = new List<GpxFileRepositoryNode>(nodes.Count);
        foreach (var actNode in nodes)
        {
            await foreach (var actSavedNode in actNode.SaveAsync())
            {
                savedNodes.Add(actSavedNode);
            }
        }

        var savedTours = savedNodes
            .SelectMany(x => x.GetAssociatedToursDeep())
            .Distinct()
            .ToArray();

        srvMessagePublisher.Publish(new TourConfigurationStateChangedMessage(savedTours, false));
    }
}
