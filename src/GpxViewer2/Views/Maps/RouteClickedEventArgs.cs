using GpxViewer2.Model;

namespace GpxViewer2.Views.Maps;

public class RouteClickedEventArgs(LoadedGpxFileTourInfo? clickedGpxTour) : EventArgs
{
    public LoadedGpxFileTourInfo? ClickedGpxTour { get; } = clickedGpxTour;
}
