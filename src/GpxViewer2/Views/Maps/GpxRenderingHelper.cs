using GpxViewer2.Model;
using GpxViewer2.Model.GpxXmlExtensions;
using Mapsui.Projections;
using Mapsui.Styles;
using NetTopologySuite.Geometries;
using RolandK.Formats.Gpx;

namespace GpxViewer2.Views.Maps;

#pragma warning disable CS8670 // Object or collection initializer implicitly dereferences possibly null member.

public static class GpxRenderingHelper
{
    private static readonly Dictionary<GpxTourLineStringType, IStyle> _lineStringStyles = new()
    {
        {
            GpxTourLineStringType.Default, new VectorStyle
            {
                Fill = null,
                Outline = null,
                Line =
                {
                    Color = Color.Gray,
                    Width = 4
                }
            }
        },
        {
            GpxTourLineStringType.Planned, new VectorStyle
            {
                Fill = null,
                Outline = null,
                Line =
                {
                    Color = Color.Yellow,
                    Width = 4
                }
            }
        },
        {
            GpxTourLineStringType.Succeeded, new VectorStyle
            {
                Fill = null,
                Outline = null,
                Line =
                {
                    Color = Color.Green,
                    Width = 4
                }
            }
        },
        {
            GpxTourLineStringType.Selected, new VectorStyle
            {
                Fill = null,
                Outline = null,
                Line =
                {
                    Color = Color.Blue,
                    Width = 6
                }
            }
        }
    };

    public static LineString? GpxWaypointsToMapsuiGeometry(
        this IReadOnlyList<GpxWaypoint> waypoints,
        int skipPointsCount)
    {
        var linePoints = new List<Coordinate>();
        // var pointCount = 0;
        for (var loop = 0; loop < waypoints.Count; loop++)
        {
            if ((loop < waypoints.Count - 1) &&
                (skipPointsCount != 0) &&
                (loop % skipPointsCount > 0))
            {
                continue;
            }

            var actPoint = waypoints[loop];

            var point = SphericalMercator.FromLonLat(actPoint.Longitude, actPoint.Latitude);
            linePoints.Add(new Coordinate(point.x, point.y));
        }

        if (linePoints.Count < 2)
        {
            return null;
        }

        return new LineString(linePoints.ToArray());
    }

    public static IStyle CreateLineStringStyle(GpxTourLineStringType lineStringType)
    {
        return _lineStringStyles[lineStringType];
    }

    public static IStyle CreateLineStringStyleForTour(LoadedGpxFileTourInfo tour)
    {
        var lineStringType = tour.RawTourExtensionData.State switch
        {
            GpxTrackState.Planned => GpxTourLineStringType.Planned,
            GpxTrackState.Succeeded => GpxTourLineStringType.Succeeded,
            _ => GpxTourLineStringType.Default
        };

        return _lineStringStyles[lineStringType];
    }
}
