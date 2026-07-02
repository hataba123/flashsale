using System.ComponentModel.DataAnnotations;

namespace FlashSale.Inventory.Api.Contracts;

public sealed class AdjustInventoryRequest
{
    [Range(-1_000_000, 1_000_000)]
    public int QuantityChange { get; init; }
}
