using EfCoreApp.Models.DB;

Console.WriteLine("Hello, Entity Framework World!");

var db = new TvMazeDbContext();

var result = db.ActionItems.OrderByDescending(ai => ai.UpdateDateTime).ToArray();

foreach (var item in result) Console.WriteLine($"{item.Id}: {item.Program}: {item.Message}: {item.UpdateDateTime}"); 

//
// PM>>> Scaffold-DbContext "Server=tcp:192.168.142.152,1433;Database=TvMazeDb;User ID=sa;Password=Sandy3942" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models/DB
//