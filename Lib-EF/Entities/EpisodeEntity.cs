using Common_Lib;
using PlexMediaControl.Models.MariaDB;
using PlexMediaControl.Models.TvmApis;
using Web_Lib;

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
        var resp               = new Response();
        var tvmApi             = new WebApi(new AppInfo("PlexMediaControl", "UpdateAllEpisodes"));
        var episodesByShowJson = tvmApi.ConvertHttpToJArray(tvmApi.GetEpisodesByShow(showId));
        var allEpisodes        = new List<TvmEpisode>();
        foreach (var episode in episodesByShowJson)
        {
            var     epiId         = !string.IsNullOrEmpty(episode["id"]?.ToString()) ? int.Parse(episode["id"]!.ToString()) : 0;
            var     tvmApiMarks   = new WebApi(new AppInfo("PlexMediaControl", "UpdateAllEpisodes"));
            var     epiMarkedJson = tvmApiMarks.ConvertHttpToJObject(tvmApiMarks.GetEpisodeMarks(epiId));
            string? tvmType       = null;
            if (!string.IsNullOrEmpty(epiMarkedJson.ToString()) && epiMarkedJson.ToString() != "{}")
            {
                var epiType = !string.IsNullOrEmpty(epiMarkedJson["type"]?.ToString()) ? int.Parse(epiMarkedJson["type"]!.ToString()) : 99;
                tvmType = ConvertEpisodeMarking(epiType);
            }

            var epiAirDate   = !string.IsNullOrEmpty(epiMarkedJson["airdate"]?.ToString()) ? DateTime.Parse(epiMarkedJson["airdate"]!.ToString()) : new DateTime(1900, 01, 01, 0, 0, 0);
            var airtime      = !string.IsNullOrEmpty(epiMarkedJson["airtime"]?.ToString()) ? epiMarkedJson["airtime"]!.ToString() : "";
            var airtimeSplit = airtime.Split(":");
            if (airtimeSplit.Length > 1)
            {
                epiAirDate = epiAirDate.AddHours(int.Parse(airtimeSplit[0]));
                epiAirDate = epiAirDate.AddMinutes(int.Parse(airtimeSplit[1]));
            }

            var epi = new TvmEpisode
                      {
                          EpisodeId = epiId,
                          Url       = string.IsNullOrEmpty(episode["url"]?.ToString()) ? episode["url"]!.ToString() : "",
                          Name      = string.IsNullOrEmpty(episode["name"]?.ToString()) ? episode["name"]!.ToString() : "",
                          Season    = string.IsNullOrEmpty(episode["season"]?.ToString()) ? int.Parse(episode["season"]!.ToString()) : 0,
                          Number    = string.IsNullOrEmpty(episode["number"]?.ToString()) ? int.Parse(episode["number"]!.ToString()) : 0,
                          Type      = string.IsNullOrEmpty(episode["type"]?.ToString()) ? episode["type"]!.ToString() : "",
                          AirDate   = epiAirDate,
                          RunTime   = string.IsNullOrEmpty(episode["runtime"]?.ToString()) ? int.Parse(episode["runtime"]!.ToString()) : 0,
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
