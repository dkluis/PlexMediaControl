namespace PlexMediaControl.Models.MariaDB;

public class ShowRssFeed
{
    public int      Id         { get; set; }
    public string   ShowName   { get; set; } = null!;
    public bool?    Processed  { get; set; }
    public string   Url        { get; set; } = null!;
    public DateTime UpdateDate { get; set; }
}
