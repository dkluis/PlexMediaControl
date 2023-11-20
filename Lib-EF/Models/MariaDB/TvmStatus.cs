namespace PlexMediaControl.Models.MariaDB;

public class TvmStatus
{
    public TvmStatus()
    {
        Shows = new HashSet<Show>();
    }
    public         string            TvmStatus1 { get; set; } = null!;
    public virtual ICollection<Show> Shows      { get; set; }
}
