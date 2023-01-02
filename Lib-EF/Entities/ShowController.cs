using Common_Lib;
using Microsoft.Extensions.Logging;
using Web_Lib;
using PlexMediaControl.Models.MariaDB;
using PlexMediaControl.Models.TvmApis;


namespace PlexMediaControl.Entities;

public class ShowController : Show, IDisposable
{
    private AppInfo AppInfo { get; }
    public TvmShow TvmShowInfo { get; set; } = new TvmShow();

    public ShowController(AppInfo appInfo)
    {
        AppInfo = appInfo;
    }

    void IDisposable.Dispose()
    {
        GC.SuppressFinalize(this);
    }

    public Response Get(int showId, bool getTvmInfo = false)
    { 
        var resp = new Response();
        try
        {
            using var db = new TvMazeNewDbContext();
            var show = db.Shows.SingleOrDefault(s => s.TvmShowId == showId);
            if (show != null)
            {
                resp.Success = true;
                resp.ResponseObject = show;
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

        if (!getTvmInfo) return resp;
        
        var result = GetTvmShowInfo();
        if (result.Success) return resp;
        
        resp.Success = false;
        resp.Message = "Trying to get TvMaze Info";
        resp.ErrorMessage = result.ErrorMessage;
        return resp;
    }

    public Response Get(string showName)
    {
        var resp = new Response();
        var cleanedShowName = Common.RemoveSpecialCharsInShowName(showName);
        try
        {
            using var db = new TvMazeNewDbContext();
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
        if (TvmShowId == 0) { resp.ErrorMessage = $"No TvmShowId was set"; return resp; } 
        
        using var db = new TvMazeNewDbContext();
        var showExist = db.Shows.SingleOrDefault(s => s.TvmShowId == TvmShowId);
        var followedExist = db.Followeds.SingleOrDefault(f => f.TvmShowId == TvmShowId);
        var tvmShowUpdates = db.TvmShowUpdates.SingleOrDefault(t => t.TvmShowId == TvmShowId);
        
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
            var resultGet = GetTvmShowInfo();
            if (!resultGet.Success) { resp.ErrorMessage = $"Could not get the Show {TvmShowId} from TvMaze {resultGet.ErrorMessage}"; return resp; }
            
            ShowName = TvmShowInfo.Name;
            TvmUrl = TvmShowInfo.Url;
            TvmStatus = TvmShowInfo.Status;
            PremiereDate = TvmShowInfo.PremiereDate;
            if (string.IsNullOrEmpty(MediaType)) MediaType = "TS";
            if (string.IsNullOrEmpty(Finder)) Finder = "Multi";
            if (string.IsNullOrEmpty(ShowStatus) && Finder != "Skip")
            {
                ShowStatus = "Following";
            }
            else if (Finder == "Skip")
            {
                ShowStatus = "Skipping";
            }
            
            // Validate the Show for insert in the DB
            var validResp = Valid();
            if (!validResp.Success)
            {
                resp.Success = false;
                resp.Message = "Validation Errors";
                resp.InfoMessage = validResp.ErrorMessage;
                return resp;
            }

            // Handle the needed tvmShowUpdate record
            if (tvmShowUpdates == null)
            {
                using var tvmShowUpdateController = new TvmShowUpdateController();
                tvmShowUpdateController.TvmShowId = TvmShowId;
                tvmShowUpdateController.TvmUpdateEpoch = TvmShowInfo.Updated;
                tvmShowUpdateController.TvmUpdateDate = DateOnly.Parse(DateTime.Now.ToShortDateString());
                var resultAddTsu = tvmShowUpdateController.Add();
                if (!resultAddTsu.Success)
                {
                    resp.Success = false;
                    resp.Message = "TvmShowUpdateController " + resultAddTsu.Message;
                    resp.InfoMessage = resultAddTsu.InfoMessage;
                    resp.ErrorMessage = resultAddTsu.ErrorMessage;
                    resp.InfoMessage = $"tvmShowUpdate";
                }
            }

            // Handle the needed Followed record
            if (followedExist == null)
            {
                using var followedController = new FollowedController();
                followedController.TvmShowId = TvmShowId;
                followedController.UpdateDate = UpdateDate;
                var resultAddFol = followedController.Add();
                if (!resultAddFol.Success)
                {
                    resp.Success = false;
                    resp.Message = "FollowedController " + resultAddFol.Message;
                    resp.InfoMessage = resultAddFol.InfoMessage;
                    resp.ErrorMessage = resultAddFol.ErrorMessage;
                    resp.InfoMessage = $"tvmShowUpdate";
                }
            }

            CleanedShowName = Common.RemoveSpecialCharsInShowName(ShowName);
            if (TvmStatus == "Skipping" || Finder == "Skip")
            {
                TvmStatus = "Skipping";
                Finder = "Skip";
                UpdateDate = DateOnly.Parse("2200-01-01");
            }
            else
            {
                UpdateDate = DateOnly.FromDateTime(DateTime.Now);
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
            var aiRec = new ActionItem
            {
                Program = "ShowController",
                Message = $"Db Failure {TvmShowId} {ShowName} {e.Message} {e.InnerException}",
            };
            var result = ActionItemController.Record(aiRec);
            if (!result.Success) { resp.ErrorMessage += $" Action Write also failed {result.ErrorMessage}"; }
            
            return (resp);
        }

        resp.Success = true;
        resp.Message = $"Show record is created {TvmShowId} {ShowName}";
        resp.InfoMessage = "";

        return resp;
    }

    private Response GetTvmShowInfo()
    {
        var resp = new Response();

        using WebApi tvmApi = new(AppInfo);
        var showJson = tvmApi.ConvertHttpToJObject(tvmApi.GetShow(TvmShowId));

        if (showJson.Count == 0) { resp.ErrorMessage = $"ShowId {TvmShowId} not found"; return resp; }

        resp.ErrorMessage += showJson["id"] == null ? "Id was null, " : "";
        resp.ErrorMessage += showJson["url"] == null ? "Url was null, " : "";
        resp.ErrorMessage += showJson["name"] == null ? "Name was null, " : "";
        resp.InfoMessage += showJson["updated"] == null ? "Updated was null, " : "";
        resp.InfoMessage += showJson["premiered"] == null ? "Premiered was null, " : "";
        resp.ErrorMessage += showJson["status"] == null ? "Status was null, " : "";
        if (!string.IsNullOrEmpty(resp.ErrorMessage)) return resp;

        var premDate = DateOnly.Parse("1900-01-01");
        if (showJson["premiered"] != null) premDate = DateOnly.Parse(showJson["premiered"]!.ToString());
        var endDate = DateOnly.Parse("2300-01-01");
        if (showJson["ended"] != null) endDate = DateOnly.Parse(showJson["ended"]!.ToString());

        TvmShowInfo.Id = int.Parse(showJson["id"]!.ToString());
        TvmShowInfo.Url = showJson["url"]!.ToString();
        TvmShowInfo.Name = showJson["name"]!.ToString();
        TvmShowInfo.Language = showJson["language"]?.ToString();
        TvmShowInfo.Updated = int.Parse(showJson["updated"]!.ToString());
        TvmShowInfo.Status = showJson["status"]!.ToString();
        TvmShowInfo.Country = showJson["network"]?["country"]?["name"]?.ToString();
        TvmShowInfo.CountryCode = showJson["network"]?["country"]?["code"]?.ToString();
        TvmShowInfo.Type = showJson["type"]?.ToString();
        TvmShowInfo.Network = showJson["network"]?["name"]?.ToString();
        TvmShowInfo.NetworkUrl = showJson["network"]?["officialSite"]?.ToString();
        TvmShowInfo.RunTime = int.Parse(showJson["runtime"]?.ToString() ?? string.Empty);
        TvmShowInfo.EndDate = endDate;
        PremiereDate = premDate;
  
        resp.Success = true;
        return resp;
    }

    private Response Valid()
    {
        var resp = new Response();
        if (TvmShowId == 0) resp.ErrorMessage += "No TvmShowId Found, ";
        if (string.IsNullOrEmpty(ShowName)) resp.ErrorMessage += "No ShowName Found, ";
        if (string.IsNullOrEmpty(TvmStatus)) resp.ErrorMessage += "No TvmStatus Found, ";
        if (string.IsNullOrEmpty(TvmUrl)) resp.ErrorMessage += "No TvmUrl Found, ";
        if (string.IsNullOrEmpty(ShowStatus)) resp.ErrorMessage += "No ShowStatus Found, ";
        if (PremiereDate == DateOnly.Parse("0001/01/01")) resp.ErrorMessage += "No PremiereDate Found, ";
        if (string.IsNullOrEmpty(Finder)) resp.ErrorMessage += "No Finder Found, ";
        if (string.IsNullOrEmpty(MediaType)) resp.ErrorMessage += "No MediaType Found, ";
        
        if (resp.ErrorMessage == null) resp.Success = true;
        return resp;
    }
}
