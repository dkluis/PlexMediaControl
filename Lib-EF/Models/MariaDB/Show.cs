namespace PlexMediaControl.Models.MariaDB;

public class Show
{
    public         int                  Id                   { get; set; }
    public         int                  TvmShowId            { get; set; }
    public         string               TvmStatus            { get; set; } = null!;
    public         string?              TvmUrl               { get; set; }
    public         string               ShowName             { get; set; } = null!;
    public         string               ShowStatus           { get; set; } = null!;
    public         DateTime             PremiereDate         { get; set; }
    public         string               Finder               { get; set; } = null!;
    public         string?              MediaType            { get; set; }
    public         string               CleanedShowName      { get; set; } = null!;
    public         string               AcquireShowName      { get; set; } = null!;
    public         DateTime             UpdateDate           { get; set; }
    public         string               PlexShowName         { get; set; } = null!;
    public virtual MediaType?           MediaTypeNavigation  { get; set; }
    public virtual ShowStatus           ShowStatusNavigation { get; set; } = null!;
    public virtual TvmShowUpdate        TvmShow              { get; set; } = null!;
    public virtual TvmStatus            TvmStatusNavigation  { get; set; } = null!;
    public virtual ICollection<Episode> Episodes             { get; set; } = new HashSet<Episode>();
}
