# Common cleanup
dotnet clean "../GpxViewer2.sln"

# Build and test
dotnet build -c Debug "../GpxViewer2.sln"
dotnet test -c Debug "../GpxViewer2.sln"