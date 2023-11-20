namespace PlexMediaControl.Models.MariaDB;

public class Showstorefresh
{
    public int      TvmShowId    { get; set; }
    public string   TvmStatus    { get; set; } = null!;
    public DateTime PremiereDate { get; set; }
    public DateTime UpdateDate   { get; set; }
    public string   ShowName     { get; set; } = null!;
    public string?  TvmUrl       { get; set; }
    public string   ShowStatus   { get; set; } = null!;
}
