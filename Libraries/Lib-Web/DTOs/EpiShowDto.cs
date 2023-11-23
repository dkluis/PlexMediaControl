using System;
using System.Text.Json.Serialization;
using static Web_Lib.DTOs.TvmazeGenericDto;

namespace Web_Lib.DTOs;

public class EpiShowDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("url")]
    public string? Url { get; set; }
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    [JsonPropertyName("season")]
    public int Season { get; set; }
    [JsonPropertyName("number")]
    public int Number { get; set; }
    [JsonPropertyName("type")]
    public string? Type { get; set; }
    [JsonPropertyName("airdate")]
    public DateTime Airdate { get; set; }
    [JsonPropertyName("airtime")]
    public string? Airtime { get; set; }
    [JsonPropertyName("airstamp")]
    public DateTime AirStamp { get; set; }
    [JsonPropertyName("runtime")]
    public int Runtime { get; set; }
    [JsonPropertyName("rating")]
    public Rating? Rating { get; set; }
    [JsonPropertyName("image")]
    public Image? Image { get; set; }
    [JsonPropertyName("summary")]
    public string? Summary { get; set; }
    [JsonPropertyName("links")]
    public Links? Links { get; set; }
    [JsonPropertyName("embedded")]
    public Embedded? Embedded { get; set; }
}
public class Embedded
{
    [JsonPropertyName("show")]
    public ShowDto? Show { get; set; }
}
/*
public class Show
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("url")]
    public string? Url { get; set; }
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    [JsonPropertyName("type")]
    public string? Type { get; set; }
    [JsonPropertyName("language")]
    public string? Language { get; set; }
    [JsonPropertyName("genres")]
    public string[]? Genres { get; set; }
    [JsonPropertyName("status")]
    public string? Status { get; set; }
    [JsonPropertyName("runtime")]
    public int? Runtime { get; set; }
    [JsonPropertyName("averageRuntime")]
    public int AverageRuntime { get; set; }
    [JsonPropertyName("premiered")]
    public DateTime Premiered { get; set; }
    [JsonPropertyName("ended")]
    public string? Ended { get; set; }
    [JsonPropertyName("officialSite")]
    public string? OfficialSite { get; set; }
    [JsonPropertyName("schedule")]
    public Schedule? Schedule { get; set; }
    [JsonPropertyName("rating")]
    public Rating? Rating { get; set; }
    [JsonPropertyName("weight")]
    public int Weight { get; set; }
    [JsonPropertyName("webChannel")]
    public WebChannel? WebChannel { get; set; }
    [JsonPropertyName("externals")]
    public Externals? Externals { get; set; }
    [JsonPropertyName("image")]
    public Image? Image { get; set; }
    [JsonPropertyName("summary")]
    public string? Summary { get; set; }
    [JsonPropertyName("updated")]
    public long? Updated { get; set; }
    [JsonPropertyName("links")]
    public Links? Links { get; set; }
}
*/
