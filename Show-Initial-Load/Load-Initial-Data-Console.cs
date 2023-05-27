using Common_Lib;
using Lib_SqlDB;
using PlexMediaControl.Models.MariaDB;

var appInfo = new AppInfo("PlexMediaControl", "Refresh Data");
appInfo.LogLevel = 5;
var log = appInfo.TxtFile;
var lastUpdate = DateOnly.FromDateTime(DateTime.Now.AddDays(-3)).ToString("yyyy-MM-dd");

log.Start();

var function = "Load Base Data";
log.Write("", function);
log.Write($"Starting function: {function} ", function);
var result = Functions.LoadBaseData(appInfo);
log.Write($"Stopped: {function}", function);
if (!result.IsSuccess) Environment.Exit(01);

function = "Load TVM Show Updates";
log.Write("", function);
log.Write($"Starting function: {function} ", function);
result = Functions.LoadTvmShowUpdates(appInfo, lastUpdate);
log.Write($"Stopped: {function}", function);
if (!result.IsSuccess) Environment.Exit(02);

function = "Load Followed Updates";
log.Write("", function);
log.Write($"Starting function: {function} ", function);
result = Functions.LoadFollowed(appInfo, lastUpdate);
log.Write($"Stopped: {function}", function);
if (!result.IsSuccess) Environment.Exit(03);

function = "Load Show Updates";
log.Write("", function);
log.Write($"Starting function: {function} ", function);
result = Functions.LoadShows(appInfo, lastUpdate);
log.Write($"Stopped: {function}", function);
if (!result.IsSuccess) Environment.Exit(04);

log.Write("");
log.Stop();

internal static class Functions
{
    internal static FunctionResult LoadBaseData(AppInfo appInfo)
    {
        var log = appInfo.TxtFile;
        var result = new FunctionResult();
        var oldAppInfo = new AppInfo("TVMaze", "Read Old Data", dbConnection: "DbAlternate");
        
        log.Write("");
        log.Write("Handling Load TVM Statuses");
        result = LoadTvmStatuses(appInfo, oldAppInfo, log);
        log.Write($"Result: IsSuccess={result.IsSuccess}, Message={result.Message}, ErrorMessage={result.ErrorMessage}", "BaseData");
        if (!result.IsSuccess) return result;
        
        log.Write("");
        log.Write("Handling Load Show Statuses");
        result = LoadShowStatuses(appInfo, oldAppInfo, log);
        log.Write($"Result: IsSuccess={result.IsSuccess}, Message={result.Message}, ErrorMessage={result.ErrorMessage}", "BaseData");
        if (!result.IsSuccess) return result;
        
        log.Write("");
        log.Write("Handling Load Plex Statuses");
        result = LoadPlexStatuses(appInfo, oldAppInfo, log);
        log.Write($"Result: IsSuccess={result.IsSuccess}, Message={result.Message}, ErrorMessage={result.ErrorMessage}", "BaseData");
        if (!result.IsSuccess) return result;
        
        log.Write("");
        log.Write("Handling Load Media Types");
        result = LoadMediaTypes(appInfo, oldAppInfo, log);
        log.Write($"Result: IsSuccess={result.IsSuccess}, Message={result.Message}, ErrorMessage={result.ErrorMessage}", "BaseData");
        if (!result.IsSuccess) return result;
        
        log.Write("");
        log.Write("Handling Load Last Show Evaluated");
        result = LoadLastShowEvaluated(appInfo, oldAppInfo, log);
        log.Write($"Result: IsSuccess={result.IsSuccess}, Message={result.Message}, ErrorMessage={result.ErrorMessage}", "BaseData");
        if (!result.IsSuccess) return result;
        
        return result;
    }
    
    internal static FunctionResult LoadTvmShowUpdates(AppInfo appInfo, string lastUpdate)
    {
        var log = appInfo.TxtFile;
        var oldAppInfo = new AppInfo("TVMaze", "Read Old Data", dbConnection: "DbAlternate");
        
        var result = UpdateTvmShowsUpdates(appInfo, oldAppInfo, log, lastUpdate);
        log.Write($"Result: IsSuccess={result.IsSuccess}, Message={result.Message}, ErrorMessage={result.ErrorMessage}", "ShowUpdates");
        if (!result.IsSuccess) return result;
        
        return result;
    }
    
