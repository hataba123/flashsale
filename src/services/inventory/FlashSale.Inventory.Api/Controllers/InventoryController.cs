using FlashSale.Inventory.Api.Contracts;
using FlashSale.Inventory.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace FlashSale.Inventory.Api.Controllers;

[ApiController]
[Route("api/inventory")]
public sealed class InventoryController : ControllerBase
{
    private readonly IInventoryService _inventoryService;

    public InventoryController(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    [HttpGet("{productId}")]
    [ProducesResponseType<InventoryResponse>(
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        StatusCodes.Status404NotFound)]
    [ProducesResponseType(
        StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<InventoryResponse>> GetByProductId(
        string productId,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(productId))
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Invalid product ID",
                detail: "Product ID is required.");
        }

        var inventory = await _inventoryService.GetByProductIdAsync(
            productId,
            cancellationToken);

        if (inventory is null)
        {
            return Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Inventory not found",
                detail: $"Inventory for product '{productId}' was not found.");
        }

        return Ok(inventory);
    }

    [HttpPost("{productId}/adjust")]
    [ProducesResponseType<InventoryResponse>(
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        StatusCodes.Status409Conflict)]
    public async Task<ActionResult<InventoryResponse>> Adjust(
        string productId,
        [FromBody] AdjustInventoryRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(productId))
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Invalid product ID",
                detail: "Product ID is required.");
        }

        var result = await _inventoryService.AdjustAsync(
            productId,
            request.QuantityChange,
            cancellationToken);

        return result.Status switch
        {
            InventoryAdjustmentStatus.Success =>
                Ok(result.Inventory),

            InventoryAdjustmentStatus.InvalidQuantityChange =>
                Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "Invalid quantity change",
                    detail: "Quantity change must not be zero."),

            InventoryAdjustmentStatus.InsufficientInventory =>
                Problem(
                    statusCode: StatusCodes.Status409Conflict,
                    title: "Insufficient inventory",
                    detail:
                        "The inventory adjustment would make the available quantity negative."),

            _ => throw new InvalidOperationException(
                $"Unsupported inventory adjustment status: {result.Status}.")
        };
    }
}
