using System.Security.AccessControl;
using Common_Lib;
using Lib_SqlDB;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using Newtonsoft.Json.Linq;
using PlexMediaControl.Entities;
using PlexMediaControl.Models.MariaDB;
using Web_Lib;

var appInfo = new AppInfo("PlexMediaControl", "Load Initial Data");
appInfo.LogLevel = 5;
var log = appInfo.TxtFile;

log.Start();

var function = "Load Base Data";
log.Write($"Starting function: {function} ");
var result = Functions.LoadBaseData(appInfo);
log.Write($"Stopped: {function}");
if (!result.IsSuccess) Environment.Exit(01);

function = "Load Show Updates";
log.Write($"Starting function: {function} ");
result = Functions.LoadShowUpdates(appInfo);
log.Write($"Stopped: {function}");
if (!result.IsSuccess) Environment.Exit(02);

function = "Load Show Updates";
log.Write($"Starting function: {function} ");
result = Functions.LoadFollowed(appInfo);
log.Write($"Stopped: {function}");
if (!result.IsSuccess) Environment.Exit(03);

log.Stop();

internal static class Functions
{
    internal static FunctionResult LoadBaseData(AppInfo appInfo)
    {
        var log = appInfo.TxtFile;
        var result = new FunctionResult();
        var oldAppInfo = new AppInfo("TVMaze", "Read Old Data", dbConnection: "DbAlternate");
        
        result = LoadTvmStatuses(appInfo, oldAppInfo, log, result);
        log.Write($"Result: IsSuccess={result.IsSuccess}, Message={result.Message}, ErrorMessage={result.ErrorMessage}", "TvmStatuses");
        if (!result.IsSuccess) return result;
        
        return result;
    }
    
    internal static FunctionResult LoadShowUpdates(AppInfo appInfo)
    {
        var log = appInfo.TxtFile;
        var oldAppInfo = new AppInfo("TVMaze", "Read Old Data", dbConnection: "DbAlternate");
        
        var result = LoadTvmShowUpdates(appInfo, oldAppInfo, log);
        log.Write($"Result: IsSuccess={result.IsSuccess}, Message={result.Message}, ErrorMessage={result.ErrorMessage}", "ShowUpdates");
        if (!result.IsSuccess) return result;
        
        return result;
    }
    
    internal static FunctionResult LoadFollowed(AppInfo appInfo)
    {
        var log = appInfo.TxtFile;
        var oldAppInfo = new AppInfo("TVMaze", "Read Old Data", dbConnection: "DbAlternate");
        
        var result = LoadShowsFollowed(appInfo, oldAppInfo, log);
        log.Write($"Result: IsSuccess={result.IsSuccess}, Message={result.Message}, ErrorMessage={result.ErrorMessage}", "Followed");
        if (!result.IsSuccess) return result;
        
        return result;
    }

    internal static FunctionResult LoadTvmStatuses(AppInfo appInfo, AppInfo oldAppInfo, TextFileHandler log, FunctionResult fResult)
    {
        var result = fResult;
        var db = new TvMaze();
        var added = 0;
        var updated = 0;
        try
        {
            using (var oldDb = new MariaDb(oldAppInfo))
            {
                var rdr = oldDb.ExecQuery($"Select * From TvmStatuses");
                while (rdr.Read())
                {
                    var test = rdr["TvmStatus"].ToString();
                    var rec = db.TvmStatuses.SingleOrDefault(t => t.TvmStatus1 == test);
                    if (rec == null)
                    {
                        var newRec = new TvmStatus()
                        {
                            TvmStatus1 = (string) rdr["TvmStatus"]
                        };
                        db.TvmStatuses.Add(newRec);
                        db.SaveChanges();
                        added++;
                    }
                }
                //oldDb.Close();
                var count = oldDb.ExecQuery("Select count(*) from TvmStatuses");
                var oldCount = 0;
                while (rdr.Read())
                {
                    oldCount = int.Parse(rdr[0].ToString());
                }
                var newCount = db.TvmStatuses.GroupBy(f => f.TvmStatus1).Count();
            }
        }
        catch (Exception e)
        {
            log.Write($"Error Occured Exception: {e.Message}, with innerException {e.InnerException}");
            result.IsSuccess = false;
            result.ErrorMessage += $"{e.Message}: {e.InnerException}";
        }

        result.IsSuccess = true;
        result.Message = $"TvmStatuses Added: {added} and Updated: {updated}";
        return result;
    }
    
