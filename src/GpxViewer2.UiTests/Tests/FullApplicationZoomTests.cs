using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using GpxViewer2.UiTestToolkit.Actions;
using GpxViewer2.UiTestToolkit.Locators;
using NSubstitute;

namespace GpxViewer2.UiTests.Tests;

public class FullApplicationZoomTests
{
    public FullApplicationZoomTests()
    {
        TestApp.Reset();
    }
    
    [AvaloniaFact]
    public async Task FullApplicationZoom_at_100_percent_by_default()
    {
        // Arrange
        TestApp.OsCheckerMock.IsOnMacOS().Returns(false);   
        
        var mainWindow = new MainWindow();
        mainWindow.Show();
        
        // Assert
        var mnuZoomLevel = await mainWindow
            .LocateByTestId("MnuZoomLevels", LocatorOptions.DefaultOnLogicalTree)
            .GetSingleAsync() as MenuItem;
        Assert.NotNull(mnuZoomLevel);
        Assert.Equal("Zoom (Current: 100%)", mnuZoomLevel.Header);
    }
}