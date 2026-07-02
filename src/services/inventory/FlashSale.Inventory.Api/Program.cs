using FlashSale.Inventory.Api.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var inventoryDatabaseConnectionString =
    builder.Configuration.GetConnectionString("InventoryDatabase");

if (string.IsNullOrWhiteSpace(inventoryDatabaseConnectionString))
{
    throw new InvalidOperationException(
        "Connection string 'InventoryDatabase' is not configured.");
}

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddDbContext<InventoryDbContext>(options =>
{
    options.UseNpgsql(inventoryDatabaseConnectionString);
});

// MỚI: Cho phép Next.js chạy tại localhost:3000 gọi API.
builder.Services.AddCors(options =>
{
    options.AddPolicy("NextJsDevelopment", policy =>
    {
        policy
            .WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// SỬA: Tạm thời không redirect HTTPS khi đang chạy local bằng HTTP.
// app.UseHttpsRedirection();

app.UseCors("NextJsDevelopment");

app.MapControllers();

// MỚI: API kiểm tra service có hoạt động hay không.
app.MapGet("/health", () =>
{
    return Results.Ok(new
    {
        status = "Healthy",
        service = "Inventory",
        timestamp = DateTimeOffset.UtcNow
    });
});

app.Run();
