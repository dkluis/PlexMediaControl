namespace PlexMediaControl.Models.TvmApis;

public class TvmEpisode
{
    public int Id { get; set; }
    public string Url { get; set; } = "";
    public string Name { get; set; } = "";
    public int Season { get; set; }
    public int Number { get; set; }
    public string Type { get; set; } = "";
    public DateOnly AirDate { get; set; }
    public TimeOnly AirTime { get; set; }
    public int RunTime { get; set; }
    public string Summary { get; set; } = "";
}
