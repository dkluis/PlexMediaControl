using PlexMediaControl.Models.MariaDB;

namespace PlexMediaControl.Entities;

public class FollowedEntity : Followed, IDisposable
{
    void IDisposable.Dispose()
    {
        GC.SuppressFinalize(this);
    }

    public Response Add()
    {
        var resp = new Response();
        UpdateDate = DateTime.Now;
        var validResp = Validate();
        if (!validResp.Success)
        {
            resp.InfoMessage = "Validation Followed Record fields";
            resp.ErrorMessage = validResp.ErrorMessage;
            return resp;
        }

        try
        {
            using var db = new TvMaze();
            db.Add(this);
            db.SaveChanges();
        }
        catch (Exception e)
        {
            resp.ErrorMessage = $"Error with Adding Followed Record for {TvmShowId} {e.Message} {e.InnerException}";
            return resp;
        }

        resp.Success = true;
        return resp;
    }

    private Response Validate()
    {
        var resp = new Response();
        if (TvmShowId == 0) resp.ErrorMessage += "TvmShowId is not set";
        if (string.IsNullOrEmpty(resp.ErrorMessage)) resp.Success = true;

        return resp;
    }
}
