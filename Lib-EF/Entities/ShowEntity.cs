using Common_Lib;
using Microsoft.EntityFrameworkCore;
using PlexMediaControl.Models.MariaDB;
using PlexMediaControl.Models.TvmApis;
using Web_Lib;

namespace PlexMediaControl.Entities;

public class ShowEntity : Show, IDisposable
{
    public ShowEntity(AppInfo appInfo)
    {
        this.AppInfo = appInfo;
    }
    private AppInfo AppInfo { get; }
    public TvmShow TvmShowInfo { get; set; } = new();

    void IDisposable.Dispose()
    {
        GC.SuppressFinalize(this);
    }

    public Response Get(int showId, bool getTvmInfo = false, bool getEpisodes = false)
    {
        var resp = new Response();
        try
        {
            using var db = new TvMaze();
            var show = getEpisodes ? db.Shows.Include(e => e.Episodes).SingleOrDefault(s => s.TvmShowId == showId) : db.Shows.SingleOrDefault(s => s.TvmShowId == showId);
            if (show != null)
            {
                CopyShow(show);
                resp.Success = true;
                resp.ResponseObject = this;
                
                if (!getTvmInfo) return resp;

                var result = GetTvmShowInfo(showId);
                if (result.Success)
                {
                    resp.ResponseObject = this;
                    return resp;
                }

                resp.Success = false;
                resp.Message += "Trying to get TvMaze Info";
                resp.ErrorMessage += result.ErrorMessage;
            }
            else
            {
                resp.Success = false;
                resp.Message = $"ShowId {showId} not found";
            }
        }
        catch (Exception e)
        {
            resp.Success = false;
            resp.Message = "Db operation error";
            resp.ErrorMessage = $"{resp.Message}: {e.Message} {e.InnerException}";
        }
        
        return resp;
    }

    public Response Get(string showName)
    {
        var resp = new Response();
        var cleanedShowName = Common.RemoveSpecialCharsInShowName(showName);
        try
        {
            using var db = new TvMaze();
            var shows = db.Shows
                .Where(s => s.ShowName == showName ||
                            s.AltShowname == showName ||
                            s.CleanedShowName == cleanedShowName)
                .Select(s => new {s.Id, s.TvmShowId, s.ShowName, s.ShowStatus})
                .ToList();
            resp.Success = true;
            resp.ResponseObject = shows;
        }
        catch (Exception e)
        {
            resp.Success = false;
            resp.Message = "Db operation error";
            resp.ErrorMessage = $"{resp.Message}: {e.Message} {e.InnerException}";
        }

        return resp;
    }

