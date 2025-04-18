﻿using GpxViewer2.Model;
using GpxViewer2.ValueObjects;
using RolandK.Formats.Gpx;

namespace GpxViewer2.Services.GpxFileStore
{
    internal class GpxFileRepositoryNodeFile : GpxFileRepositoryNode
    {
        private LoadedGpxFile? _gpxFile;
        private LoadedGpxFileTourInfo? _tour;
        private Exception? _fileLoadError;

        public FileOrDirectoryPath FilePath { get; }

        public override bool IsDirectory => false;

        /// <inheritdoc />
        public override FileOrDirectoryPath Source => this.FilePath;

        /// <inheritdoc />
        public override bool CanSave => true;

        /// <inheritdoc />
        public override bool HasError => _fileLoadError != null;

        public GpxFileRepositoryNodeFile(FileOrDirectoryPath filePath)
        {
            this.FilePath = filePath;

            try
            {
                _gpxFile = new LoadedGpxFile(
                    Path.GetFileName(filePath.Path),
                    GpxFile.Load(
                        filePath.Path,
                        GpxFileDeserializationMethod.Compatibility));
            }
            catch (Exception e)
            {
                _gpxFile = null;
                _fileLoadError = e;
            }

            this.InitializeProperties();
        }

        public GpxFileRepositoryNodeFile(string fileName, GpxFile gpxFile, FileOrDirectoryPath filePath)
        {
            this.FilePath = filePath;
            _gpxFile = new LoadedGpxFile(fileName, gpxFile);

            this.InitializeProperties();
        }

        private void InitializeProperties()
        {
            if (_gpxFile == null)
            {
                _tour = null;
                return;
            }

            switch (_gpxFile.Tours.Count)
            {
                case 1:
                    _tour = _gpxFile.Tours[0];
                    break;

                case > 1:
                    {
                        foreach (var actTour in _gpxFile.Tours)
                        {
                            var newChildNode = new GpxFileRepositoryNodeTour(_gpxFile, actTour);
                            newChildNode.Parent = this;
                            this.ChildNodes.Add(newChildNode);
                        }
                        break;
                    }
            }
        }

        /// <inheritdoc />
        protected override async ValueTask SaveThisNodesContentsAsync()
        {
            if (_gpxFile == null)
            {
                return;
            }

            await Task.Factory.StartNew(
                () => GpxFile.Save(_gpxFile.RawGpxFile, this.FilePath.Path));
            _gpxFile.ContentsChanged = false;
        }

        /// <inheritdoc />
        protected override bool HasThisNodesContentsChanged()
        {
            return _gpxFile?.ContentsChanged ?? false;
        }

        /// <inheritdoc />
        protected override string GetNodeText()
        {
            return Path.GetFileName(this.FilePath.Path);
        }

        /// <inheritdoc />
        public override Exception? GetErrorDetails()
        {
            return _fileLoadError;
        }

        /// <inheritdoc />
        public override LoadedGpxFile? GetAssociatedGpxFile()
        {
            return _gpxFile;
        }

        /// <inheritdoc />
        public override LoadedGpxFileTourInfo? GetAssociatedTour()
        {
            return _tour;
        }
    }
}
