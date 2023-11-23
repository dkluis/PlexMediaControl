namespace PlexMediaControl.Models.TvmApis;

public class TvmShow
{
    public int      Id           { get; set; }
    public string   Url          { get; set; } = "";
    public string   Name         { get; set; } = "";
    public string?  Type         { get; set; }
    public string   Status       { get; set; } = "";
    public int      RunTime      { get; set; }
    public string?  Language     { get; set; }
    public long     Updated      { get; set; }
    public DateTime PremiereDate { get; set; } = new(1900, 01, 01);
    public DateTime EndDate      { get; set; } = new(2300, 01, 01);
    public string?  Network      { get; set; }
    public string?  NetworkUrl   { get; set; }
    public string?  Country      { get; set; }
    public string?  CountryCode  { get; set; }
}
