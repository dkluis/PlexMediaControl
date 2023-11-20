namespace PlexMediaControl.Models.MariaDB;

public class PlexWatchedEpisode
{
    public int      Id                { get; set; }
    public int?     TvmShowId         { get; set; }
    public int?     TvmEpisodeId      { get; set; }
    public string   PlexShowName      { get; set; } = null!;
    public int      PlexSeasonNum     { get; set; }
    public int      PlexEpisodeNum    { get; set; }
    public string   PlexSeasonEpisode { get; set; } = null!;
    public DateTime PlexWatchedDate   { get; set; }
    public bool     ProcessedToTvmaze { get; set; }
    public DateTime UpdateDate        { get; set; }
}