    internal static FunctionResult LoadFollowed(AppInfo appInfo, string lastUpdate)
    {
        var log = appInfo.TxtFile;
        var oldAppInfo = new AppInfo("TVMaze", "Read Old Data", dbConnection: "DbAlternate");
        
        var result = UpdateFollowed(appInfo, oldAppInfo, log, lastUpdate);
        log.Write($"Result: IsSuccess={result.IsSuccess}, Message={result.Message}, ErrorMessage={result.ErrorMessage}", "Followed");
        if (!result.IsSuccess) return result;
        
        return result;
    }
    
    internal static FunctionResult LoadShows(AppInfo appInfo, string lastUpdate)
    {
        var log = appInfo.TxtFile;
        var oldAppInfo = new AppInfo("TVMaze", "Read Old Data", dbConnection: "DbAlternate");
        
        var result = UpdateShows(appInfo, oldAppInfo, log, lastUpdate);
        log.Write($"Result: IsSuccess={result.IsSuccess}, Message={result.Message}, ErrorMessage={result.ErrorMessage}", "Followed");
        if (!result.IsSuccess) return result;
        
        return result;
    }

    internal static FunctionResult LoadTvmStatuses(AppInfo appInfo, AppInfo oldAppInfo, TextFileHandler log)
    {
        var result = new FunctionResult();
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
                oldDb.Close();
                var oldCount = 0;
                var countResult = GetCount(appInfo, "Select count(*) from TvmStatuses");
                if (countResult.IsSuccess) oldCount = int.Parse(countResult.Message);
                var newCount = db.TvmStatuses.GroupBy(f => f.TvmStatus1).Count();
                log.Write($"Num of Old records: {oldCount}, New Records: {newCount}", "TvmStatuses");
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
    
    internal static FunctionResult LoadShowStatuses(AppInfo appInfo, AppInfo oldAppInfo, TextFileHandler log)
    {
        var result = new FunctionResult();
        var db = new TvMaze();
        var added = 0;
        var updated = 0;
        try
        {
            using (var oldDb = new MariaDb(oldAppInfo))
            {
                var rdr = oldDb.ExecQuery($"Select * From ShowStatuses");
                while (rdr.Read())
                {
                    var test = rdr["ShowStatus"].ToString();
                    var rec = db.ShowStatuses.SingleOrDefault(t => t.ShowStatus1 == test);
                    if (rec == null)
                    {
                        var newRec = new ShowStatus()
                        {
                            ShowStatus1 = (string) rdr["ShowStatus"]
                        };
                        db.ShowStatuses.Add(newRec);
                        db.SaveChanges();
                        added++;
                    }
                }
                oldDb.Close();
                var oldCount = 0;
                var countResult = GetCount(appInfo, "Select count(*) from ShowStatuses");
                if (countResult.IsSuccess) oldCount = int.Parse(countResult.Message);
                var newCount = db.ShowStatuses.GroupBy(f => f.ShowStatus1).Count();
                log.Write($"Num of Old records: {oldCount}, New Records: {newCount}", "ShowStatuses");
            }
        }
        catch (Exception e)
        {
            log.Write($"Error Occured Exception: {e.Message}, with innerException {e.InnerException}");
            result.IsSuccess = false;
            result.ErrorMessage += $"{e.Message}: {e.InnerException}";
        }

        result.IsSuccess = true;
        result.Message = $"ShowStatuses Added: {added} and Updated: {updated}";
        return result;
    }
    
    internal static FunctionResult LoadPlexStatuses(AppInfo appInfo, AppInfo oldAppInfo, TextFileHandler log)
    {
        var result = new FunctionResult();
        var db = new TvMaze();
        var added = 0;
        var updated = 0;
        try
        {
            using (var oldDb = new MariaDb(oldAppInfo))
            {
                var rdr = oldDb.ExecQuery($"Select * From PlexStatuses");
                while (rdr.Read())
                {
                    var test = rdr["PlexStatus"].ToString();
                    var rec = db.PlexStatuses.SingleOrDefault(t => t.PlexStatus1 == test);
                    if (rec == null)
                    {
                        var newRec = new PlexStatus()
                        {
                            PlexStatus1 = (string) rdr["PlexStatus"]
                        };
                        db.PlexStatuses.Add(newRec);
                        db.SaveChanges();
                        added++;
                    }
                }
                oldDb.Close();
                var oldCount = 0;
                var countResult = GetCount(appInfo, "Select count(*) from PlexStatuses");
                if (countResult.IsSuccess) oldCount = int.Parse(countResult.Message);
                var newCount = db.PlexStatuses.GroupBy(f => f.PlexStatus1).Count();
                log.Write($"Num of Old records: {oldCount}, New Records: {newCount}", "PlexStatuses");
            }
        }
        catch (Exception e)
        {
            log.Write($"Error Occured Exception: {e.Message}, with innerException {e.InnerException}");
            result.IsSuccess = false;
            result.ErrorMessage += $"{e.Message}: {e.InnerException}";
        }

        result.IsSuccess = true;
        result.Message = $"PlexStatuses Added: {added} and Updated: {updated}";
        return result;
    }
    
    internal static FunctionResult LoadMediaTypes(AppInfo appInfo, AppInfo oldAppInfo, TextFileHandler log)
    {
        var result = new FunctionResult();
        var db = new TvMaze();
        var added = 0;
        var updated = 0;
        try
        {
            using (var oldDb = new MariaDb(oldAppInfo))
            {
                var rdr = oldDb.ExecQuery($"Select * From MediaTypes");
                while (rdr.Read())
                {
                    var test = rdr["MediaType"].ToString();
                    var rec = db.MediaTypes.SingleOrDefault(t => t.MediaType1 == test);
                    if (rec == null)
                    {
                        var newRec = new MediaType
                        {
                            MediaType1 = (string) rdr["MediaType"],
                            PlexLocation = (string) rdr["PlexLocation"],
                            AutoDelete = (string) rdr["AutoDelete"],
                        };
                        db.MediaTypes.Add(newRec);
                        db.SaveChanges();
                        added++;
                    }
                    else
                    {
                        rec.PlexLocation = (string) rdr["MediaType"];
                        rec.AutoDelete = (string) rdr["AutoDelete"];
                        db.SaveChanges();
                        updated++;
                    }
                }
                oldDb.Close();
                var oldCount = 0;
                var countResult = GetCount(appInfo, "Select count(*) from MediaTypes");
                if (countResult.IsSuccess) oldCount = int.Parse(countResult.Message);
                var newCount = db.MediaTypes.GroupBy(f => f.MediaType1).Count();
                log.Write($"Num of Old records: {oldCount}, New Records: {newCount}", "MediaTypes");
            }
        }
        catch (Exception e)
        {
            log.Write($"Error Occured Exception: {e.Message}, with innerException {e.InnerException}");
            result.IsSuccess = false;
            result.ErrorMessage += $"{e.Message}: {e.InnerException}";
        }

        result.IsSuccess = true;
        result.Message = $"PlexStatuses Added: {added} and Updated: {updated}";
        return result;
    }
    
    internal static FunctionResult LoadLastShowEvaluated(AppInfo appInfo, AppInfo oldAppInfo, TextFileHandler log)
    {
        var result = new FunctionResult();
        var db = new TvMaze();
        var added = 0;
        var updated = 0;
        try
        {
            using (var oldDb = new MariaDb(oldAppInfo))
            {
                var rdr = oldDb.ExecQuery($"Select * From LastShowEvaluated");
                while (rdr.Read())
                {
                    var test = (int) int.Parse(rdr["ShowId"].ToString()!);
                    var rec = db.LastShowEvaluateds.SingleOrDefault(r => r.Id == 1);
                    if (rec == null)
                    {
                        var newRec = new LastShowEvaluated()
                        {
                            Id = 1,
                            ShowId = test,
                        };
                        db.Add(newRec);
                        db.SaveChanges();
                        added++;
                    }
                    else
                    {
                        rec.ShowId = test;
                        db.SaveChanges();
                        updated++;
                    }
                }
                oldDb.Close();
                log.Write($"Num of Old records: {1}, New Records: {1}", "Last Updated Show");
            }
        }
        catch (Exception e)
        {
            log.Write($"Error Occured Exception: {e.Message}, with innerException {e.InnerException}");
            result.IsSuccess = false;
            result.ErrorMessage += $"{e.Message}: {e.InnerException}";
        }

        result.IsSuccess = true;
        result.Message = $"PlexStatuses Added: {added} and Updated: {updated}";
        return result;
    }

    internal static FunctionResult GetCount(AppInfo appInfo, string sql)
    {
        var mDb = new MariaDb(appInfo);
        var rdr = mDb.ExecQuery(sql);
        var oldCount = "";
        while (rdr.Read())
        {
            oldCount = rdr[0].ToString()!;
        }
 
        if (oldCount == "")
        {
            mDb.Close();
            return new FunctionResult()
            {
                IsSuccess = false,
            };
        }
        mDb.Close();

        return new FunctionResult()
        {
            IsSuccess = true,
            Message = oldCount
        };
    }
    
    internal static FunctionResult UpdateFollowed(AppInfo appInfo, AppInfo oldAppInfo, TextFileHandler log, string lastUpdate)
    {
        var result = new FunctionResult();
        var db = new TvMaze();
        var added = 0;
        var updated = 0;
        var total = 0;
        try
        {
            using (var oldDb = new MariaDb(oldAppInfo))
            {
                var rdr = oldDb.ExecQuery($"Select * From Followed WHERE UpdateDate >= '{lastUpdate}' ORDER BY TvmShowId");
                while (rdr.Read())
                {
                    total++;
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
                var oldCount = 0;
                var countResult = GetCount(appInfo, "Select count(*) from Followed");
                if (countResult.IsSuccess) oldCount = int.Parse(countResult.Message);
                var newCount = db.Followeds.GroupBy(f => f.Id).Count();
                log.Write($"Num of Old records: {oldCount}, Records Selected: {total}, New Records: {newCount}", "Followed");
                
                var allFollowed = db.Followeds.ToList();
                foreach (var followed in allFollowed)
                {
                    var rdrOld = oldDb.ExecQuery($"select * from Followed where TvmShowId = {followed.TvmShowId}");
                    if (!rdrOld.HasRows)
                    {
                        log.Write($"Deleting {followed.TvmShowId} from DB");
                        var fRec = db.Followeds.SingleOrDefault(f => f.TvmShowId == followed.TvmShowId);
                        if (fRec != null) db.Remove(fRec);
                        db.SaveChanges();
                    }
                    oldDb.Close();
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
        result.Message = $"Followed Added: {added} and Updated: {updated}";
        return result;
    }
    
    internal static FunctionResult UpdateTvmShowsUpdates(AppInfo appInfo, AppInfo oldAppInfo, TextFileHandler log, string lastUpdate)
    {
        var result = new FunctionResult();
        var db = new TvMaze();
        var added = 0;
        var updated = 0;
        var total = 0;
        try
        {
            using (var oldDb = new MariaDb(oldAppInfo))
            {
                var rdr = oldDb.ExecQuery($"Select * From TvmShowUpdates WHERE TvmUpdateDate >= '{lastUpdate}' ORDER BY TvmShowId");
                while (rdr.Read())
                {
                    total++;
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
                    else if (rec.TvmUpdateEpoch != (int) rdr["TvmUpdateEpoch"] || rec.TvmUpdateDate != Convert.ToDateTime(rdr["TvmUpdateDate"].ToString()))
                    {
                        rec.TvmUpdateEpoch = (int) rdr["TvmUpdateEpoch"];
                        rec.TvmUpdateDate = Convert.ToDateTime(rdr["TvmUpdateDate"].ToString());
                        db.SaveChanges();
                        updated++;
                    }
                }
                oldDb.Close();
                var oldCount = 0;
                var countResult = GetCount(appInfo, "Select count(*) from TvmShowUpdates");
                if (countResult.IsSuccess) oldCount = int.Parse(countResult.Message);
                var newCount = db.TvmShowUpdates.GroupBy(f => f.Id).Count();
                log.Write($"Num of Old records: {oldCount}, Records Selected: {total}, New Records: {newCount}", "TvmShowUpdates");
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
    
    internal static FunctionResult UpdateShows(AppInfo appInfo, AppInfo oldAppInfo, TextFileHandler log, string lastUpdate)
    {
        var result = new FunctionResult();
        var db = new TvMaze();
        var added = 0;
        var updated = 0;
        var total = 0;
        try
        {
            using (var oldDb = new MariaDb(oldAppInfo))
            {
                var rdr = oldDb.ExecQuery($"Select * From Shows WHERE UpdateDate >= '{lastUpdate}' ORDER BY TvmShowId");
                while (rdr.Read())
                {
                    total++;
                    var rec = db.Shows.SingleOrDefault(t => t.TvmShowId == (int) rdr["TvmShowId"]);
                    if (rec == null)
                    {
                        var showsRec = new Show
                        {
                            TvmShowId = (int) rdr["TvmShowId"], 
                            UpdateDate = Convert.ToDateTime(rdr["UpdateDate"].ToString()),
                            TvmStatus = rdr["TvmStatus"].ToString()!,
                            TvmUrl = rdr["TvmUrl"].ToString(),
                            ShowName = rdr["ShowName"].ToString()!,
                            ShowStatus = rdr["ShowStatus"].ToString()!,
                            PremiereDate = Convert.ToDateTime(rdr["PremiereDate"].ToString()),
                            Finder = rdr["Finder"].ToString()!,
                            MediaType = rdr["MediaType"].ToString()!,
                            CleanedShowName = rdr["CleanedShowName"].ToString()!,
                            AcquireShowName = rdr["AltShowName"].ToString()!,
                            PlexShowName = rdr["AltShowName"].ToString()!,
                        };
                        db.Shows.Add(showsRec);
                        db.SaveChanges();
                        added++;
                    }
                    else if (rec.UpdateDate != Convert.ToDateTime(rdr["UpdateDate"].ToString()))
                    {
                        rec.TvmShowId = (int) rdr["TvmShowId"];
                        rec.UpdateDate = Convert.ToDateTime(rdr["UpdateDate"].ToString());
                        rec.TvmStatus = rdr["TvmStatus"].ToString()!;
                        rec.TvmUrl = rdr["TvmUrl"].ToString();
                        rec.ShowName = rdr["ShowName"].ToString()!;
                        rec.ShowStatus = rdr["ShowStatus"].ToString()!;
                        rec.PremiereDate = Convert.ToDateTime(rdr["PremiereDate"].ToString());
                        rec.Finder = rdr["Finder"].ToString()!;
                        rec.MediaType = rdr["MediaType"].ToString()!;
                        rec.CleanedShowName = rdr["CleanedShowName"].ToString()!;
                        rec.AcquireShowName = rdr["AltShowName"].ToString()!;
                        rec.PlexShowName = rdr["AltShowName"].ToString()!;
                        db.SaveChanges();
                        updated++;
                    }
                }
                oldDb.Close();
                var oldCount = 0;
                var countResult = GetCount(appInfo, "Select count(*) from Shows");
                if (countResult.IsSuccess) oldCount = int.Parse(countResult.Message);
                var newCount = db.Shows.GroupBy(f => f.Id).Count();
                log.Write($"Num of Old records: {oldCount}, Records Selected: {total}, New Records: {newCount}");

                var allShows = db.Shows.ToList();
                foreach (var show in allShows)
                {
                    var rdrOld = oldDb.ExecQuery($"select * from shows where TvmShowId = {show.TvmShowId}");
                    if (!rdrOld.HasRows)
                    {
                        log.Write($"Deleting {show.TvmShowId}, {show.ShowName} from DB");
                        var fRec = db.Followeds.SingleOrDefault(f => f.TvmShowId == show.TvmShowId);
                        var eRecs = db.Episodes.Where(e => e.TvmShowId == show.TvmShowId).ToList();
                        if (fRec != null) db.Remove(fRec);
                        db.RemoveRange(eRecs);
                        db.Remove(show);
                        db.SaveChanges();
                    }
                    oldDb.Close();
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
        result.Message = $"Shows Added: {added} and Updated: {updated}";
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



