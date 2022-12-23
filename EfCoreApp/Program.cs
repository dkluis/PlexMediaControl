using EfCoreApp.Models.MariaDB;

Console.WriteLine("Hello, Entity Framework World!");

var db = new TvMazeNewDbContext();

var result = db.ActionItems.OrderByDescending(ai => ai.UpdateDateTime).ToArray();

foreach (var item in result) Console.WriteLine($"{item.Id}: {item.Program}: {item.Message}: {item.UpdateDateTime}");

Console.WriteLine("Sql Server Db Done");
//
// PM>>> Scaffold-DbContext "Server=tcp:192.168.142.152,1433;Database=TvMazeDb;User ID=sa;Password=Sandy3942" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models/DB -f
//

//PM >> Scaffold - DbContext "Server=ca-server.local;port=3306;Database=TvMazeNewDb;uid=dick;pwd=Sandy3942" Pomelo.EntityFrameworkCore.MySql - OutputDir Models / MariaDB - f

