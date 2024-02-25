using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Avalonia.Input;
using GpxViewer2.Model;
using GpxViewer2.Views.Maps;
using Mapsui;
using Mapsui.Animations;
using Mapsui.Layers;
using Mapsui.Nts;
using Mapsui.Tiling;
using RolandK.AvaloniaExtensions.Mvvm;
using RolandK.AvaloniaExtensions.Mvvm.Controls;
using RolandK.AvaloniaExtensions.ViewServices.Base;

namespace GpxViewer2.Views;

public partial class MapView : MvvmUserControl, IMapsViewService
{
    private static readonly TourVisualizationDetailLevel[] DETAIL_LEVELS = new[]
    {
        new TourVisualizationDetailLevel(10, 0),
        new TourVisualizationDetailLevel(50, 5),
        new TourVisualizationDetailLevel(100, 10),
        new TourVisualizationDetailLevel(200, 20),
        new TourVisualizationDetailLevel(double.MaxValue, 30)
    };

    private TourVisualizationDetailLevel? _lastDetailLevel;
    
    private MemoryLayer _lineStringLayerForAll;
    private MemoryLayer _lineStringLayerForSelection;
    
    private DateTimeOffset _lastPointerPressTimestamp = DateTimeOffset.MinValue;

    private MapViewModel? _attachedViewModel;
    
    /// <inheritdoc />
    public event EventHandler<RouteClickedEventArgs>? RouteClicked;
    
    /// <inheritdoc />
    public event EventHandler<ViewServiceRequestEventArgs>? ViewServiceRequest;
    
    public MapView()
    {
        this.InitializeComponent();
        
        _lineStringLayerForAll = new MemoryLayer();
        _lineStringLayerForAll.IsMapInfoLayer = true;
        _lineStringLayerForSelection = new MemoryLayer();
        
        this.CtrlMap.Map.Layers.Add(OpenStreetMap.CreateTileLayer());
        this.CtrlMap.Map.Layers.Add(_lineStringLayerForSelection);
        this.CtrlMap.Map.Layers.Add(_lineStringLayerForAll);
        this.CtrlMap.UnSnapRotationDegrees = 30;
        this.CtrlMap.ReSnapRotationDegrees = 5;
        
        this.CtrlMap.Map.Navigator.ViewportChanged += this.OnCtrlMap_Navigator_OnViewportChanged;

        this.ViewServices.Add(this);
    }

    private static TourVisualizationDetailLevel GetDetailLevel(double resolution)
    {
        foreach(var actDetailLevel in DETAIL_LEVELS)
        {
            if (resolution <= actDetailLevel.MaxResolution)
            {
                return actDetailLevel;
            }
        }

        return DETAIL_LEVELS.Last();
    }

    private int GetTourPointsToSkip()
    {
        return GetDetailLevel(this.CtrlMap.Map.Navigator.Viewport.Resolution).PointsToSkip;
    }

    /// <inheritdoc />
    public void AddAvailableGpxTours(IEnumerable<LoadedGpxFileTourInfo> newGpxTours)
    {
        _lineStringLayerForAll.Features = _lineStringLayerForAll.Features.Concat(
            newGpxTours
                .SelectMany(x => x.Segments)
                .Select(actSegment =>
                {
                    var result = new GeometryFeatureWithMetadata()
                    {
                        Geometry = actSegment.Points.GpxWaypointsToMapsuiGeometry(this.GetTourPointsToSkip()),
                        Styles = new[] { GpxRenderingHelper.CreateLineStringStyleForTour(actSegment.Tour) },
                        Tour = actSegment.Tour,
                        Points = actSegment.Points
                    };
                    return result;
                }))
            .ToArray();
        
        this.CtrlMap.RefreshGraphics();
    }

    /// <inheritdoc />
    public void RemoveAvailableGpxTours(IEnumerable<LoadedGpxFileTourInfo> existingGpxTours)
    {
        _lineStringLayerForAll.Features = _lineStringLayerForAll.Features
            .Where(x =>
            {
                if (x is not GeometryFeatureWithMetadata feature) { return true; }

                return !existingGpxTours.Contains(feature.Tour);
            })
            .ToArray();
        _lineStringLayerForSelection.Features = _lineStringLayerForAll.Features
            .Where(x =>
            {
                if (x is not GeometryFeatureWithMetadata feature) { return true; }

                return !existingGpxTours.Contains(feature.Tour);
            })
            .ToArray();
        
        this.CtrlMap.RefreshGraphics();
    }

    /// <inheritdoc />
    public void UpdateGpxTourVisualization()
    {
        var recreateLines = false;
        var currentDetaiLevel = GetDetailLevel(this.CtrlMap.Map.Navigator.Viewport.Resolution);
        if ((_lastDetailLevel == null) ||
            (_lastDetailLevel != currentDetaiLevel))
        {
            _lastDetailLevel = currentDetaiLevel;
            recreateLines = true;
        }
        
        foreach (var actFeature in _lineStringLayerForAll.Features)
        {
            if ((actFeature is not GeometryFeatureWithMetadata featureWithMetadata) ||
                (featureWithMetadata.Tour == null))
            {
                continue;
            }

            featureWithMetadata.Styles = new[]
            {
                GpxRenderingHelper.CreateLineStringStyleForTour(featureWithMetadata.Tour)
            };

            if (recreateLines)
            {
                featureWithMetadata.Geometry =
                    featureWithMetadata.Points.GpxWaypointsToMapsuiGeometry(this.GetTourPointsToSkip());
            }
        }

        if (recreateLines)
        {
            foreach (var actFeature in _lineStringLayerForSelection.Features)
            {
                if ((actFeature is not GeometryFeatureWithMetadata featureWithMetadata) ||
                    (featureWithMetadata.Tour == null))
                {
                    continue;
                }
                
                featureWithMetadata.Geometry =
                    featureWithMetadata.Points.GpxWaypointsToMapsuiGeometry(this.GetTourPointsToSkip());
            }
        }
    }

