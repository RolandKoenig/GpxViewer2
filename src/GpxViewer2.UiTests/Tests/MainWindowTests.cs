using Avalonia.Headless.XUnit;

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

    [AvaloniaFact]
    public void MainWindow_main_menu_bar_visibility()
    {
        
    }
}