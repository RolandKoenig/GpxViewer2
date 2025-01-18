using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using GpxViewer2.Controls.PropertyGrid;

namespace GpxViewer2.Controls;

public partial class PropertyGridControl : UserControl
{
    public static readonly DirectProperty<PropertyGridControl, object?> SelectedObjectProperty =
        AvaloniaProperty.RegisterDirect<PropertyGridControl, object?>(
            nameof(SelectedObject),
            x => x.SelectedObject,
            (x, y) => x.SelectedObject = y);

    public static readonly StyledProperty<PropertyGridEditControlFactory?> EditControlFactoryProperty =
        AvaloniaProperty.Register<PropertyGridControl, PropertyGridEditControlFactory?>(
            nameof(EditControlFactory),
            new PropertyGridEditControlFactory());

    public static readonly StyledProperty<IPropertyContractResolver?> PropertyContractResolverProperty =
        AvaloniaProperty.Register<PropertyGridControl, IPropertyContractResolver?>(
            nameof(PropertyContractResolver));

    private PropertyGridViewModel _propertyGridVM;
    private Control? _firstValueRowEditor;
    private object? _selectedObject;

    public object? SelectedObject
    {
        get => _selectedObject;
        set
        {
            var changed = _selectedObject != value;
            _selectedObject = value;

            if (changed)
            {
                this.OnSelectedObjectChanged();
            }
        }
    }

    public PropertyGridEditControlFactory? EditControlFactory
    {
        get => this.GetValue(EditControlFactoryProperty);
        set => this.SetValue(EditControlFactoryProperty, value);
    }

    public IPropertyContractResolver? PropertyContractResolver
    {
        get => this.GetValue(PropertyContractResolverProperty);
        set
        {
            this.SetValue(PropertyContractResolverProperty, value);
            _propertyGridVM.SetPropertyContractResolver(value);
        }
    }

    public PropertyGridControl()
    {
        this.InitializeComponent();

        _propertyGridVM = new PropertyGridViewModel();
        this.GridMain.DataContext = _propertyGridVM;
    }

    public void FocusFirstValueRowEditor()
    {
        // TODO: FocusManager.Instance removed?
        // FocusManager.Instance?.Focus(_firstValueRowEditor, NavigationMethod.Tab);
    }

    private void OnSelectedObjectChanged()
    {
        this._propertyGridVM.SelectedObject = this.SelectedObject;
        this.UpdatePropertiesView();
    }

    private void UpdatePropertiesView()
    {
        this.GridMain.Children.Clear();
        this.GridMain.RowDefinitions.Clear();

        var lstProperties = new List<ConfigurablePropertyRuntime>(_propertyGridVM.PropertyMetadata);
        lstProperties.Sort((left, right) =>
            string.Compare(left.Metadata.CategoryName, right.Metadata.CategoryName, StringComparison.Ordinal));
        var allPropertiesMetadata = lstProperties.Select(x => x.Metadata);

        // Create all controls
        var actRowIndex = 0;
        var actCategory = string.Empty;
        var editControlFactory = this.EditControlFactory;
        if (editControlFactory == null)
        { editControlFactory = new PropertyGridEditControlFactory(); }

        foreach (var actProperty in _propertyGridVM.PropertyMetadata)
        {
            // Create category rows
            if (actProperty.Metadata.CategoryName != actCategory)
            {
                this.GridMain.RowDefinitions.Add(new RowDefinition { Height = new GridLength(40) });

                actCategory = actProperty.Metadata.CategoryName;

                var txtHeader = new TextBlock
                {
                    Text = actCategory
                };

                txtHeader.SetValue(Grid.RowProperty, actRowIndex);
                txtHeader.SetValue(Grid.ColumnSpanProperty, 3);
                txtHeader.SetValue(Grid.ColumnProperty, 0);
                txtHeader.Margin = new Thickness(5d, 5d, 5d, 5d);
                txtHeader.VerticalAlignment = VerticalAlignment.Center;
                this.GridMain.Children.Add(txtHeader);

                var categorySeparator = new Separator();
                categorySeparator.VerticalAlignment = VerticalAlignment.Bottom;
                categorySeparator.Margin = new Thickness(5d, 5d, 0d, 5d);
                categorySeparator.SetValue(Grid.RowProperty, actRowIndex);
                categorySeparator.SetValue(Grid.ColumnSpanProperty, 3);
                categorySeparator.SetValue(Grid.ColumnProperty, 0);
                this.GridMain.Children.Add(categorySeparator);

                actRowIndex++;
            }

            // Create row header
            var ctrlTextContainer = new Border();
            var ctrlText = new TextBlock();
            ctrlText.Text = actProperty.Metadata.PropertyDisplayName;
            ctrlText.VerticalAlignment = VerticalAlignment.Center;
            ctrlText.Margin = new Thickness(5d, 0, 0, 0);
            SetToolTip(ctrlText, actProperty.Metadata.Description);
            ctrlTextContainer.Height = 35.0;
            ctrlTextContainer.Child = ctrlText;
            ctrlTextContainer.SetValue(Grid.RowProperty, actRowIndex);
            ctrlTextContainer.SetValue(Grid.ColumnProperty, 0);
            ctrlTextContainer.VerticalAlignment = VerticalAlignment.Top;
            this.GridMain.Children.Add(ctrlTextContainer);

            // Create and configure row editor
            var ctrlValueEdit = editControlFactory.CreateControl(actProperty.Metadata, nameof(actProperty.ValueAccessor), allPropertiesMetadata);
            this.GridMain.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
            ctrlValueEdit.VerticalAlignment = VerticalAlignment.Center;
            ctrlValueEdit.SetValue(Grid.RowProperty, actRowIndex);
            ctrlValueEdit.SetValue(Grid.ColumnProperty, 2);
            ctrlValueEdit.DataContext = actProperty;
            SetToolTip(ctrlValueEdit, actProperty.Metadata.Description);
            this.GridMain.Children.Add(ctrlValueEdit);

            _firstValueRowEditor ??= ctrlValueEdit;

            actRowIndex++;
        }

        if (this.GridMain.RowDefinitions.Count > 0)
        {
            var gridSplitter = new GridSplitter();
            gridSplitter.SetValue(Grid.ColumnProperty, 1);
            gridSplitter.SetValue(Grid.RowSpanProperty, this.GridMain.RowDefinitions.Count);
            gridSplitter.HorizontalAlignment = HorizontalAlignment.Stretch;
            gridSplitter.VerticalAlignment = VerticalAlignment.Stretch;
            gridSplitter.Background = Brushes.Transparent;
            this.GridMain.Children.Insert(0, gridSplitter);
        }
    }

    public static void SetToolTip(AvaloniaObject targetObject, string toolTip)
    {
        if (string.IsNullOrEmpty(toolTip))
        { return; }
        targetObject.SetValue(ToolTip.TipProperty, toolTip);
    }
}