    /// <inheritdoc />
    public void SetSelectedGpxTours(IReadOnlyList<LoadedGpxFileTourInfo> selection)
    {
        if (selection.Count == 0)
        {
            _lineStringLayerForSelection.Features = Array.Empty<GeometryFeature>();
        }
        else
        {
            _lineStringLayerForSelection.Features = selection
                .SelectMany(x => x.Segments)
                .Select(actSegment =>
                {
                    return new GeometryFeatureWithMetadata()
                    {
                        Geometry = actSegment.Points.GpxWaypointsToMapsuiGeometry(this.GetTourPointsToSkip()),
                        Styles = new[] { GpxRenderingHelper.CreateLineStringStyle(GpxTourLineStringType.Selected) },
                        Tour = actSegment.Tour,
                        Points = actSegment.Points
                    };
                })
                .ToArray();
        }
        
        this.CtrlMap.RefreshGraphics();
    }

    private void OnCtrlMap_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        _lastPointerPressTimestamp = DateTimeOffset.UtcNow;
    }
    
    private void OnCtrlMap_PointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (DateTimeOffset.UtcNow - _lastPointerPressTimestamp > TimeSpan.FromMilliseconds(300)) { return; }

        if (e.InitialPressMouseButton == MouseButton.Right)
        {
            this.RouteClicked?.Invoke(this, new RouteClickedEventArgs(null));
            return;
        }

        if (e.InitialPressMouseButton == MouseButton.Left)
        {
            var mousePosition = e.GetCurrentPoint(this.CtrlMap);
        
            var clickInfo = this.CtrlMap.GetMapInfo(
                new MPoint(mousePosition.Position.X, mousePosition.Position.Y),
                3);
            if (clickInfo?.Feature is GeometryFeatureWithMetadata featureWithMetadata)
            {
                this.RouteClicked?.Invoke(this, new RouteClickedEventArgs(featureWithMetadata.Tour));
            }
            else
            {
                this.RouteClicked?.Invoke(this, new RouteClickedEventArgs(null));
            }
        }
    }

    private void OnCtrlMap_PointerMoved(object? sender, PointerEventArgs e)
    {
        var mousePosition = e.GetCurrentPoint(this.CtrlMap);
        
        var mouseLocationInfo = this.CtrlMap.GetMapInfo(
            new MPoint(mousePosition.Position.X, mousePosition.Position.Y),
            3);

        this.CtrlMap.Cursor = mouseLocationInfo?.Feature != null
            ? new Cursor(StandardCursorType.Hand)
            : Cursor.Default;
    }
    
    private void OnCtrlMap_Navigator_OnViewportChanged(object? sender, PropertyChangedEventArgs e)
    {
        this.UpdateGpxTourVisualization();
    }
    
    private void OnAttachedViewModel_ZoomToGpxToursRequest(object? sender, ZoomToGpxToursRequestEventArgs e)
    {
        var rectBuilder = new NavigationMRectBuilder();
        foreach (var actTour in e.Tours)
        {
            foreach (var actSegment in actTour.Segments)
            {
                var actFeature = new GeometryFeatureWithMetadata()
                {
                    Geometry = actSegment.Points.GpxWaypointsToMapsuiGeometry(this.GetTourPointsToSkip()),
                    Styles = new[] { GpxRenderingHelper.CreateLineStringStyle(GpxTourLineStringType.Selected) },
                    Tour = actSegment.Tour,
                    Points = actSegment.Points
                };
                rectBuilder.TryAddFeature(actFeature);
            }
        }

        if (rectBuilder.CanBuildBoundingBox)
        {
            this.CtrlMap.Map.Navigator.ZoomToBox(
                rectBuilder.TryBuild(),
                MBoxFit.Fit,
                500L,
                Easing.Linear);
        }
    }

    /// <inheritdoc />
    protected override void OnViewModelAttached(ViewModelAttachedEventArgs args)
    {
        base.OnViewModelAttached(args);

        if (_attachedViewModel != null)
        {
            _attachedViewModel.ZoomToGpxToursRequest -= this.OnAttachedViewModel_ZoomToGpxToursRequest;
            _attachedViewModel = null;
        }
        
        _attachedViewModel = args.ViewModel as MapViewModel;

        if (_attachedViewModel != null)
        {
            _attachedViewModel.ZoomToGpxToursRequest += this.OnAttachedViewModel_ZoomToGpxToursRequest;
        }
    }

    /// <inheritdoc />
    protected override void OnViewModelDetached(ViewModelDetachedEventArgs args)
    {
        base.OnViewModelDetached(args);
        
        if (_attachedViewModel != null)
        {
            _attachedViewModel.ZoomToGpxToursRequest -= this.OnAttachedViewModel_ZoomToGpxToursRequest;
            _attachedViewModel = null;
        }
    }
}