# Cleanup previous contents of publish directory
Remove-Item ../publish -Recurse -Force
mkdir ../publish

# Create macOS app package
dotnet publish -c Release -r osx-arm64 --self-contained -o ../publish/GpxViewer2/Contents/MacOS -p:PublishReadyToRun=true ../src/GpxViewer2/GpxViewer2.csproj
cp -r ../macos-app-template/* ../publish/GpxViewer2/Contents
mv ../publish/GpxViewer2 "../publish/RolandK GpxViewer 2.app"