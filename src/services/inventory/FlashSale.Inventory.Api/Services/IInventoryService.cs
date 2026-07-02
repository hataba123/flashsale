using FlashSale.Inventory.Api.Contracts;

namespace FlashSale.Inventory.Api.Services;

public interface IInventoryService
{
    Task<InventoryResponse?> GetByProductIdAsync(
        string productId,
        CancellationToken cancellationToken = default);

    Task<InventoryAdjustmentResult> AdjustAsync(
        string productId,
        int quantityChange,
        CancellationToken cancellationToken = default);
}
