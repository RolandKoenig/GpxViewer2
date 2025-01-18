namespace GpxViewer2.Services.StartupArguments;

public class StartupArgumentsContainer : IStartupArgumentsContainer
{
    /// <inheritdoc />
    public string? InitialFile { get; }

    public StartupArgumentsContainer(string[] args)
    {
        if ((args.Length > 0) &&
            (File.Exists(args[0])))
        {
            this.InitialFile = args[0];
        }
    }
}
