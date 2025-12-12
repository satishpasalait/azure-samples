namespace Azure.Samples.EventGrid.Publisher.Models;

public class UserEvent
{
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty; // Created, Updated, Deleted
    public DateTime Timestamp { get; set; }
}

