using Avalonia.Metadata;

// Common XAML namespace (things like MarkupExtensions, Converters, etc.)
[assembly: XmlnsPrefix("local-application-common", "c")]
[assembly: XmlnsDefinition(
    "local-application-common",
    "GpxViewer2.MarkupExtensions")]
[assembly: XmlnsDefinition(
    "local-application-common",
    "GpxViewer2.Controls")]

// View XAML namespace (for Views / ViewModels only)
[assembly: XmlnsPrefix("local-application-views", "v")]
[assembly: XmlnsDefinition(
    "local-application-views",
    "GpxViewer2")]
[assembly: XmlnsDefinition(
    "local-application-views",
    "GpxViewer2.Views")]
[assembly: XmlnsDefinition(
    "local-application-views",
    "GpxViewer2.Views.RouteSelection")]