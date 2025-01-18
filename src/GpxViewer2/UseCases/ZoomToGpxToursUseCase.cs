using GpxViewer2.Messages;
using GpxViewer2.Model;
using RolandK.InProcessMessaging;

namespace GpxViewer2.UseCases;

public class ZoomToGpxToursUseCase(
    IInProcessMessagePublisher srvMessagePublisher)
{
    public void ZoomToGpxTours(
        IReadOnlyList<LoadedGpxFileTourInfo> gpxTours)
    {
        srvMessagePublisher.Publish(new ZoomToGpxToursRequestMessage(gpxTours));
    }
}
