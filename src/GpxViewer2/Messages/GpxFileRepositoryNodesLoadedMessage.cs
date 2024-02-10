using System.Collections.Generic;
using GpxViewer2.Services.GpxFileStore;
using RolandK.InProcessMessaging;

namespace GpxViewer2.Messages;

[InProcessMessage]
public record GpxFileRepositoryNodesLoadedMessage(
    IReadOnlyList<GpxFileRepositoryNode> Nodes);