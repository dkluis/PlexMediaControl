using Common_Lib;
using PlexMediaControl.Models.MariaDB;
using PlexMediaControl.Models.TvmApis;

namespace PlexMediaControl.Entities;

public class EpisodeController : Episode, IDisposable
{
    public EpisodeController(AppInfo appInfo)
    {
        AppInfo = appInfo;
    }

    public TvmEpisode TvmEpisodeInfo { get; set; } = new();
    private AppInfo AppInfo { get; }

    void IDisposable.Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
