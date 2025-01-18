namespace GpxViewer2.Views.RouteSelection;

public interface IRouteSelectionViewService
{
    event EventHandler NodeSelectionChanged;

    IReadOnlyList<RouteSelectionNode> GetSelectedNodes();

    void SetSelectedNodes(IReadOnlyList<RouteSelectionNode> nodes);
}
