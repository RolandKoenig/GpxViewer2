using System.Linq;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using GpxViewer2.Controls;
using GpxViewer2.Messages;
using GpxViewer2.Model;
using GpxViewer2.Util;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using RolandK.Formats.Gpx;
using SkiaSharp;

namespace GpxViewer2.Views;

public partial class ElevationProfileViewModel : OwnViewModelBase, INavigationTarget
{
    public static readonly ElevationProfileViewModel EmptyViewModel = new();

    private LoadedGpxFileTourInfo? _selectedTour;

    [ObservableProperty]
    private ISeries[] _series = [];

    /// <inheritdoc />
    public string Title { get; } = "Navigation profile";

    /// <inheritdoc />
    public Control CreateViewInstance()
    {
        return new ElevationProfileView();
    }

    private void OnMessageReceived(GpxToursSelectedMessage message)
    {
        if (message.GpxTours.Count != 1)
        {
            this.Series = [];
            _selectedTour = null;
            return;
        }

        if (message.GpxTours[0] == _selectedTour)
        {
            return;
        }
        _selectedTour = message.GpxTours[0];

        GpxWaypoint? lastPoint = null;
        var actDistanceM = 0.0;
        this.Series = message.GpxTours[0].Segments
            .Select(segment => new LineSeries<ObservablePoint>()
            {
                Values = segment.Points.Select(actPoint =>
                {
                    if (lastPoint == null)
                    {
                        lastPoint = actPoint;
                        return new ObservablePoint(
                            actDistanceM / 1000.0,
                            actPoint.Elevation ?? 0.0);
                    }

                    actDistanceM += GeoCalculator.CalculateDistanceMeters(lastPoint, actPoint);
                    lastPoint = actPoint;

                    return new ObservablePoint(
                        actDistanceM / 1000.0,
                        actPoint.Elevation ?? 0.0);
                }).ToArray(),
                Fill = null,
                GeometrySize = 0,
                LineSmoothness = 0,
                Stroke = new SolidColorPaint(
                    SKColors.SteelBlue,
                    3),
                GeometryStroke = new SolidColorPaint(
                    SKColors.SteelBlue,
                    3)
            })
            .ToArray();
    }
}
