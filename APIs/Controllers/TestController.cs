using Microsoft.AspNetCore.Mvc;
using PlexMediaControl.Models.MariaDB;

namespace APIs.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    private static readonly string[] Summaries =
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
            TvmStatus = "Status",
            TvmUrl = "Url",
            ShowName = "ShowName",
            ShowStatus = "ShowStatus",
            PremiereDate = DateOnly.Parse("2023-01-01"),
            Finder = "Finder",
            MediaType = "MediaType",
            CleanedShowName = "Clean",
            AltShowname = "Alt",
            UpdateDate = DateOnly.Parse("2023-01-01")
        };
        // return new ActionItem
        // {
        //     Id = 0,
        //     Program = "Program",
        //     Message = "Message",
        //     UpdateDateTime = DateTime.Now,
        // };
    }
}
