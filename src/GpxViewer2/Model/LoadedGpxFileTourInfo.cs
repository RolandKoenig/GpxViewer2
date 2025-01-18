using GpxViewer2.Model.GpxXmlExtensions;
using GpxViewer2.Util;
using RolandK.Formats.Gpx;

namespace GpxViewer2.Model;

public class LoadedGpxFileTourInfo
{
    public LoadedGpxFile File { get; }

    public GpxTrack? RawTrackData { get; }
    public GpxRoute? RawRouteData { get; }
    public GpxTrackOrRoute RawTrackOrRoute { get; }

    public TourExtension RawTourExtensionData { get; }

    public List<LoadedGpxFileTourSegmentInfo> Segments { get; }

    public List<LoadedGpxFileWaypointInfo> Waypoints { get; }

    public double DistanceKm { get; private set; }

    public double ElevationUpMeters { get; private set; }

    public double ElevationDownMeters { get; private set; }

    public double ElevationMaxMeters { get; private set; }

    public double ElevationMinMeters { get; private set; }

    public int CountSegments { get; private set; }

    public int CountWaypointsWithinSegments { get; private set; }

    public LoadedGpxFileTourInfo(LoadedGpxFile file, GpxRoute rawRouteData)
    {
        this.File = file;
        this.RawRouteData = rawRouteData;
        this.RawTrackOrRoute = rawRouteData;

        rawRouteData.Extensions ??= new GpxExtensions();
        this.RawTourExtensionData = rawRouteData.Extensions.GetOrCreateExtension<RouteExtension>();

        this.Segments = new List<LoadedGpxFileTourSegmentInfo>(1);
        this.Segments.Add(new LoadedGpxFileTourSegmentInfo(
            this,
            rawRouteData));

        this.Waypoints = file.Waypoints;

        this.CalculateTourMetrics();
    }

    public LoadedGpxFileTourInfo(LoadedGpxFile file, GpxTrack rawTrackData)
    {
        this.File = file;
        this.RawTrackData = rawTrackData;
        this.RawTrackOrRoute = rawTrackData;

        rawTrackData.Extensions ??= new GpxExtensions();
        this.RawTourExtensionData = rawTrackData.Extensions.GetOrCreateExtension<TrackExtension>();

        this.Segments = new List<LoadedGpxFileTourSegmentInfo>(rawTrackData.Segments.Count);
        foreach (var actSegment in rawTrackData.Segments)
        {
            this.Segments.Add(new LoadedGpxFileTourSegmentInfo(
                this,
                actSegment));
        }

        this.Waypoints = file.Waypoints;

        this.CalculateTourMetrics();
    }

    public void CalculateTourMetrics()
    {
        var distanceMeters = 0.0;
        var elevationUpMeters = 0.0;
        var elevationDownMeters = 0.0;
        var elevationMaxMeters = double.MinValue;
        var elevationMinMeters = double.MaxValue;
        var segmentCount = 0;
        var waypointCount = 0;
        foreach (var actSegment in this.Segments)
        {
            if (actSegment.Points.Count <= 1)
            {
                continue;
            }
            segmentCount++;

            var lastPoint = actSegment.Points[0];
            foreach (var actPoint in actSegment.Points.GetRange(1, actSegment.Points.Count - 1))
            {
                waypointCount++;
                distanceMeters += GeoCalculator.CalculateDistanceMeters(
                    lastPoint, actPoint);

                if (actPoint.ElevationSpecified)
                {
                    var elevationAct = (double)actPoint.Elevation!;
                    if (elevationAct > elevationMaxMeters) { elevationMaxMeters = elevationAct; }
                    if (elevationAct < elevationMinMeters) { elevationMinMeters = elevationAct; }
                }

                if (lastPoint.ElevationSpecified && actPoint.ElevationSpecified)
                {
                    var elevationLast = (double)lastPoint.Elevation!;
                    var elevationAct = (double)actPoint.Elevation!;
                    // if (elevationLast.EqualsWithTolerance(elevationAct)) { }
                    if (elevationAct > elevationLast)
                    {
                        elevationUpMeters += (elevationAct - elevationLast);
                    }
                    else
                    {
                        elevationDownMeters += (elevationLast - elevationAct);
                    }
                }

                lastPoint = actPoint;
            }
        }

        
        this.DistanceKm = distanceMeters / 1000.0;
        this.ElevationUpMeters = elevationUpMeters;
        this.ElevationDownMeters = elevationDownMeters;
        this.ElevationMaxMeters = elevationMaxMeters.EqualsWithTolerance(double.MinValue, 0.001) ? 0.0 : elevationMaxMeters;
        this.ElevationMinMeters = elevationMinMeters.EqualsWithTolerance(double.MaxValue, 0.001) ? 0.0 : elevationMinMeters;
        this.CountSegments = segmentCount;
        this.CountWaypointsWithinSegments = waypointCount;
    }
}
