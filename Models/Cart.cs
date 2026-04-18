namespace RetailOrdering.API.Models;

public class Cart
{
    public int CartId { get; set; }
    public int UserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public User User { get; set; } = null!;
    public ICollection<CartItem> CartItems { get; set; } = [];
}
