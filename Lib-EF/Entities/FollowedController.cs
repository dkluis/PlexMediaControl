using PlexMediaControl.Models.MariaDB;

namespace PlexMediaControl.Entities;

public class FollowedController : Followed, IDisposable 
{
    void IDisposable.Dispose()
    {
        GC.SuppressFinalize(this);
    }

    public Response Add()
    {
        var resp = new Response();

        return resp;
    }
}