    internal static FunctionResult LoadTvmShowUpdates(AppInfo appInfo, AppInfo oldAppInfo, TextFileHandler log)
    {
        var result = new FunctionResult();
        var db = new TvMaze();
        var added = 0;
        var updated = 0;
        try
        {
            using (var oldDb = new MariaDb(oldAppInfo))
            {
                var rdr = oldDb.ExecQuery($"Select * From Followed ORDER BY TvmShowId");
                while (rdr.Read())
                {
                    var rec = db.Followeds.SingleOrDefault(t => t.TvmShowId == (int) rdr["TvmShowId"]);
                    if (rec == null)
                    {
                        var followedRec = new Followed() 
                            {
                                TvmShowId = (int) rdr["TvmShowId"], 
                                UpdateDate = Convert.ToDateTime(rdr["UpdateDate"].ToString())
                            };
                        db.Followeds.Add(followedRec);
                        db.SaveChanges();
                        added++;
                    }
                    else if (rec.UpdateDate != Convert.ToDateTime(rdr["UpdateDate"].ToString()))
                    {
                        rec.UpdateDate = Convert.ToDateTime(rdr["UpdateDate"].ToString());
                        db.SaveChanges();
                        updated++;
                    }
                }
                oldDb.Close();
                var oldCount = oldDb.ExecQuery("Select count(*) from Followed");
                var newCount = db.Followeds.Select(f => f.Id).ToArray();
            }
        }
        catch (Exception e)
        {
            log.Write($"Error Occured Exception: {e.Message}, with innerException {e.InnerException}");
            result.IsSuccess = false;
            result.ErrorMessage += $"{e.Message}: {e.InnerException}";
            return result;
        }

        result.IsSuccess = true;
        result.Message = $"TvmShowUpdates Added: {added} and Updated: {updated}";
        return result;
    }
    
    internal static FunctionResult LoadShowsFollowed(AppInfo appInfo, AppInfo oldAppInfo, TextFileHandler log)
    {
        var result = new FunctionResult();
        var db = new TvMaze();
        var added = 0;
        var updated = 0;
        try
        {
            using (var oldDb = new MariaDb(oldAppInfo))
            {
                var rdr = oldDb.ExecQuery($"Select * From TvmShowUpdates ORDER BY TvmShowId");
                while (rdr.Read())
                {
                    var rec = db.TvmShowUpdates.SingleOrDefault(t => t.TvmShowId == (int) rdr["TvmShowId"]);
                    if (rec == null)
                    {
                        var showUpdateRec = new TvmShowUpdate() 
                        {
                            TvmShowId = (int) rdr["TvmShowId"], 
                            TvmUpdateDate = Convert.ToDateTime(rdr["TvmUpdateDate"].ToString()),
                            TvmUpdateEpoch = (int) rdr["TvmUpdateEpoch"]
                        };
                        db.TvmShowUpdates.Add(showUpdateRec);
                        db.SaveChanges();
                        added++;
                    }
                    else if (rec.TvmUpdateEpoch != (int) rdr["TvmUpdateEpoch"])
                    {
                        rec.TvmUpdateEpoch = (int) rdr["TvmUpdateEpoch"];
                        db.SaveChanges();
                        updated++;
                    }
                }
            }
        }
        catch (Exception e)
        {
            log.Write($"Error Occured Exception: {e.Message}, with innerException {e.InnerException}");
            result.IsSuccess = false;
            result.ErrorMessage += $"{e.Message}: {e.InnerException}";
            return result;
        }

        result.IsSuccess = true;
        result.Message = $"TvmShowUpdates Added: {added} and Updated: {updated}";
        return result;
    }
}


