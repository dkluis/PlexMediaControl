using PlexMediaControl.Models.MariaDB;

Console.WriteLine("Hello, Entity Framework World!");

var db = new TvMazeNewDbContext();

var totalRecords = 0;
var recCount= db.ActionItems.Count();
totalRecords += recCount;
Console.WriteLine($"Action Items {recCount} records found");

var result2 = db.Episodes.ToArray();
totalRecords += result2.Length;
Console.WriteLine($"Episodes {result2.Length} records found");

var result3 = db.Followeds.ToArray();
totalRecords += result3.Length;
Console.WriteLine($"Followed {result3.Length} records found");

var result4 = db.LastShowEvaluateds.ToArray();
totalRecords += result4.Length;
Console.WriteLine($"Last Evaluated Show {result4.Length} records found");

var result5 = db.MediaTypes.ToArray();
totalRecords += result5.Length;
Console.WriteLine($"Media Types {result5.Length} records found");

var result6 = db.PlexStatuses.ToArray();
totalRecords += result6.Length;
Console.WriteLine($"Plex Statuses {result6.Length} records found");

var result7 = db.PlexWatchedEpisodes.ToArray();
totalRecords += result7.Length;
Console.WriteLine($"Plex Watched Episodes {result7.Length} records found");

var result1 = db.Shows.ToArray();
totalRecords += result1.Length;
Console.WriteLine($"Shows {result1.Length} records found");

var result8 = db.ShowRssFeeds.ToArray();
totalRecords += result8.Length;
Console.WriteLine($"ShowRss Feeds {result8.Length} records found");

var result9 = db.ShowStatuses.ToArray();
totalRecords += result9.Length;
Console.WriteLine($"Show Statuses {result9.Length} records found");

var result10 = db.TvmStatuses.ToArray();
totalRecords += result10.Length;
Console.WriteLine($"Tvm Statuses {result10.Length} records found");

Console.WriteLine($"Db has {totalRecords} records");
//
// PM>>> Scaffold-DbContext "Server=tcp:192.168.142.152,1433;Database=TvMazeDb;User ID=sa;Password=Sandy3942" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models/DB -f
//

// dotnet ef dbContext scaffold "Server=ca-server.local;port=3306;Database=TvMazeNewDb;uid=dick;pwd=Sandy3942" Pomelo.EntityFrameworkCore.MySql -o Models/MariaDB -f


