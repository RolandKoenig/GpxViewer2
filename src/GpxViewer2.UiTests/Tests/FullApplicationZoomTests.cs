using Avalonia.Headless.XUnit;

namespace GpxViewer2.UiTests.Tests;

public class FullApplicationZoomTests
{
    public FullApplicationZoomTests()
    {
        TestApp.Reset();
    }
    
    [AvaloniaFact]
    public void FullApplicationZoom_at_100_percent_by_default()
    {
        // Arrange
        var mainWindow = new MainWindow();
        mainWindow.Show();
        
        // Assert
        
    }
}