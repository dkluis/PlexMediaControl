using Common_Lib;
using PlexMediaControl.Models.MariaDB;

namespace PlexMediaControl.Entities;

public class TvmShowUpdateController : TvmShowUpdate, IDisposable
{
    void IDisposable.Dispose()
    {
        GC.SuppressFinalize(this);
    }

    public Response Add()
    {
        var resp = new Response();
        var result = Valid();
        if (!result.Success) return resp;

        try
        {
            using var db = new TvMazeNewDbContext();
            db.TvmShowUpdates.Add(this);
            db.SaveChanges();
        }
        catch (Exception e)
        {
            resp.ErrorMessage = $"Error writing DB {TvmShowId}, {e.Message} {e.InnerException}";
            return resp;
        }

        return resp;
    }
    
    public Response Get(int showId)
    {
        var resp = new Response();

        try
        {
            using var db = new TvMazeNewDbContext();
            var record = db.TvmShowUpdates.SingleOrDefault(t => t.TvmShowId == TvmShowId);
            if (record == null) { resp.ErrorMessage = $"Record for {TvmShowId} not found"; return resp; }

            resp.Success = true;
            resp.ResponseObject = record;
        }
        catch (Exception e)
        {
            resp.ErrorMessage = $"Error Reading DB {TvmShowId}, {e.Message} {e.InnerException}";
            return resp;
        }

        return resp;
    }

    public Response Update(int showId, int epoch)
    {
        var resp = new Response();


        return resp;
    }

    private Response Valid()
    {
        var resp = new Response();
        if (TvmShowId == 0) resp.ErrorMessage += "No TvmShowId Found, ";
        if (TvmUpdateEpoch == 0) resp.InfoMessage += "TvmUpdateEpoch was zero, ";
        if (TvmUpdateDate == DateOnly.Parse("0001/01/01")) TvmUpdateDate = DateOnly.Parse(DateTime.Now.ToShortDateString());
        if (resp.ErrorMessage == null) resp.Success = true;
        return resp;
    }
    
}
