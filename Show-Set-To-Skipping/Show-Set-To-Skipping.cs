using Common_Lib;
using PlexMediaControl.Entities;
using PlexMediaControl.Models.MariaDB;

const string program = "Show-Set-To-Skipping";
var appInfo = new AppInfo("PlexMediaControl", program);
var log = appInfo.TxtFile;

log.Start();

var showId = 57680;
log.Elapsed();
using var showController = new ShowEntity(appInfo);
var response = showController.Get(showId, getEpisodes: true);
var showRec = response.ResponseObject as Show;
var epiWatched = showRec!.Episodes.Where(e => e.PlexStatus == "Acquired");
log.Elapsed();
if (showRec == null) Environment.Exit(99);
foreach (var epi in showRec.Episodes) Console.WriteLine($"{showRec.ShowName} {epi.SeasonEpisode} {epi.PlexStatus} {epi.PlexDate}");
Environment.Exit(0);
/*
showRec.TvmStatus = "Skipping";
showRec.Finder = "Skip";
showRec.UpdateDate = DateOnly.Parse("01/01/2200");
db.Shows.Update(showRec);
db.SaveChanges();

var episodeRecs = db.Episodes.Where(e => e.TvmShowId == showId).ToList();
foreach (var episodeRec in episodeRecs.Where(episodeRec => episodeRec.PlexStatus != "Watched"))
{
    db.Episodes.Remove(episodeRec);
    db.SaveChanges();
}
*/

//DeleteEpisodeFiles();

// Testing CommandLine Args

/*var clArgs = new CommandLineArgs();
if (clArgs.Success)
    foreach (var arg in clArgs.Args)
        Console.WriteLine($"{arg.ToString()}");
else
    Console.WriteLine("Nothing Found");

Console.WriteLine(clArgs.Get("-show1"));

//log.Stop();

Environment.Exit(0);*/

/*bool DeleteEpisodeFiles()
{
    var ai = new ActionItemEntity();
    var directory = GetMediaDirectory(showRec.MediaType);
    var showName = showRec.AltShowname != ""
        ? showRec.AltShowname
        : CultureInfo.CurrentCulture.TextInfo.ToTitleCase(showRec.CleanedShowName);
    if (Directory.Exists(directory))
        try
        {
            var files = Directory.GetFiles(findIn);
            foreach (var file in files)
            {
                var mediaName = file.Replace(findIn, "").Replace("/", "");
                var trashLoc = Path.Combine(_appInfo.HomeDir, "Trash", mediaName);
                log.Write($"File to Delete {mediaName} for episode {seasonEpisode}", "MediaFileHandler", 4);
                if (!file.ToLower().Contains(seasonEpisode.ToLower())) continue;
                try
                {
                    File.Move(file, trashLoc);
                    log.Write($"Delete {mediaName}, to {trashLoc}", "", 4);
                }
                catch (Exception e)
                {
                    log.Write($"Something went wrong moving {mediaName} to {trashLoc}: {e.Message}", "", 0);
                    if (ai.Fill(appInfo.Program, $"Something went wrong moving {mediaName} to {trashLoc}: {e.Message}")) ai.Add(log);
                }
            }
        }
        catch (Exception e)
        {
            log.Write($"Error on getting Files for {Path.Combine(directory, showName, seas)}: {e}");
        }
    else
        log.Write($"Directory {findIn} does not exist");

    return false;
}*/
