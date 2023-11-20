namespace PlexMediaControl.Models.TvmApis;

public class TvmEpisode
{
    public int      EpisodeId { get; set; }
    public string   Url       { get; set; } = "";
    public string   Name      { get; set; } = "";
    public int      Season    { get; set; }
    public int      Number    { get; set; }
    public string   Type      { get; set; } = "";
    public DateTime AirDate   { get; set; }
    public int      RunTime   { get; set; }
    public string?  Status    { get; set; }
}
