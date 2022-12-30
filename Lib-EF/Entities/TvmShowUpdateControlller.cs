using PlexMediaControl.Models.MariaDB;

namespace PlexMediaControl.Entities;

public class TvmShowUpdateControlller : TvmShowUpdate, IDisposable
{
    void IDisposable.Dispose()
    {
        GC.SuppressFinalize(this);
    }

    public Response Add()
    {
        var resp = new Response();
        var result = Valid();
        
        return resp;
    }
    
    public Response Get(int showId)
    {
        var resp = new Response();

        return resp;
    }

    public Response Update(int showId, int epoch)
    {
        var resp = new Response();
        
        return resp;
    }

    private Response Valid()
    {
        var resp = new Response
        {
            Success = false,
            Message = "",
            InfoMessage = "",
            ErrorMessage = "",
        };
        return resp;
    }


    
}
