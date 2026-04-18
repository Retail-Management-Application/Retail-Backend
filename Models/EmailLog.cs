namespace RetailOrdering.API.Models;

public class EmailLog
{
    public int EmailLogId { get; set; }
    public int UserId { get; set; }
    public int? OrderId { get; set; }
    public string EmailType { get; set; } = string.Empty;
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
    public string Status { get; set; } = "Sent";
    public string? ErrorMessage { get; set; }

    // Navigation
    public User User { get; set; } = null!;
    public Order? Order { get; set; }
}
