using PlexMediaControl.Models.MariaDB;

namespace PlexMediaControl.Entities;

public class FollowedController : Followed, IDisposable 
{
    void IDisposable.Dispose()
    {
        GC.SuppressFinalize(this);
    }

    public Response Add(Followed followed)
    {
        var resp = new Response();

        return resp;
    }
}
