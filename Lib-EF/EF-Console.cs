using Common_Lib;
using PlexMediaControl.Models.MariaDB;

var appInfo = new AppInfo("PlexMediaControl", "EF-Console");
var log = appInfo.TxtFile;

log.Start();

var db = new TvMaze();

var totalRecords = 0;
var recCount = db.ActionItems.Count();
totalRecords += recCount;
log.Write($"Action Items {recCount} records found");

recCount = db.Episodes.Count();
totalRecords += recCount;
log.Write($"Episodes {recCount} records found");

recCount = db.Followeds.Count();
totalRecords += recCount;
log.Write($"Followed {recCount} records found");

recCount = db.LastShowEvaluateds.Count();
totalRecords += recCount;
log.Write($"Last Evaluated Show {recCount} records found");

recCount = db.MediaTypes.Count();
totalRecords += recCount;
log.Write($"Media Types {recCount} records found");

recCount = db.PlexStatuses.Count();
totalRecords += recCount;
log.Write($"Plex Statuses {recCount} records found");

recCount = db.PlexWatchedEpisodes.Count();
totalRecords += recCount;
log.Write($"Plex Watched Episodes {recCount} records found");

recCount = db.TvmShowUpdates.Count();
totalRecords += recCount;
log.Write($"Shows Updates {recCount} records found");

recCount = db.Shows.Count();
totalRecords += recCount;
log.Write($"Shows {recCount} records found");

recCount = db.ShowRssFeeds.Count();
totalRecords += recCount;
log.Write($"ShowRss Feeds {recCount} records found");

recCount = db.ShowStatuses.Count();
totalRecords += recCount;
log.Write($"Show Statuses {recCount} records found");

recCount = db.TvmStatuses.Count();
totalRecords += recCount;
log.Write($"Tvm Statuses {recCount} records found");

log.Write($"Db has {totalRecords} records");
log.Stop();
//
// PM>>> Scaffold-DbContext "Server=tcp:192.168.142.152,1433;Database=TvMazeDb;User ID=sa;Password=Sandy3942" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models/DB -f
//

// dotnet ef dbContext scaffold "Server=ca-server.local;port=3306;Database=TvMazeNewDb;uid=dick;pwd=Sandy3942" Pomelo.EntityFrameworkCore.MySql -o Models/MariaDB -f

// dotnet ef dbContext scaffold "Server=ca-server.local;port=3306;Database=TvMazeProd;uid=dick;pwd=Sandy3942" Pomelo.EntityFrameworkCore.MySql -o Models/MariaDB -f -c TvMaze --schema TvMazeProd --no-build

