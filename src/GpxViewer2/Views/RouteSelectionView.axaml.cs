using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using GpxViewer2.Views.RouteSelection;
using RolandK.AvaloniaExtensions.Mvvm.Controls;

namespace GpxViewer2.Views;

public partial class RouteSelectionView : MvvmUserControl, IRouteSelectionViewService
{
    /// <inheritdoc />
    public event EventHandler? NodeSelectionChanged;

    public RouteSelectionView()
    {
        this.InitializeComponent();
    }

    private void OnTreeView_DoubleTabbed(object? sender, TappedEventArgs e)
    {
        if ((e.Source is not StyledElement styledElement) ||
            (styledElement.DataContext is not RouteSelectionNode node) ||
            (this.DataContext is not RouteSelectionViewModel viewModel))
        {
            return;
        }

        viewModel.NotifyDoubleTabbed(node);
    }

    /// <inheritdoc />
    public IReadOnlyList<RouteSelectionNode> GetSelectedNodes()
    {
        return this.CtrlNodeTree.SelectedItems
            .Cast<RouteSelectionNode>()
            .ToArray();
    }

    /// <inheritdoc />
    public void SetSelectedNodes(IReadOnlyList<RouteSelectionNode> nodes)
    {
        var newList = new AvaloniaList<RouteSelectionNode>(nodes);
        this.CtrlNodeTree.SelectedItems = newList;

        if (newList.Count > 0)
        {
            // Expand parent chain of given nodes
            foreach (var actNode in nodes)
            {
                if (actNode.ParentNode == null)
                { continue; }
                this.EnsureNodesOpened(actNode.ParentNode);
            }

            // Bring at least one node into view
            var firstItem = newList[0];
            Dispatcher.UIThread.Post(() =>
            {
                var treeItem = this.CtrlNodeTree.TreeContainerFromItem(newList[0]);
                treeItem?.BringIntoView();
            });
        }
    }

    private void EnsureNodesOpened(RouteSelectionNode node)
    {
        if (node.ParentNode != null)
        {
            this.EnsureNodesOpened(node.ParentNode);
        }

        var actTreeNode = this.CtrlNodeTree.TreeContainerFromItem(node) as TreeViewItem;
        if (actTreeNode != null)
        {
            actTreeNode.IsExpanded = true;
        }
    }

    private void OnCtrlNodeTree_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        this.NodeSelectionChanged?.Invoke(this, EventArgs.Empty);
    }
}
