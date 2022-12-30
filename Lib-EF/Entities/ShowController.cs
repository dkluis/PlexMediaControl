using System.Globalization;
using Common_Lib;
using PlexMediaControl.Models.MariaDB;

namespace PlexMediaControl.Entities;

public class ShowController : Show, IDisposable
{
    private int Epoch { get; set; }

    void IDisposable.Dispose()
    {
        GC.SuppressFinalize(this);
    }

    public Response Get(int showId)
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

    public Response Add(int epoch = 0)
    {
        var resp = new Response();
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
            
            if (tvmShowUpdates == null)
            {
                using var tvmShowUpdateController = new TvmShowUpdateControlller()
                {
                    TvmShowId = TvmShowId,
                    TvmUpdateEpoch = epoch,
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
            
            var validResp = Valid();
            if (!validResp.Success)
            {
                resp.Success = false;
                resp.Message = "Validation Errors";
                resp.InfoMessage = validResp.ErrorMessage;
                return resp;
            }

            CleanedShowName = Common.RemoveSpecialCharsInShowName(ShowName);
            if (TvmStatus == "Skipping" || Finder == "Skip")
            {
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
