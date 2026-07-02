using FlashSale.Inventory.Api.Contracts;
using FlashSale.Inventory.Api.Data;
using FlashSale.Inventory.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace FlashSale.Inventory.Api.Services;

public sealed class InventoryService : IInventoryService
{
    private readonly InventoryDbContext _dbContext;

    public InventoryService(InventoryDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<InventoryResponse?> GetByProductIdAsync(
        string productId,
        CancellationToken cancellationToken = default)
    {
        var normalizedProductId = productId.Trim();

        var inventoryItem = await _dbContext.InventoryItems
            .AsNoTracking()
            .SingleOrDefaultAsync(
                item => item.ProductId == normalizedProductId,
                cancellationToken);

        return inventoryItem is null
            ? null
            : MapToResponse(inventoryItem);
    }

    public async Task<InventoryAdjustmentResult> AdjustAsync(
        string productId,
        int quantityChange,
        CancellationToken cancellationToken = default)
    {
        if (quantityChange == 0)
        {
            return new InventoryAdjustmentResult(
                InventoryAdjustmentStatus.InvalidQuantityChange);
        }

        var normalizedProductId = productId.Trim();

        var inventoryItem = await _dbContext.InventoryItems
            .SingleOrDefaultAsync(
                item => item.ProductId == normalizedProductId,
                cancellationToken);

        var nowUtc = DateTimeOffset.UtcNow;

        if (inventoryItem is null)
        {
            if (quantityChange < 0)
            {
                return new InventoryAdjustmentResult(
                    InventoryAdjustmentStatus.InsufficientInventory);
            }

            inventoryItem = new InventoryItem
            {
                Id = Guid.NewGuid(),
                ProductId = normalizedProductId,
                AvailableQuantity = quantityChange,
                ReservedQuantity = 0,
                CreatedAtUtc = nowUtc,
                UpdatedAtUtc = nowUtc
            };

            _dbContext.InventoryItems.Add(inventoryItem);
        }
        else
        {
            var updatedQuantity =
                (long)inventoryItem.AvailableQuantity + quantityChange;

            if (updatedQuantity < 0)
            {
                return new InventoryAdjustmentResult(
                    InventoryAdjustmentStatus.InsufficientInventory);
            }

            if (updatedQuantity > int.MaxValue)
            {
                throw new InvalidOperationException(
                    "Inventory quantity exceeded the supported maximum value.");
            }

            inventoryItem.AvailableQuantity = (int)updatedQuantity;
            inventoryItem.UpdatedAtUtc = nowUtc;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new InventoryAdjustmentResult(
            InventoryAdjustmentStatus.Success,
            MapToResponse(inventoryItem));
    }

    private static InventoryResponse MapToResponse(
        InventoryItem inventoryItem)
    {
        return new InventoryResponse(
            inventoryItem.ProductId,
            inventoryItem.AvailableQuantity,
            inventoryItem.ReservedQuantity,
            inventoryItem.CreatedAtUtc,
            inventoryItem.UpdatedAtUtc);
    }
}
