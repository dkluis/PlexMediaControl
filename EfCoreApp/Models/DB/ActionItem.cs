namespace EfCoreApp.Models.DB;

public class ActionItem
{
    public int Id { get; }
    public string Program { get; set; } = null!;
    public string Message { get; set; } = null!;
    public string UpdateDateTime { get; set; } = null!;
}