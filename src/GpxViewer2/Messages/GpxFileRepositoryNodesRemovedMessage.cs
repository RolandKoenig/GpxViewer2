using GpxViewer2.Services.GpxFileStore;
using RolandK.InProcessMessaging;

namespace GpxViewer2.Messages;

[InProcessMessage]
public record GpxFileRepositoryNodesRemovedMessage(
    IReadOnlyList<GpxFileRepositoryNode> Nodes);
