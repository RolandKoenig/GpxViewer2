using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Input;

namespace GpxViewer2.UiTestToolkit;

public static class InteractionExtensions
{
    public static void SimulateClick(this Visual visual)
    {
        var topLevel = TopLevel.GetTopLevel(visual);
        if (topLevel == null) { throw new LocateElementException("Unable to find TopLevel!"); }

        SimulateClick(visual, topLevel);
    }
    
    public static void SimulateClick(this Visual visual, TopLevel topLevel)
    {
        var middleOffset = new Point(
            visual.Bounds.Width / 2f,
            visual.Bounds.Height / 2f);
        
        var pointOnWindow = visual.TranslatePoint(middleOffset, topLevel);
        topLevel.MouseDown(pointOnWindow!.Value, MouseButton.Left);
        topLevel.MouseUp(pointOnWindow.Value, MouseButton.Left);
    }
}