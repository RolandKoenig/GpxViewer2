using System.Text.Json;

namespace GpxViewer2.Services.RecentlyOpened;

public class RecentlyOpenedService : IRecentlyOpenedService
{
    private const string FILE_NAME = "recentlyOpened.json";

    private readonly string _applicationDirectoryName;
    private readonly int _maxEntryCount;

    public RecentlyOpenedService(string applicationDirectoryName, int maxEntryCount)
    {
        if (maxEntryCount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maxEntryCount));
        }
        if (string.IsNullOrEmpty(applicationDirectoryName))
        {
            throw new ArgumentException(nameof(applicationDirectoryName));
        }

        _applicationDirectoryName = applicationDirectoryName;
        _maxEntryCount = maxEntryCount;
    }

    public async Task AddOpenedAsync(string path, RecentlyOpenedType type)
    {
        await AddOpenedAsync([path], type);
    }

    /// <inheritdoc />
    public async Task AddOpenedAsync(IReadOnlyList<string> paths, RecentlyOpenedType type)
    {
        if (paths.Count == 0)
        {
            throw new ArgumentException(nameof(paths));
        }

        var model = await this.ReadRecentlyOpenedAsync();
        foreach (var actPath in paths)
        {
            if (string.IsNullOrEmpty(actPath))
            {
                continue;
            }

            model.Entries.RemoveAll(
                x => x.FullPath.Equals(actPath, StringComparison.InvariantCultureIgnoreCase));
            model.Entries.Insert(0, new RecentlyOpenedFileOrDirectoryModel()
            {
                FullPath = actPath,
                Type = type
            });
        }

        while (model.Entries.Count > _maxEntryCount)
        {
            model.Entries.RemoveAt(model.Entries.Count - 1);
        }

        await this.WriteRecentlyOpenedAsync(model);
    }

    public async Task<RecentlyOpenedFileOrDirectoryModel?> TryGetLastOpenedAsync()
    {
        var model = await this.ReadRecentlyOpenedAsync();

        if (model.Entries.Count > 0)
        {
            return model.Entries[0];
        }

        return null;
    }

    public async Task<IReadOnlyList<RecentlyOpenedFileOrDirectoryModel>> GetAllRecentlyOpenedAsync()
    {
        var model = await this.ReadRecentlyOpenedAsync();
        return model.Entries
            .ToArray();
    }

    private async Task WriteRecentlyOpenedAsync(RecentlyOpenedModel model)
    {
        var directory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var appPath = Path.Combine(directory, _applicationDirectoryName);

        if (!Directory.Exists(appPath))
        {
            Directory.CreateDirectory(appPath);
        }

        var filePath = Path.Combine(appPath, FILE_NAME);
        await using var fileStream = File.Create(filePath);

        await JsonSerializer.SerializeAsync(
            fileStream, model,
            new JsonSerializerOptions(JsonSerializerDefaults.General)
            {
                WriteIndented = true,
            });
    }

    private async Task<RecentlyOpenedModel> ReadRecentlyOpenedAsync()
    {
        var directory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var appPath = Path.Combine(directory, _applicationDirectoryName);

        if (!Directory.Exists(appPath))
        {
            return new RecentlyOpenedModel();
        }

        var filePath = Path.Combine(appPath, FILE_NAME);
        if (!File.Exists(filePath))
        {
            return new RecentlyOpenedModel();
        }

        try
        {
            await using var fileStream = File.OpenRead(filePath);
            var result = await JsonSerializer.DeserializeAsync<RecentlyOpenedModel>(fileStream);

            result ??= new RecentlyOpenedModel();
            return result;
        }
        catch (Exception)
        {
            return new RecentlyOpenedModel();
        }
    }
}
