using Avalonia;
using Avalonia.LogicalTree;
using Avalonia.VisualTree;

namespace GpxViewer2.UiTestToolkit.Locators;

public static class LocatorUtil
{
    public static IEnumerable<Visual> QueryChildVisualsDeep(ILogical rootNode, LocatorOptions options)
    {
        foreach (var actChildLogical in GetChildEnumerable(rootNode, options))
        {
            if (!DoesChildMatch(actChildLogical, options))
            {
                continue;
            }
            
            if (actChildLogical is Visual actChildVisual)
            {
                yield return actChildVisual;
            }
            
            foreach (var actInnerChildControl in QueryChildVisualsDeep(actChildLogical, options))
            {
                yield return actInnerChildControl;
            }
        }
    }
    
    public static IEnumerable<Visual> QueryChildVisualsDeep(IEnumerable<ILogical> rootNodes, LocatorOptions options)
    {
        foreach (var actRootNode in rootNodes)
        {
            foreach (var actVisual in QueryChildVisualsDeep(actRootNode, options))
            {
                yield return actVisual;
            }
        }
    }
    
    public static IEnumerable<Visual> QueryChildVisualsDeepContainingSelf(IEnumerable<ILogical> rootNodes, LocatorOptions options)
    {
        foreach (var actRootNode in rootNodes)
        {
            if (!DoesChildMatch(actRootNode, options))
            {
                continue;
            }
            
            foreach (var actVisual in QueryChildVisualsDeep(actRootNode, options))
            {
                yield return actVisual;
            }
        }
    }
    
    private static IEnumerable<ILogical> GetChildEnumerable(ILogical node, LocatorOptions options)
    {
        return options.SearchTree switch
        {
            LocatorSearchTree.VisualTree => node is Visual visual ? visual.GetVisualChildren() : Enumerable.Empty<ILogical>(),
            LocatorSearchTree.LogicalTree => node.GetLogicalChildren(),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    /// <summary>
    /// Helper: Does the given <see cref="Visual"/> match the given <see cref="LocatorOptions"/>
    /// </summary>
    private static bool DoesChildMatch(Visual visual, LocatorOptions options)
    {
        if ((!options.IncludeInvisible) &&
            (!visual.IsVisible))
        {
            return false;
        }

        return true;
    }
    
    /// <summary>
    /// Helper: Does the given <see cref="ILogical"/> match the given <see cref="LocatorOptions"/>
    /// </summary>
    private static bool DoesChildMatch(ILogical logical, LocatorOptions options)
    {
        if (logical is Visual visual)
        {
            return DoesChildMatch(visual, options);
        }

        return true;
    }
}