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

    public Response GetAllEpisodes(int showId, bool getTvmInfo = false)
    {
        var resp = new Response();
        try
        {
            using var db = new TvMazeNewDbContext();
            var showEpisodes = db.Episodes.Where(s => s.TvmShowId == showId).ToArray();
            if (showEpisodes.Length != 0)
            {
                resp.Success = true;
                resp.ResponseObject = showEpisodes;
            }
            else
            {
                resp.Success = false;
                resp.Message = $"No Episodes for {showId} found";
            }
        }
        catch (Exception e)
        {
            resp.Success = false;
            resp.Message = "Db operation error";
            resp.ErrorMessage = $"{resp.Message}: {e.Message} {e.InnerException}";
        }

        if (!getTvmInfo) return resp;

        // var result = GetTvmShowInfo();
        // if (result.Success) return resp;

        // resp.Success = false;
        // resp.Message = "Trying to get TvMaze Info";
        // resp.ErrorMessage = result.ErrorMessage;
        return resp;
    }
}
