using Avalonia.Markup.Xaml;

namespace GpxViewer2.MarkupExtensions;

// ReSharper disable once MemberCanBePrivate.Global

public class DoubleValue : MarkupExtension
{
    [ConstructorArgument("value")]
    public double Value { get; set; }

    public DoubleValue()
    {
        
    }

    public DoubleValue(double value)
    {
        this.Value = value;
    }
    
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return Value;
    }
}