    public Response Add()
    {
        var resp = new Response();
        if (TvmShowId == 0)
        {
            resp.ErrorMessage = "No TvmShowId was set";
            return resp;
        }

        using var db = new TvMaze();
        var showExist = db.Shows.SingleOrDefault(s => s.TvmShowId == TvmShowId);
        var followedExist = db.Followeds.SingleOrDefault(f => f.TvmShowId == TvmShowId);
        var tvmShowUpdateExist = db.TvmShowUpdates.SingleOrDefault(t => t.TvmShowId == TvmShowId);

        try
        {
            if (showExist != null)
            {
                resp.Success = false;
                resp.Message = "Validation Errors";
                resp.InfoMessage = $"Show already exists in DB {showExist.TvmShowId} {showExist.ShowName}";
                return resp;
            }

            // Get TvMaze Info on the show
            var resultGet = GetTvmShowInfo(TvmShowId);
            if (!resultGet.Success)
            {
                resp.ErrorMessage = $"Could not get the Show {TvmShowId} from TvMaze {resultGet.ErrorMessage}";
                return resp;
            }

            ShowName = TvmShowInfo.Name;
            TvmUrl = TvmShowInfo.Url;
            ShowStatus = TvmShowInfo.Status;
            PremiereDate = TvmShowInfo.PremiereDate;
            if (string.IsNullOrEmpty(MediaType)) MediaType = "TS";
            if (string.IsNullOrEmpty(Finder)) Finder = "Multi";
            if (string.IsNullOrEmpty(TvmStatus) && Finder != "Skip")
                TvmStatus = "Following";
            else if (Finder == "Skip") TvmStatus = "Skipping";

            // Validate the Show for insert in the DB
            var validResp = Validate();
            if (!validResp.Success)
            {
                resp.Success = false;
                resp.Message = "Validation Errors";
                resp.InfoMessage = validResp.ErrorMessage;
                return resp;
            }

            // Handle the needed tvmShowUpdate record
            if (tvmShowUpdateExist == null)
            {
                using var tvmShowUpdateController = new TvmShowUpdateController();
                tvmShowUpdateController.TvmShowId = TvmShowId;
                tvmShowUpdateController.TvmUpdateEpoch = TvmShowInfo.Updated;
                tvmShowUpdateController.TvmUpdateDate = DateTime.Now;
                var resultAddTsu = tvmShowUpdateController.Add();
                if (!resultAddTsu.Success)
                {
                    resp.Success = false;
                    resp.Message = "TvmShowUpdateController " + resultAddTsu.Message;
                    resp.InfoMessage = resultAddTsu.InfoMessage;
                    resp.ErrorMessage = resultAddTsu.ErrorMessage;
                    resp.InfoMessage = "tvmShowUpdate";
                    var result = ActionItemEntity.Record(new ActionItem
                    {
                        Program = "ShowEntity Add",
                        Message = $"Db Error Updating the TvmShowUpdate record for {TvmShowId} {ShowName}",
                        UpdateDateTime = DateTime.Now,
                    });
                    if (!result.Success) resp.ErrorMessage += $" *** ActionItem Db Error {result.ErrorMessage}";

                }
            }
            else
            {
                // ToDo Update the Epoch date, etc.
            }

            // Handle the needed Followed record
            if (followedExist == null)
            {
                using var followedController = new FollowedEntity();
                followedController.TvmShowId = TvmShowId;
                followedController.UpdateDate = UpdateDate;
                var resultAddFol = followedController.Add();
                if (!resultAddFol.Success)
                {
                    resp.Success = false;
                    resp.Message = "FollowedEntity " + resultAddFol.Message;
                    resp.InfoMessage = resultAddFol.InfoMessage;
                    resp.ErrorMessage = resultAddFol.ErrorMessage;
                    resp.InfoMessage = "tvmShowUpdate";
                    
                    var result = ActionItemEntity.Record(new ActionItem
                    {
                        Program = "ShowEntity Add",
                        Message = $"Db Error Updating the Followed record for {TvmShowId} {ShowName}",
                        UpdateDateTime = DateTime.Now,
                    });
                    if (!result.Success) resp.ErrorMessage += $" *** ActionItem Db Error {result.ErrorMessage}";
                    
                    return resp;
                    
                }
            }

            CleanedShowName = Common.RemoveSpecialCharsInShowName(ShowName);
            if (TvmStatus == "Skipping" || Finder == "Skip")
            {
                TvmStatus = "Skipping";
                Finder = "Skip";
                UpdateDate = new DateTime(2200,01,01,0,0,0);
            }
            else
            {
                UpdateDate = DateTime.Now;
            }

            db.Add(this);
            db.SaveChanges();
            resp.InfoMessage = "Show";
        }
        catch (Exception e)
        {
            resp.Success = false;
            resp.Message = "Exception Error";
            resp.ErrorMessage = $"{e.Message}, {e.InnerException}";
          
            var result = ActionItemEntity.Record(new ActionItem
            {
                Program = "ShowEntity Add",
                Message = $"Db Error Updating the TvmShowUpdate record for {TvmShowId} {ShowName}",
                UpdateDateTime = DateTime.Now,
            });
            if (!result.Success) resp.ErrorMessage += $" *** ActionItem Db Error {result.ErrorMessage}";

            return resp;
        }

        var skipping = Finder == "Skip" || TvmStatus == "Skipping";
        var episodesResult = EpisodeEntity.UpdateAllEpisodes(TvmShowId, skipping);
        if (!episodesResult.Success)
        {
            var result = ActionItemEntity.Record(new ActionItem
            {
                Program = "ShowEntity Add",
                Message = $"Errors during the Episodes Update record for {TvmShowId} {ShowName}",
                UpdateDateTime = DateTime.Now,
            });
            if (!result.Success) resp.ErrorMessage += $" *** ActionItem Db Error {result.ErrorMessage}";
        }
        
        resp.Success = true;
        resp.Message = $"Show record is created {TvmShowId} {ShowName}";
        resp.InfoMessage = "";

        return resp;
    }

    public Response Delete()
    {
        var resp = new Response();

        return resp;
    }

