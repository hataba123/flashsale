namespace FlashSale.Inventory.Api.Models;

public sealed class InventoryItem
{
    public Guid Id { get; set; }

    public string ProductId { get; set; } = string.Empty;

    public int AvailableQuantity { get; set; }

    public int ReservedQuantity { get; set; }

    public DateTimeOffset CreatedAtUtc { get; set; }

    public DateTimeOffset UpdatedAtUtc { get; set; }
}
