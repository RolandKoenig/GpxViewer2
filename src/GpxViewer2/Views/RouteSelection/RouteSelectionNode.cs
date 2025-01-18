using System.Collections.ObjectModel;
using System.ComponentModel;
using GpxViewer2.Model;
using GpxViewer2.Model.GpxXmlExtensions;
using GpxViewer2.Services.GpxFileStore;
using GpxViewer2.Util;

namespace GpxViewer2.Views.RouteSelection;

public class RouteSelectionNode : INotifyPropertyChanged
{
    /// <inheritdoc />
    public event PropertyChangedEventHandler? PropertyChanged;

    public RouteSelectionNode? ParentNode { get; }

    public ObservableCollection<RouteSelectionNode> ChildNodes { get; } = new();

    public LoadedGpxFileTourInfo? AssociatedTour { get; }

    public GpxFileRepositoryNode Node { get; }

    public bool HasAssociatedTour => AssociatedTour != null;

    public bool IsTourFinishedVisible
        => this.AssociatedTour?.RawTourExtensionData.State == GpxTrackState.Succeeded;

    public bool IsTourPlannedVisible
        => this.AssociatedTour?.RawTourExtensionData.State == GpxTrackState.Planned;
    
    public bool IsTopTourVisible
        => this.AssociatedTour?.RawTourExtensionData.IsTopTour == true;

    public bool IsDirectory => this.Node.IsDirectory;

    public double DistanceKm
        => this.AssociatedTour?.DistanceKm ?? 0.0;

    public double ElevationUpMeters
        => this.AssociatedTour?.ElevationUpMeters ?? 0.0;

    public double ElevationDownMeters
        => this.AssociatedTour?.ElevationDownMeters ?? 0.0;

    public string NodeText => this.Node.NodeText;

    public string TooltipText
    {
        get
        {
            using var scope = PooledStringBuilders.Current.UseStringBuilder(out var stringBuilder);

            if ((this.AssociatedTour != null) &&
                (this.AssociatedTour.RawTourExtensionData.IsTopTour))
            {
                stringBuilder.Append("TOP! ");
            }
            
            stringBuilder.Append(this.Node.NodeText);
            
            if (this.AssociatedTour != null)
            {
                stringBuilder.Append(", ");
                stringBuilder.Append(this.AssociatedTour.DistanceKm.ToString("N1"));
                stringBuilder.Append(" km, ");
                stringBuilder.Append(this.AssociatedTour.ElevationUpMeters.ToString("N0"));
                stringBuilder.Append(" m up, ");
                stringBuilder.Append(this.AssociatedTour.ElevationDownMeters.ToString("N0"));
                stringBuilder.Append(" m down");
            }

            return stringBuilder.ToString();
        }
    }

    public RouteSelectionNode(GpxFileRepositoryNode node, RouteSelectionNode? parentNode)
    {
        this.Node = node;
        this.AssociatedTour = node.GetAssociatedTour();

        foreach (var actChildNode in node.ChildNodes)
        {
            this.ChildNodes.Add(new RouteSelectionNode(actChildNode, this));
        }

        this.ParentNode = parentNode;
    }

    public static IEnumerable<RouteSelectionNode> LoopOverAllDeep(IEnumerable<RouteSelectionNode> nodes)
    {
        foreach (var actNode in nodes)
        {
            yield return actNode;

            foreach (var actNodeInner in LoopOverAllDeep(actNode.ChildNodes))
            {
                yield return actNodeInner;
            }
        }
    }

    public void RaiseNodePropertiesChanged()
    {
        this.PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(nameof(this.NodeText)));
        this.PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(nameof(this.TooltipText)));
        this.PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(nameof(this.IsTopTourVisible)));
        this.PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(nameof(this.IsTourFinishedVisible)));
        this.PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(nameof(this.IsTourPlannedVisible)));
    }
}
