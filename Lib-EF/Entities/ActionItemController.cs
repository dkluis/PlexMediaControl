using PlexMediaControl.Models.MariaDB;

namespace PlexMediaControl.Entities;

public abstract class ActionItemController : IDisposable
{
    void IDisposable.Dispose()
    {
        GC.SuppressFinalize(this);
    }

    public static Response Record(ActionItem record)
    {
        var resp = new Response();
        if (!Validate(record))
        {
            resp.ErrorMessage = "Message or Program or both were blank";
            return resp;
        }

        try
        {
            using var db = new TvMazeNewDbContext();
            db.ActionItems.Add(record);
            db.SaveChanges();
        }
        catch (Exception e)
        {
            resp.ErrorMessage = $"Error Occured {e.Message} {e.InnerException}";
            return resp;
        }

        resp.Success = true;
        return resp;
    }

    private static bool Validate(ActionItem check)
    {
        var result = true;
        if (string.IsNullOrEmpty(check.Message))
            result = false;
        else if (string.IsNullOrEmpty(check.Program)) result = false;

        return result;
    }
}
