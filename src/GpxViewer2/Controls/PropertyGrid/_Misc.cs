namespace GpxViewer2.Controls.PropertyGrid;

public enum PropertyValueType
{
    Unsupported,

    Bool,

    String,

    Enum
}

public interface IPropertyContractResolver
{
    T? GetDataAnnotation<T>(Type targetType, string propertyName)
        where T : Attribute;

    IEnumerable<Attribute> GetDataAnnotations(Type targetType, string propertyName);
}
