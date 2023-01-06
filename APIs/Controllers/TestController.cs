using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlexMediaControl.Models.MariaDB;

namespace APIs.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    private readonly ILogger<TestController> _logger;

    public TestController(ILogger<TestController> logger)
    {
        _logger = logger;
    }

    [HttpGet("Show/Single{id:int}")]
    public ActionResult<Show> GetSingle(int id)
    {
        using var db = new TvMaze();
        var show = db.Shows.Single(s => s.Id == id);
        return show;
    }

    [HttpGet("Show/All")]
    public ActionResult<IEnumerable<Show>> GetAll()
    {
        using var db = new TvMaze();
        var shows = db.Shows.Take(25).ToArray();
        return shows;
    }
}
