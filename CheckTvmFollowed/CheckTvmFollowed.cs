using Common_Lib;
using Newtonsoft.Json.Linq;
using PlexMediaControl.Entities;
using PlexMediaControl.Models.MariaDB;
using Web_Lib;

var commandLineArgs = new CommandLineArgs("CheckTvmFollowed");
if (!commandLineArgs.Success) Environment.Exit(99);
var logLevel = commandLineArgs.GetLogLevel();

const string program = "Check TvM Followed";
var appInfo = logLevel > 8 ? new AppInfo("PlexMediaControl", program) : new AppInfo("PlexMediaControl", program, logLevel: logLevel);
var log = appInfo.TxtFile;

log.Start();

using var tvmAPI = new WebApi(appInfo);
using var db = new TvMaze();
var tvmJArray = tvmAPI.GetFollowedShows();
var tvmFollowed = ((JArray) tvmJArray).ToList();
var followed = db.Shows.Where(s => s.TvmStatus == "Following" || s.TvmStatus == "Skipping").ToArray();

if (tvmFollowed.Count == followed.Length)
{
    log.Write($"Nothing to do TvMaze Follows: {tvmFollowed.Count} : Plex Follows: {followed.Length}");
}
else
{
    log.Write($"Processing to do TvMaze Follows: {tvmFollowed.Count} : Plex Follows: {followed.Length}");
    if (tvmFollowed.Count > followed.Length)
    {
        foreach (var tvmShow in tvmFollowed.Where(tvmShow => followed.SingleOrDefault(s => s.TvmShowId == int.Parse(tvmShow["show_id"]!.ToString())) is null))
        {
            log.Write($"Have to work this show {int.Parse(tvmShow["show_id"]!.ToString())} does not exist on Plex");

            using var showEntity = new ShowEntity(appInfo);
            showEntity.TvmShowId = int.Parse(tvmShow["show_id"]!.ToString());
            var result = showEntity.Add();
            if (!result.Success) log.Write($"Error Adding the Show: {showEntity.TvmShowId} {result.Message} {result.InfoMessage} {result.ErrorMessage}");
        }
    }
    else
    {
        foreach (var show in followed)
        {
            if (tvmFollowed.Exists(t => t["show_id"]!.Value<int>() == show.TvmShowId)) continue;
            log.Write($"Have to work this show {show.TvmShowId} {show.ShowName} does not exist on Plex as Followed anymore");
            using var showEntity = new ShowEntity(appInfo);
            showEntity.TvmShowId = show.TvmShowId;
            var result = showEntity.Delete();
            if (!result.Success) log.Write($"Error Deleting the Show: {showEntity.TvmShowId} {result.Message} {result.InfoMessage} {result.ErrorMessage}");
        }
    }
}

log.Elapsed();
log.Stop();

