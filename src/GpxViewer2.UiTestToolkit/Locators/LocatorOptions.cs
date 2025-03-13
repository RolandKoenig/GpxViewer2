namespace GpxViewer2.UiTestToolkit.Locators;

public record LocatorOptions(LocatorSearchTree SearchTree, bool IncludeInvisible)
{
    public static readonly LocatorOptions Default = new(
        LocatorSearchTree.VisualTree,
        false);
    
    public static readonly LocatorOptions DefaultOnLogicalTree 
        = Default with { SearchTree = LocatorSearchTree.LogicalTree };
}