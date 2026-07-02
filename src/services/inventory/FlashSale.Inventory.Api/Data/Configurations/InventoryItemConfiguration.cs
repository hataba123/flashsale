using FlashSale.Inventory.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlashSale.Inventory.Api.Data.Configurations;

public sealed class InventoryItemConfiguration
    : IEntityTypeConfiguration<InventoryItem>
{
    public void Configure(EntityTypeBuilder<InventoryItem> builder)
    {
        builder.ToTable(
            "inventory_items",
            tableBuilder =>
            {
                tableBuilder.HasCheckConstraint(
                    "ck_inventory_items_available_quantity_non_negative",
                    "\"available_quantity\" >= 0");

                tableBuilder.HasCheckConstraint(
                    "ck_inventory_items_reserved_quantity_non_negative",
                    "\"reserved_quantity\" >= 0");
            });

        builder.HasKey(item => item.Id)
            .HasName("pk_inventory_items");

        builder.Property(item => item.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(item => item.ProductId)
            .HasColumnName("product_id")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(item => item.AvailableQuantity)
            .HasColumnName("available_quantity")
            .IsRequired();

        builder.Property(item => item.ReservedQuantity)
            .HasColumnName("reserved_quantity")
            .IsRequired();

        builder.Property(item => item.CreatedAtUtc)
            .HasColumnName("created_at_utc")
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .IsRequired();

        builder.Property(item => item.UpdatedAtUtc)
            .HasColumnName("updated_at_utc")
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .IsRequired();

        builder.HasIndex(item => item.ProductId)
            .IsUnique()
            .HasDatabaseName("ux_inventory_items_product_id");
    }
}
