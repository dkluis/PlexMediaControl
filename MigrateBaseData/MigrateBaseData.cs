using Common_Lib;
using PlexMediaControl.Models.MariaDB;

var appInfo = new AppInfo("PlexMediaControl", "MigrateBaseData");
var log = appInfo.TxtFile;

log.Start();

var ToDb = new TvMaze();
