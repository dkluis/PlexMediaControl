namespace PlexMediaControl.Models.MariaDB;

public class Followed
{
    public int Id { get; set; }
    public int TvmShowId { get; set; }
    public DateOnly UpdateDate { get; set; }
}
