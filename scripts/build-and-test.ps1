# Common cleanup
dotnet clean "../GpxViewer2.core.slnf"

# Build and test
dotnet build -c Debug "../GpxViewer2.core.slnf"
dotnet test -c Debug "../GpxViewer2.core.slnf"