#region SomeFunction

/*internal static void FindOutWhatShowsToLoad(AppInfo appInfo)
    {
        var log = appInfo.TxtFile;
        JArray myEpisodeJArray;
        using (WebApi tvmApi = new(appInfo))
        {
            myEpisodeJArray = tvmApi.ConvertHttpToJArray(tvmApi.GetAllMyEpisodes());
        }

        var allShows = new List<int>();
        foreach (var episode in myEpisodeJArray)
        {
            using WebApi tvmApiAlt = new(appInfo);
            var epiId = 0;
            epiId = int.Parse(episode["episode_id"]!.ToString());
            //if (epiId <= 49775) continue;

            using var db = new TvMaze();
            var episodeExists = db.Episodes.SingleOrDefault(e => e.TvmEpisodeId == epiId);
            if (episodeExists != null) continue;

            var showContent = tvmApiAlt.ConvertHttpToJObject(tvmApiAlt.GetEpisode(epiId));
            var showId = int.Parse(showContent["_embedded"]!["show"]!["id"]!.ToString());
            var cont = true;

            if (allShows.Count > 0)
            {
                cont = true;
                foreach (var show in allShows)
                {
                    if (show == showId)
                    {
                        cont = false;
                        break;
                    }
                }
            }

            if (!cont) continue;

            log.Write($"Processing Show for Epi: {epiId} for Show {showContent["_embedded"]!["show"]!["id"]}");

            allShows.Add(showId);


            var showExists = db.Shows.SingleOrDefault(s => s.TvmShowId == showId);
            if (showExists == null) log.Write($"####################   Need to Add TVMaze ShowId: {showId}");
        }

        var uniqueShows = allShows.GroupBy(i => i).Select(i => i).ToList();
        appInfo.TxtFile.Write($"Counts are: All {allShows.Count} and Unique {uniqueShows.Count} ");

        log.Write($"{uniqueShows}");
    }*/

#endregion

#region Process All shows to load

