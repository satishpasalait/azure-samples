namespace Azure.Samples.EventGrid.Subscriber.Models;

public class OrderEvent
{
    public int OrderId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public int ItemCount { get; set; }
    public DateTime OrderDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Notes { get; set; }
}


