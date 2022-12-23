using EfCoreApp.Models.MariaDB;

Console.WriteLine("Hello, Entity Framework World!");

var db = new TvMazeNewDbContext();

var result = db.Shows.OrderByDescending(ai => ai.Id).ToArray();

foreach (var item in result) Console.WriteLine($"{item.TvmShowId}: {item.ShowName}: {item.Episodes.Count}: {item.ShowStatus}: {item.UpdateDate}");

Console.WriteLine("Sql Server Db Done");
//
// PM>>> Scaffold-DbContext "Server=tcp:192.168.142.152,1433;Database=TvMazeDb;User ID=sa;Password=Sandy3942" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models/DB -f
//

//PM >> Scaffold - DbContext "Server=ca-server.local;port=3306;Database=TvMazeNewDb;uid=dick;pwd=Sandy3942" Pomelo.EntityFrameworkCore.MySql - OutputDir Models / MariaDB - f

