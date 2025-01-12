using GpxViewer2.Model;
using Mapsui.Nts;
using RolandK.Formats.Gpx;

namespace GpxViewer2.Views.Maps;

public class GeometryFeatureWithMetadata : GeometryFeature
{
    public LoadedGpxFileTourInfo? Tour { get; set; }

    public required IReadOnlyList<GpxWaypoint> Points { get; set; }
}
