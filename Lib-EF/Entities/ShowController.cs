using Common_Lib;
using PlexMediaControl.Models.MariaDB;
using PlexMediaControl.Models.TvmApis;
using Web_Lib;

namespace PlexMediaControl.Entities;

public class ShowController : Show, IDisposable
{
    public TvmShow TvmShowInfo { get; set; } = new TvmShow();
    //public new List<Episode> Episodes { get; set; } = new List<Episode>();

    public ShowController(AppInfo appInfo)
    {
        AppInfo = appInfo;
    }

    private AppInfo AppInfo { get; }

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
        if (!result.Success)
        {
            // Figure out what to return and why
        }

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
            
            var result = GetTvmShowInfo();
            if (!result.Success) { resp.ErrorMessage = $"Could not get the Show {TvmShowId} from TvMaze {result.ErrorMessage}"; return resp; }
            
            ShowName = TvmShowInfo!.Name;
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
            };
            
            var validResp = Valid();
            if (!validResp.Success)
            {
                resp.Success = false;
                resp.Message = "Validation Errors";
                resp.InfoMessage = validResp.ErrorMessage;
                return resp;
            }
            
            if (tvmShowUpdates == null)
            {
                using var tvmShowUpdateController = new TvmShowUpdateController()
                {
                    TvmShowId = TvmShowId,
                    TvmUpdateEpoch = TvmShowInfo.Updated,
                    TvmUpdateDate = DateOnly.Parse(DateTime.Now.ToShortDateString()),
                };
                var tsu = tvmShowUpdateController.Add();
                if (!tsu.Success)
                {
                    resp.Success = false;
                    resp.Message = "tvmShowUpdateController " + tsu.Message;
                    resp.InfoMessage = tsu.InfoMessage;
                    resp.ErrorMessage = tsu.ErrorMessage;
                    return resp;
                }
                resp.InfoMessage = $"tvmShowUpdate";
            }
            
            if (followedExist == null)
            {
                // var tsu = FollowedController.Add();
                // if (!tsu.Success)
                // {
                //     resp.Success = false;
                //     resp.Message = "tvmShowUpdateController " + tsu.Message;
                //     resp.InfoMessage = tsu.InfoMessage;
                //     resp.ErrorMessage = tsu.ErrorMessage;
                //     return resp;
                // }
                resp.InfoMessage = $"followed";
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

        
        TvmShowInfo!.Id = int.Parse(showJson["id"]!.ToString());
        TvmShowInfo!.Url = showJson["url"]!.ToString();
        TvmShowInfo.Name = showJson["name"]!.ToString();
        TvmShowInfo.Language = showJson["language"]?.ToString();
        TvmShowInfo.Updated = int.Parse(showJson["updated"]!.ToString());
        TvmShowInfo.Status = showJson["status"]!.ToString();
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
