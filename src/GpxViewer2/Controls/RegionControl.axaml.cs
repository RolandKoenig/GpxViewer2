using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;
using RolandK.AvaloniaExtensions.DependencyInjection;

namespace GpxViewer2.Controls;

public partial class RegionControl : UserControl
{
    public static readonly DirectProperty<RegionControl, Type?> ViewModelTypeProperty =
        AvaloniaProperty.RegisterDirect<RegionControl, Type?>(
            nameof(ViewModelType),
            x => x.ViewModelType,
            (x, y) => x.ViewModelType = y,
            defaultBindingMode: BindingMode.OneTime);

    public string? TitleText
    {
        get => this.CtrlTitle.Text;
        set => this.CtrlTitle.Text = value;
    }

    public Type? ViewModelType
    {
        get;
        set;
    }

    public RegionControl()
    {
        this.InitializeComponent();
    }

    private void ApplyTargetView()
    {
        if (this.ViewModelType == null)
        {
            return;
        }

        var serviceProvider = this.GetServiceProvider();
        var targetViewModel = serviceProvider.GetService(this.ViewModelType);
        if (targetViewModel is not INavigationTarget navigationTarget)
        {
            return;
        }

        var targetView = navigationTarget.CreateViewInstance();
        targetView.DataContext = navigationTarget;

        this.TitleText = navigationTarget.Title;
        this.CtrlContentControl.Content = targetView;
    }

    /// <inheritdoc />
    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        this.ApplyTargetView();
    }
}
