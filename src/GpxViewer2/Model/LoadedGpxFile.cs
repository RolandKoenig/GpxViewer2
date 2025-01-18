using RolandK.Formats.Gpx;

namespace GpxViewer2.Model;

public class LoadedGpxFile
{
    public GpxFile RawGpxFile { get; }

    public string FileName { get; }

    public List<LoadedGpxFileTourInfo> Tours { get; }

    public List<LoadedGpxFileWaypointInfo> Waypoints { get; }

    public bool ContentsChanged { get; set; }

    public LoadedGpxFile(string fileName, GpxFile gpxFile)
    {
        this.FileName = fileName;
        this.RawGpxFile = gpxFile;

        this.Waypoints = new List<LoadedGpxFileWaypointInfo>(gpxFile.Waypoints.Count);
        foreach (var actRawWaypointInfo in gpxFile.Waypoints)
        {
            this.Waypoints.Add(new LoadedGpxFileWaypointInfo(this, actRawWaypointInfo));
        }

        this.Tours = new List<LoadedGpxFileTourInfo>();
        foreach (var actRawRouteInfo in gpxFile.Routes)
        {
            this.Tours.Add(new LoadedGpxFileTourInfo(this, actRawRouteInfo));
        }
        foreach (var actRawTrackData in gpxFile.Tracks)
        {
            this.Tours.Add(new LoadedGpxFileTourInfo(this, actRawTrackData));
        }
    }
}
