namespace PlexMediaControl.Models.TvmApis;

public class TvmShow
{
    public int Id { get; set; }
    public string Url { get; set; } = "";
    public string Name { get; set; } = "";
    public string? Type { get; set; }
    public string Status { get; set; } = "";
    public int RunTime { get; set; }
    public string? Language { get; set; }
    public int Updated { get; set; }
    public DateOnly PremiereDate { get; set; } = DateOnly.Parse("1900-01-01");
    public DateOnly EndDate { get; set; } = DateOnly.Parse("2300-01-01");
    public string? Network { get; set; }
    public string? NetworkUrl { get; set; }
    public string? Country { get; set; }
    public string? CountryCode { get; set; }
}

public class TvmEpisode
{
    public int Id { get; set; }
    public string Url { get; set; } = "";
    public string Name { get; set; } = "";
    public int Season { get; set; }
    public int Number { get; set; }
    public string Type { get; set; } = "";
    public DateOnly AirDate { get; set; }
    public TimeOnly AirTime { get; set; }
    public int RunTime { get; set; }
    public string Summary { get; set; } = "";
}