using GpxViewer2.Services.GpxFileStore;
using GpxViewer2.ValueObjects;

namespace GpxViewer2.Services;

public interface IGpxFileRepositoryService
{
    IReadOnlyList<GpxFileRepositoryNode> GetAllLoadedNodes();

    GpxFileRepositoryNode? TryGetExistingNode(FileOrDirectoryPath fileOrDirectoryPath);

    GpxFileRepositoryNode LoadFileNode(FileOrDirectoryPath filePath);

    GpxFileRepositoryNode LoadDirectoryNode(FileOrDirectoryPath directoryPath);

    void RemoveNode(GpxFileRepositoryNode node);
}
