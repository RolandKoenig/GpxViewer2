using Avalonia;
using Avalonia.Data;
using Avalonia.Interactivity;

namespace GpxViewer2.UiTestToolkit;

// ReSharper disable once ClassNeverInstantiated.Global
public class TestProperties : AvaloniaObject
{
    public static readonly AttachedProperty<string> TestIdProperty =
        AvaloniaProperty.RegisterAttached<TestProperties, Interactive, string>(
            "TestId", string.Empty, false, BindingMode.OneTime);
    
    public static void SetTestId(AvaloniaObject element, string value) 
        => element.SetValue(TestIdProperty, value);
    
    public static string GetTestId(AvaloniaObject element) 
        => element.GetValue(TestIdProperty);
}