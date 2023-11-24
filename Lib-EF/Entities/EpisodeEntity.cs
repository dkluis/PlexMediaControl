using System.Diagnostics;
using Common_Lib;
using PlexMediaControl.Models.MariaDB;
using PlexMediaControl.Models.TvmApis;
using Web_Lib;
using Web_Lib.DTOs;

namespace PlexMediaControl.Entities;

public class EpisodeEntity : Episode, IDisposable
{
    public EpisodeEntity(AppInfo appInfo)
    {
        AppInfo = appInfo;
    }
    public  TvmEpisode TvmEpisodeInfo { get; set; } = new();
    private AppInfo    AppInfo        { get; }
    void IDisposable.Dispose()
    {
        GC.SuppressFinalize(this);
    }
    public Response GetAllEpisodes(int showId, bool getTvmInfo = false)
    {
        var resp = new Response();
        try
        {
            using var db           = new TvMaze();
            var       showEpisodes = db.Episodes.Where(s => s.TvmShowId == showId).ToArray();
            if (showEpisodes.Length != 0)
            {
                resp.Success        = true;
                resp.ResponseObject = showEpisodes;
            } else
            {
                resp.Success = false;
                resp.Message = $"No Episodes for {showId} found";
            }
        }
        catch (Exception e)
        {
            resp.Success      = false;
            resp.Message      = "Db operation error";
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
    public static Response UpdateAllEpisodes(int showId, bool isSkipping)
    {
        var resp        = new Response();
        var allEpisodes = GetEpisodesOnTvMaze(showId);
        if (!allEpisodes.Success)
        {
            resp.ErrorMessage = allEpisodes.ErrorMessage;
            return resp;
        }
        var       allFoundEpisodes = allEpisodes.ResponseObject as List<TvmEpisode>;
        using var db               = new TvMaze();
        foreach (var episode in allFoundEpisodes!)
        {
            var epiInDb  = db.Episodes.SingleOrDefault(e => e.Id == episode.EpisodeId);
            var epiExist = epiInDb != null;
            if (epiExist)
                if (isSkipping && episode.Status != "Watched")
                    // Delete Episodes if Show is set to Skipping and not is Watched before
                    try
                    {
                        resp.InfoMessage += $"Deleting Episode {episode.EpisodeId} due to Show set to Skipping and is not Watched";
                        db.Remove(epiInDb!);
                        db.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        resp.ErrorMessage += $"Db Error: {e.Message} {e.InnerException}";
                        var result                             = ActionItemEntity.Record(new ActionItem {Program = "Episode Entity", Message = $"Db Error: Deleting Episode {episode.EpisodeId} due to Show set to Skipping and is not Watched", UpdateDateTime = DateTime.Now});
                        if (!result.Success) resp.ErrorMessage += $" *** ActionItem Db Error {result.ErrorMessage}";
                    }
            // Update Episodes if show is not set to Skipping or is set to Watched
            // Add the Episode
        }

        // Check for Epi in DB but not in TvMaze anymore:

        return resp;
    }
    private static Response GetEpisodesOnTvMaze(int showId)
    {
        var           resp               = new Response();
        var           tvmApi             = new WebApi(new AppInfo("PlexMediaControl", "UpdateAllEpisodes"));
        var episodesDTos = tvmApi.GetEpisodesByShow(showId);
        if (episodesDTos == null)
        {
            resp.Success      = false;
            resp.ErrorMessage = "No Episodes found";
            return resp;
        }
        var           allEpisodes        = new List<TvmEpisode>();
        foreach (var episode in episodesDTos)
        {
            var     epiId        = episode.Id;
            var     tvmApiMarks  = new WebApi(new AppInfo("PlexMediaControl", "UpdateAllEpisodes"));
            var     epiMarkedDto = tvmApiMarks.GetEpisodeMarks(epiId);
            string? tvmType      = null;
            if (epiMarkedDto == null)
            {
                tvmType = "Unknown";
            } else
            {
                tvmType = ConvertEpisodeMarking(epiMarkedDto.Type ?? 99);
            }
            var epiAirDate   = episode.Airdate ?? new DateTime(1900, 01, 01, 0, 0, 0);
            var airtime      = episode.Airtime ?? "";

            var epi = new TvmEpisode
                      {
                          EpisodeId = epiId,
                          Url       = episode.Url ?? "",
                          Name      = episode.Name ?? "",
                          Season    = episode.Season ?? 0,
                          Number    = episode.Number ?? 0,
                          Type      = episode.Type ?? "",
                          AirDate   = epiAirDate,
                          RunTime   = episode.Runtime ?? 0,
                          Status    = tvmType,
                      };
            allEpisodes.Add(epi);
        }

        resp.Success        = true;
        resp.ResponseObject = allEpisodes;
        return resp;
    }
    private static string ConvertEpisodeMarking(int tvmType)
    {
        var result = tvmType switch
                     {
                         0 => "Watched", 1 => "Acquired", 2 => "Skipped", _ => "Unknown",
                     };
        return result;
    }
}