    private Response GetTvmShowInfo(int showId)
    {
        var resp = new Response();

        using WebApi tvmApi = new(AppInfo);
        var showJson = tvmApi.ConvertHttpToJObject(tvmApi.GetShow(showId));

        if (showJson.Count == 0)
        {
            resp.ErrorMessage = $"ShowId {TvmShowId} not found";
            return resp;
        }

        resp.ErrorMessage += showJson["id"] == null ? "Id was null, " : "";
        resp.ErrorMessage += showJson["url"] == null ? "Url was null, " : "";
        resp.ErrorMessage += showJson["name"] == null ? "Name was null, " : "";
        resp.InfoMessage += showJson["updated"] == null ? "Updated was null, " : "";
        resp.InfoMessage += showJson["premiered"] == null ? "Premiered was null, " : "";
        resp.ErrorMessage += showJson["status"] == null ? "Status was null, " : "";
        if (!string.IsNullOrEmpty(resp.ErrorMessage)) return resp;

        TvmShowInfo!.Id = int.Parse(showJson["id"]!.ToString());
        TvmShowInfo.Url = showJson["url"]!.ToString();
        TvmShowInfo.Name = showJson["name"]!.ToString();
        TvmShowInfo.Language = !string.IsNullOrEmpty(showJson["language"]?.ToString()) ? showJson["language"]!.ToString() : string.Empty;
        TvmShowInfo.Updated = int.Parse(showJson["updated"]!.ToString());
        TvmShowInfo.Status = !string.IsNullOrEmpty(showJson["status"]?.ToString()) ? showJson["status"]!.ToString() : string.Empty;
        TvmShowInfo.Type = !string.IsNullOrEmpty(showJson["type"]?.ToString()) ? showJson["type"]!.ToString() : string.Empty;
        
        if (!string.IsNullOrEmpty(showJson["network"]?.ToString()))
        {
            if (!string.IsNullOrEmpty(showJson["network"]!["country"]?.ToString()))
            {
                TvmShowInfo.Country = !string.IsNullOrEmpty(showJson["network"]!["country"]!["name"]?.ToString()) ? showJson["network"]!["country"]!["name"]!.ToString() : string.Empty;
                TvmShowInfo.CountryCode = !string.IsNullOrEmpty(showJson["network"]!["country"]!["code"]?.ToString()) ? showJson["network"]!["country"]!["code"]!.ToString() : string.Empty;
            }
            TvmShowInfo.Network = showJson["network"]!["name"]?.ToString();
            TvmShowInfo.NetworkUrl = showJson["network"]!["officialSite"]?.ToString();
        }
        TvmShowInfo.RunTime =  !string.IsNullOrEmpty(showJson["runtime"]?.ToString()) ? int.Parse(showJson["runtime"]!.ToString()) : 0;
        TvmShowInfo.EndDate = !string.IsNullOrEmpty(showJson["ended"]?.ToString()) ? DateTime.Parse(showJson["ended"]!.ToString()) : new DateTime(2300,01,01, 0, 0, 0);
        TvmShowInfo.PremiereDate = !string.IsNullOrEmpty(showJson["premiered"]?.ToString()) ? DateTime.Parse(showJson["premiered"]!.ToString()) : new DateTime(1900,01,01, 0, 0, 0);

        resp.Success = true;
        return resp;
    }

    private void CopyShow(Show show)
    {
        Id = show.Id;
        TvmShowId = show.TvmShowId;
        TvmStatus = show.ShowStatus;
        TvmUrl = show.TvmUrl;
        ShowName = show.ShowName;
        ShowStatus = show.ShowStatus;
        PremiereDate = show.PremiereDate;
        Finder = show.Finder;
        MediaType = show.MediaType;
        CleanedShowName = show.CleanedShowName;
        AltShowname = show.AltShowname;
        UpdateDate = show.UpdateDate;
    }

    private Response Validate()
    {
        var resp = new Response();
        if (TvmShowId == 0) resp.ErrorMessage += "No TvmShowId Found, ";
        if (string.IsNullOrEmpty(ShowName)) resp.ErrorMessage += "No ShowName Found, ";
        if (string.IsNullOrEmpty(TvmStatus)) resp.ErrorMessage += "No TvmStatus Found, ";
        if (string.IsNullOrEmpty(TvmUrl)) resp.ErrorMessage += "No TvmUrl Found, ";
        if (string.IsNullOrEmpty(ShowStatus)) resp.ErrorMessage += "No ShowStatus Found, ";
        if (PremiereDate == new DateTime(0001,01,01,0,0,0)) resp.ErrorMessage += "No PremiereDate Found, ";
        if (string.IsNullOrEmpty(Finder)) resp.ErrorMessage += "No Finder Found, ";
        if (string.IsNullOrEmpty(MediaType)) resp.ErrorMessage += "No MediaType Found, ";

        if (resp.ErrorMessage == null) resp.Success = true;
        return resp;
    }
}
