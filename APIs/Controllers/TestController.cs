using Microsoft.AspNetCore.Mvc;
using PlexMediaControl.Models.MariaDB;

namespace APIs.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<TestController> _logger;

    public TestController(ILogger<TestController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetTest")]
    public Show Get()
    {
        //using var db = new TvMazeNewDbContext();
        //var show = db.Shows.Single(s => s.Id == 1);
        //return show;
        return new Show
        {
            Id = 0,
            TvmShowId = 0,
            TvmStatus = null,
            TvmUrl = null,
            ShowName = null,
            ShowStatus = null,
            PremiereDate = default,
            Finder = null,
            MediaType = null,
            CleanedShowName = null,
            AltShowname = null,
            UpdateDate = default,
            MediaTypeNavigation = null,
            ShowStatusNavigation = null,
            TvmShow = null,
            TvmStatusNavigation = null,
            Episodes = null
        };
    }
}
