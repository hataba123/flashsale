using FlashSale.Inventory.Api.Contracts;

namespace FlashSale.Inventory.Api.Services;

public sealed record InventoryAdjustmentResult(
    InventoryAdjustmentStatus Status,
    InventoryResponse? Inventory = null);

public enum InventoryAdjustmentStatus
{
    Success,
    InvalidQuantityChange,
    InsufficientInventory
}
