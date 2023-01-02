using Common_Lib;
using Web_Lib;
using PlexMediaControl.Models.MariaDB;
using PlexMediaControl.Models.TvmApis;


namespace PlexMediaControl.Entities;

public class EpisodeController: Episode, IDisposable
{
    public TvmEpisode TvmEpisodeInfo { get; set; } = new TvmEpisode();
    private AppInfo AppInfo { get; }
    
    void IDisposable.Dispose()
    {
        GC.SuppressFinalize(this);
    }
    
    public EpisodeController(AppInfo appInfo)
    {
        AppInfo = appInfo;
    }


}
