namespace PlexMediaControl.Entities;

public class Response
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public string? InfoMessage { get; set; }
    public string? ErrorMessage { get; set; }
    public object? ResponseObject { get; set; }
}
