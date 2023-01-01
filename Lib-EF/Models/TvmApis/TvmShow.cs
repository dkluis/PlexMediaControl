namespace PlexMediaControl.Models.TvmApis;

public class TvmShow
{
    public int Id { get; set; }
    public string Url { get; set; } = "";
    public string Name { get; set; } = "";
    public string? Language { get; set; }
    public int Updated { get; set; }
    public string Status { get; set; } = "";
    public DateOnly PremiereDate { get; set; } = DateOnly.Parse("1900-01-01");
}