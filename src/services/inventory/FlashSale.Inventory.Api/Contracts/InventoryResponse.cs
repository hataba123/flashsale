namespace FlashSale.Inventory.Api.Contracts;

public sealed record InventoryResponse(
    string ProductId,
    int AvailableQuantity,
    int ReservedQuantity,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset UpdatedAtUtc);
