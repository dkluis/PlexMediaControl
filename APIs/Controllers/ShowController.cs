using Common_Lib;

using Microsoft.AspNetCore.Mvc;

using PlexMediaControl.Entities;
using PlexMediaControl.Models.MariaDB;

namespace APIs.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShowController : ControllerBase
{
    private readonly ILogger<ShowController> _logger;

    public ShowController(ILogger<ShowController> logger)
    {
        _logger = logger;
    }

    [HttpGet("Get/{id:int}")]
    public ActionResult<ShowEntity> Get(int id)
    {
        using var showEntity = new ShowEntity(new AppInfo("PlexMediaControl", "APIs"));
        showEntity.Get(id);

        return showEntity;
    }

    [HttpGet("Get/{id:int}/Tvm")]
    public ActionResult<ShowEntity> GetTvm(int id)
    {
        using var showEntity = new ShowEntity(new AppInfo("PlexMediaControl", "APIs"));
        showEntity.Get(id, true);

        return showEntity;
    }

    [HttpGet("Get/{id:int}/Episodes")]
    public ActionResult<ShowEntity> GetEpisode(int id)
    {
        using var showEntity = new ShowEntity(new AppInfo("PlexMediaControl", "APIs"));
        showEntity.Get(id, getEpisodes: true);

        return showEntity;
    }

    [HttpGet("GetAll")]
    public ActionResult<IEnumerable<Show>> GetAll()
    {
        using var db    = new TvMaze();
        var       shows = db.Shows.ToArray();

        return shows;
    }
}
