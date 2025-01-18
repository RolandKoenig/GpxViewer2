using GpxViewer2.Model;
using RolandK.InProcessMessaging;

namespace GpxViewer2.Messages;

[InProcessMessage]
public record GpxToursSelectedMessage(
    IReadOnlyList<LoadedGpxFileTourInfo> GpxTours);
