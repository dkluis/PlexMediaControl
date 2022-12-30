namespace PlexMediaControl.Models.MariaDB;

public class Showepisodecount
{
    public int ShowsTvmShowId { get; set; }
    public string ShowName { get; set; } = null!;
    public string Status { get; set; } = null!;
    public string? Url { get; set; }
    public int? EpisodeCount { get; set; }
}
