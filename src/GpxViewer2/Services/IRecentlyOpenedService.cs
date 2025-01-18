using GpxViewer2.Services.RecentlyOpened;

namespace GpxViewer2.Services;

public interface IRecentlyOpenedService
{
    Task AddOpenedAsync(string path, RecentlyOpenedType type);

    Task AddOpenedAsync(IReadOnlyList<string> paths, RecentlyOpenedType type);

    Task<RecentlyOpenedFileOrDirectoryModel?> TryGetLastOpenedAsync();

    Task<IReadOnlyList<RecentlyOpenedFileOrDirectoryModel>> GetAllRecentlyOpenedAsync();
}