/*
var allShowsToAdd = Path.Combine(appInfo.ConfigPath!, "Inputs", "ShowsToAdd.csv");
if (!File.Exists(allShowsToAdd))
{
    log.Write($"The Input File Does not Exist {allShowsToAdd}");
    log.Stop();
    Environment.Exit(0);
}

var addTheseShows = File.ReadAllLines(allShowsToAdd);
log.Write($"Found {addTheseShows.Length} records in {allShowsToAdd}");

var db = new TvMazeNewDbContext();

foreach (var show in addTheseShows)
{
    // Followed
    var showId = int.Parse(show);
    var exists = db.Followeds.SingleOrDefault(s => s.TvmShowId == showId);
    if (exists != null) { log.Write($"Show already exists in Shows {showId}");
        continue;
    }
    var newFollowRec = new Followed()
    {
        TvmShowId = showId,
        UpdateDate = DateTime.Now.Date
    };
    
    // Show
    var newShowRec = new Show() { };
    using var tvmApi = new WebApi(appInfo);
    var showContent = tvmApi.ConvertHttpToJObject(tvmApi.GetShow(showId));
    newShowRec.TvmShowId = int.Parse(showContent["id"]!.ToString());
    newShowRec.TvmStatus = "Skipping";
    newShowRec.TvmUrl = showContent["url"]!.ToString();
    newShowRec.ShowName = showContent["name"]!.ToString();
    newShowRec.ShowStatus = showContent["status"]!.ToString();
    newShowRec.Finder = "Skip";
    newShowRec.MediaType = "TS";
    newShowRec.CleanedShowName = Common.RemoveSuffixFromShowName(Common.RemoveSpecialCharsInShowName(newShowRec.ShowName));
    if (showContent["premiered"]!.HasValues)
    {
        newShowRec.PremiereDate = DateOnly.Parse(showContent["premiered"]!.ToString());
    }
    else
    {
        newShowRec.PremiereDate = DateOnly.Parse("01/01/1900");
    }
    newShowRec.UpdateDate = DateOnly.Parse("01/01/2200");
    
    // TvmShowUpdates
    var tvmShowUpdateExist = db.TvmShowUpdates.SingleOrDefault(t => t.TvmShowId == showId) != null;
    var newTvmShowUpdatesRec = new TvmShowUpdate()
    {
        TvmShowId = showId,
        TvmUpdateEpoch = int.Parse(showContent["updated"]!.ToString()),
        TvmUpdateDate = newShowRec.PremiereDate
    };
    
    // TVMaze 
    using var tvmApiUpd = new WebApi(appInfo);
    var updateResult = tvmApiUpd.PutShowToFollowed(showId);
    if (!updateResult.IsSuccessStatusCode)
    {
        log.Write($"Error Occured trying to update Tvmaze to followed for {showId} {newShowRec.ShowName} with error: {updateResult.StatusCode}");
        continue;
    }
    
    // Do all DB Updates in 1 transaction
    using var transaction = db.Database.BeginTransaction();
    try
    {
        db.Followeds.Add(newFollowRec);
        db.Shows.Add(newShowRec);
        if (!tvmShowUpdateExist) db.TvmShowUpdates.Add(newTvmShowUpdatesRec);

        db.SaveChanges();
        transaction.Commit();
    }
    catch (Exception e)
    {
        transaction.Rollback();
        log.Write($"Transaction Failed for ShowId {showId} with error {e.Message} {e.InnerException}");
    }

}
*/

#endregion

#region Add the Episodes

/*
JArray myEpisodeJArray;
using (WebApi tvmApi = new(appInfo))
{
    myEpisodeJArray = tvmApi.ConvertHttpToJArray(tvmApi.GetAllMyEpisodes());
};

foreach (var episode in myEpisodeJArray)
{
    using WebApi tvmApiAlt = new(appInfo);
    var epiId = int.Parse(episode["episode_id"]!.ToString());
    var type = int.Parse(episode["type"]!.ToString());
    var marked_at = int.Parse(episode["marked_at"]!.ToString());

    using var db = new TvMazeNewDbContext();
    var episodeExists = db.Episodes.SingleOrDefault(e => e.TvmEpisodeId == epiId);
    if (episodeExists != null) continue;

    var episodeContent = tvmApiAlt.ConvertHttpToJObject(tvmApiAlt.GetEpisode(epiId));
    var showId = int.Parse(episodeContent["_embedded"]!["show"]!["id"]!.ToString());

    var showRec = db.Shows.SingleOrDefault(s => s.TvmShowId == showId);
    if (showRec == null)
    {
        log.Write($"####################   Need to Add TVMaze ShowId: {showId}", "", 0);
        continue;
    }

    if (showRec.TvmStatus != "Skipping") continue;

    var url = episodeContent["url"] != null ? episodeContent["url"]!.ToString() : "";
    var season = episodeContent["season"] != null ? int.Parse(episodeContent["season"]!.ToString()) : 0;
    var number = 999;
    try
    {
        number = episodeContent["number"] != null ? int.Parse(episodeContent["number"]!.ToString()) : 0;
    }
    catch (Exception e)
    {
        log.Write($"Exception trying to get the Number for {epiId} of {showId} {url} {e.Message}");
    }
    var airdate = DateOnly.Parse("01/01/1900");
    try
    {
        airdate = episodeContent["airdate"] != null  ? DateOnly.Parse(episodeContent["airdate"]!.ToString()) : DateOnly.Parse("01/01/1900");
    }
    catch (Exception e)
    {
        log.Write($"Exception trying to get the AirDate for {epiId} of {showId} {url} {e.Message}");
    }
    
    var plexDate = DateOnly.Parse(Common.ConvertEpochToDate(marked_at));
    var plexStatus = type switch
    {
        0 => "Watched",
        1 => "Acquired",
        2 => "Skipped",
        _ => ""
    };

    var episodeRec = new Episode()
    {
        TvmShowId = showId,
        TvmEpisodeId = epiId,
        PlexStatus = plexStatus,
        TvmUrl = url,
        BroadcastDate = airdate,
        Season = season,
        Episode1 = number,
        PlexDate = plexDate,
        UpdateDate = plexDate,
        SeasonEpisode = $"S{season:00}E{number:00}"
    };

    if (season == 0 && number == 0 && airdate == DateOnly.Parse("01/01/1900"))
    {
        log.Write($"Something wrong with Episode {showId} {epiId} {url}");
    }
    else
    {
        db.Episodes.Add(episodeRec);
        db.SaveChanges();
        log.Write($"Adding Episode {episodeRec.TvmShowId} {episodeRec.SeasonEpisode} {episodeRec.TvmUrl}");
    }
    
}
*/

