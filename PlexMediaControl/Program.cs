using EfCoreApp.Models.MariaDB;

Console.WriteLine("Hello, Entity Framework World!");

var db = new TvMazeNewDbContext();

var totalRecords = 0;
var result = db.ActionItems.ToArray();
totalRecords = result.Length;
Console.WriteLine($"Action Items {result.Length} records found");

var result2 = db.Episodes.ToArray();
totalRecords += result2.Length;
Console.WriteLine($"Episodes {result2.Length} records found");

var result3 = db.Followeds.ToArray();
totalRecords += result3.Length;
Console.WriteLine($"Followed {result3.Length} records found");

var result4 = db.LastShowEvaluateds.ToArray();
totalRecords += result4.Length;
Console.WriteLine($"Followed {result4.Length} records found");

var result5 = db.MediaTypes.ToArray();
totalRecords += result5.Length;
Console.WriteLine($"Followed {result5.Length} records found");

var result1 = db.Shows.ToArray();
totalRecords += result1.Length;
Console.WriteLine($"Shows {result1.Length} records found");





Console.WriteLine($"Db has {totalRecords} records");
//
// PM>>> Scaffold-DbContext "Server=tcp:192.168.142.152,1433;Database=TvMazeDb;User ID=sa;Password=Sandy3942" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models/DB -f
//

//PM >> Scaffold - DbContext "Server=ca-server.local;port=3306;Database=TvMazeNewDb;uid=dick;pwd=Sandy3942" Pomelo.EntityFrameworkCore.MySql - OutputDir Models / MariaDB - f

