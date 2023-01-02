using Common_Lib;
using Web_Lib;
using PlexMediaControl.Models.MariaDB;
using PlexMediaControl.Models.TvmApis;


namespace PlexMediaControl.Entities;

public class EpisodeController: Episode, IDisposable
{
    private AppInfo AppInfo { get; }
    
    public EpisodeController(AppInfo appInfo)
    {
        AppInfo = appInfo;
    }

    void IDisposable.Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
