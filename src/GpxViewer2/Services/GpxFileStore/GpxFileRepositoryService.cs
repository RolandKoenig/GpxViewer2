using GpxViewer2.ValueObjects;

namespace GpxViewer2.Services.GpxFileStore;

public class GpxFileRepositoryService : IGpxFileRepositoryService
{
    private List<GpxFileRepositoryNode> _loadedNodes = new();

    /// <inheritdoc />
    public IReadOnlyList<GpxFileRepositoryNode> GetAllLoadedNodes()
    {
        return _loadedNodes;
    }

    /// <summary>
    /// Gets the existing node based on the given <see cref="FileOrDirectoryPath"/>.
    /// Null is returned, if not loaded before.
    /// </summary>
    private static GpxFileRepositoryNode? TryGetExistingNode(FileOrDirectoryPath pathToSearch, IEnumerable<GpxFileRepositoryNode> nodes)
    {
        foreach (var actNode in nodes)
        {
            if (actNode.Source.Equals(pathToSearch))
            {
                return actNode;
            }

            var childNodeResult = TryGetExistingNode(pathToSearch, actNode.ChildNodes);
            if (childNodeResult != null)
            {
                return childNodeResult;
            }
        }

        return null;
    }

    /// <summary>
    /// Removes the given node. True is returned, when node was found and removed.
    /// </summary>
    private static bool RemoveNodeIfFound(GpxFileRepositoryNode node, IList<GpxFileRepositoryNode> nodeCollection)
    {
        for (var loop = 0; loop < nodeCollection.Count; loop++)
        {
            if (nodeCollection[loop] == node)
            {
                nodeCollection.RemoveAt(loop);
                return true;
            }

            var removedOnChildCollection = RemoveNodeIfFound(node, nodeCollection[loop].ChildNodes);
            if (removedOnChildCollection)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Checks whether there is an existing node loaded from given <see cref="FileOrDirectoryPath"/>
    /// </summary>
    private static bool ContainsExistingNode(FileOrDirectoryPath pathToSearch, IEnumerable<GpxFileRepositoryNode> nodes)
    {
        var existingNode = TryGetExistingNode(pathToSearch, nodes);
        return existingNode != null;
    }

    /// <inheritdoc />
    public GpxFileRepositoryNode? TryGetExistingNode(FileOrDirectoryPath fileOrDirectoryPath)
    {
        return TryGetExistingNode(fileOrDirectoryPath, _loadedNodes);
    }

    /// <inheritdoc />
    public GpxFileRepositoryNode LoadFileNode(FileOrDirectoryPath filePath)
    {
        if (ContainsExistingNode(filePath, _loadedNodes))
        {
            throw new InvalidOperationException($"File {filePath} already loaded!");
        }

        var gpxFileNode = new GpxFileRepositoryNodeFile(filePath);
        _loadedNodes.Add(gpxFileNode);
        return gpxFileNode;
    }

    /// <inheritdoc />
    public GpxFileRepositoryNode LoadDirectoryNode(FileOrDirectoryPath directoryPath)
    {
        if (ContainsExistingNode(directoryPath, _loadedNodes))
        {
            throw new InvalidOperationException($"Directory {directoryPath} already loaded!");
        }

        var gpxFileNode = new GpxFileRepositoryNodeDirectory(directoryPath);
        _loadedNodes.Add(gpxFileNode);
        return gpxFileNode;
    }

    /// <inheritdoc />
    public void RemoveNode(GpxFileRepositoryNode node)
    {
        RemoveNodeIfFound(node, _loadedNodes);
    }
}
