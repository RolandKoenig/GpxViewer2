using GpxViewer2.Model;
using GpxViewer2.Util;
using GpxViewer2.ValueObjects;

namespace GpxViewer2.Services.GpxFileStore
{
    internal class GpxFileRepositoryNodeDirectory : GpxFileRepositoryNode
    {
        public FileOrDirectoryPath DirectoryPath { get; }

        /// <inheritdoc />
        public override FileOrDirectoryPath Source => this.DirectoryPath;

        public override bool IsDirectory => true;

        /// <inheritdoc />
        public override bool CanSave
        {
            get
            {
                return this.ChildNodes.Any(actChild => actChild.CanSave);
            }
        }

        public GpxFileRepositoryNodeDirectory(FileOrDirectoryPath directory)
        {
            this.DirectoryPath = directory;

            foreach (var actDirectory in Directory.GetDirectories(this.DirectoryPath.Path))
            {
                var childDirectory = new GpxFileRepositoryNodeDirectory(new FileOrDirectoryPath(actDirectory));
                childDirectory.Parent = this;
                this.ChildNodes.Add(childDirectory);
            }

            foreach (var actFilePath in Directory.GetFiles(this.DirectoryPath.Path))
            {
                var actFileExtension = Path.GetExtension(actFilePath);
                if (!actFileExtension.Equals(".gpx", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                var childFile = new GpxFileRepositoryNodeFile(new FileOrDirectoryPath(actFilePath));
                childFile.Parent = this;
                this.ChildNodes.Add(childFile);
            }

            this.ChildNodes.Sort((x, y) => x.NodeText.CompareTo(y.NodeText));
        }

        /// <inheritdoc />
        protected override bool HasThisNodesContentsChanged()
        {
            return false;
        }

        /// <inheritdoc />
        protected override string GetNodeText()
        {
            return FileOrDirectoryUtility.GetSourcePathDisplayNameForRecentlyOpened(this.DirectoryPath.Path);
        }

        /// <inheritdoc />
        public override LoadedGpxFile? GetAssociatedGpxFile()
        {
            return null;
        }

        /// <inheritdoc />
        public override LoadedGpxFileTourInfo? GetAssociatedTour()
        {
            return null;
        }
    }
}
