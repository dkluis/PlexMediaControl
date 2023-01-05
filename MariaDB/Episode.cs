﻿namespace PlexMediaControl.Models.MariaDB;

public class Episode
{
    public int Id { get; set; }
    public int TvmShowId { get; set; }
    public int TvmEpisodeId { get; set; }
    public string TvmUrl { get; set; } = null!;
    public string SeasonEpisode { get; set; } = null!;
    public int Season { get; set; }
    public int Episode1 { get; set; }
    public DateOnly? BroadcastDate { get; set; }
    public string PlexStatus { get; set; } = null!;
    public DateOnly? PlexDate { get; set; }
    public DateOnly UpdateDate { get; set; }

    public virtual PlexStatus PlexStatusNavigation { get; set; } = null!;
    public virtual Show TvmShow { get; set; } = null!;
}