#endregion

#region Cleanup unwanted episodes because Show is Skipping and Episode is not recorded or recorded as Skipped

/*
using var db = new TvMazeNewDbContext();
var episodesToDelete = db.Episodesfullinfos.Where(s => s.TvmStatus == "Skipping" && s.PlexStatus != "Watched").OrderBy(s => s.TvmEpisodeId).ToArray();
foreach (var rec in episodesToDelete)
{
    var delRec = db.Episodes.SingleOrDefault(e => e.TvmEpisodeId == rec.TvmEpisodeId);
    if (delRec != null)
    {
        log.Write($"Deleting {rec.ShowName} {rec.SeasonEpisode} {rec.PlexStatus} {rec.PlexDate} {rec.TvmEpisodeId}");
        db.Episodes.Remove(delRec);
        db.SaveChanges();
    }
}
*/

#endregion

#region Unfollow and Delete Skipping Shows with no Episodes

/*
using var db = new TvMazeNewDbContext();
var showsToUnfollow = db.Showepisodecounts.ToArray();

var tvmApi = new WebApi(appInfo);
foreach (var show in showsToUnfollow)
{
    var showRec = db.Shows.Single(s => s.TvmShowId == show.ShowsTvmShowId);
    db.Shows.Remove(showRec);
    var followed = db.Followeds.Single(f => f.TvmShowId == show.ShowsTvmShowId);
    db.Followeds.Remove(followed);
    db.SaveChanges();
    tvmApi.PutShowToUnfollowed(show.ShowsTvmShowId);
}
*/

#endregion

#region Try out some tests

// var result = showCont.GetShow("Secrets & L");
// log.Write($"Result s: {result.Success} m: {result.Message} i: {result.InfoMessage} e: {result.ErrorMessage}");
// var showList = result.ResponseObject as List<Show>;
// var premiereDate = DateOnly.Parse("03/01/2015");
// //var showFound = showList.Where(s => s.PremiereDate == premiereDate);
// if (showList != null)
//     foreach (var show in showList)
//        log.Write($"Found {show.TvmShowId} {show.ShowName}, {show.AcquireShowname}, {show.PlexShowname} {show.CleanedShowName} {show.ShowStatus} {show.PremiereDate} {show.UpdateDate}");

// using var showController = new ShowEntity(appInfo)
// {
//     TvmShowId = 83,
//     TvmStatus = "Skipping"
// };
//
// var response = showController.Add();
//
// log.Write(response.Success ? $"Show Created: {showController.ShowName} {response.Message} {response.InfoMessage}" : $"Show NOT created {showController.ShowName} {response.Message}, {response.InfoMessage} {response.ErrorMessage}");

#endregion



