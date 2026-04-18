namespace RetailOrdering.API.Models;

public class Inventory
{
    public int InventoryId { get; set; }
    public int ProductId { get; set; }
    public int QuantityInStock { get; set; }
    public int ReorderLevel { get; set; } = 10;
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

    // Navigation
    public Product Product { get; set; } = null!;
}
