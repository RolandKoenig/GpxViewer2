using Avalonia.Headless.XUnit;
using GpxViewer2.UiTestToolkit.Actions;
using GpxViewer2.UiTestToolkit.Locators;
using NSubstitute;

namespace GpxViewer2.UiTests.Tests;

public class MainWindowTests
{
    public MainWindowTests()
    {
        TestApp.Reset();
    }
    
    [AvaloniaFact]
    public void MainWindow_has_correct_title()
    {
        // Arrange
        var mainWindow = new MainWindow();
        mainWindow.Show();
        
        // Assert
        Assert.NotNull(mainWindow.Title);
        Assert.StartsWith(mainWindow.Title, "RolandK GPXviewer 2");
    }

    [AvaloniaTheory]
    [InlineData(false, true)]
    [InlineData(true, false)]
    public async Task MainWindow_main_menu_bar_visibility(bool isOnMac, bool expectedVisibility)
    {
        // Arrange
        TestApp.OsCheckerMock.IsOnMacOS().Returns(isOnMac);
        
        var mainWindow = new MainWindow();
        mainWindow.Show();
        
        // Assert
        var mainMenu = await mainWindow
            .LocateByTestId("MainMenu", LocatorOptions.Default with { IncludeInvisible = true })
            .GetSingleAsync();
        Assert.Equal(expectedVisibility, mainMenu.IsVisible);
    }
}