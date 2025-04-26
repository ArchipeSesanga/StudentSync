namespace StudentSync.Models;

public class ErrorViewModel
{
    int Error { get; set; }
    public string? RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}