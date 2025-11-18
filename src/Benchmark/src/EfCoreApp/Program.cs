using EfCoreApp;
using EfCoreApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<AppDbContext>();
builder.Services.AddScoped<ProductServiceEF>();
builder.Services.AddScoped<IServiceScopeFactory>(provider => provider.GetRequiredService<IServiceScopeFactory>());

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await context.Database.EnsureCreatedAsync();

    if (!await context.Products.AnyAsync())
    {
        await SeedData(context);
    }
}

app.MapGet("/products/{id}",
    async (int id, ProductServiceEF service) =>
    {
        var product = await service.GetProductByIdAsync(id);
        return product != null ? Results.Ok(product) : Results.NotFound();
    });

app.MapPost("/products",
    async ([FromBody] Product product, ProductServiceEF service) =>
    {
        var createdProduct = await service.CreateProductAsync(product);
        return Results.Created($"/products/{createdProduct.Id}", createdProduct);
    });

app.MapPost("/products/batch",
    async ([FromBody] List<Product> products, ProductServiceEF service) =>
    {
        var createdProducts = await service.CreateProductsBatchAsync(products);
        return Results.Created("/products/batch", new { insertedCount = createdProducts.Count });
    });

app.MapDelete("/products/{id}",
    async (int id, ProductServiceEF service) =>
    {
        var deleted = await service.DeleteProductAsync(id);
        return deleted ? Results.Ok() : Results.NotFound();
    });

app.MapDelete("/products/batch",
    async ([FromBody] BatchDeleteRequest request, ProductServiceEF service) =>
    {
        var deletedCount = await service.DeleteProductsBatchAsync(request.Ids);
        return Results.Ok(new { deletedCount });
    });

app.MapDelete("/test/cleanup",
    async (ProductServiceEF service) =>
    {
        await service.CleanupTestDataAsync();
        return Results.Ok();
    });

app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }));

app.Run("http://*:5000");

async Task SeedData(AppDbContext context)
{
    var categories = Enumerable.Range(1, 10)
        .Select(i => new Category { Name = $"Category_{i}" })
        .ToList();

    await context.Categories.AddRangeAsync(categories);
    await context.SaveChangesAsync();

    var random = new Random();
    var products = Enumerable.Range(1, 1000)
        .Select(i => new Product
        {
            Name = $"Product_{i}",
            Price = (decimal)(random.NextDouble() * 1000),
            CategoryId = random.Next(1, 11),
            CreatedAt = DateTime.UtcNow.AddDays(-random.Next(1, 365))
        })
        .ToList();

    await context.Products.AddRangeAsync(products);
    await context.SaveChangesAsync();

    var orders = Enumerable.Range(1, 5000)
        .Select(i => new Order
        {
            ProductId = random.Next(1, 1001),
            Quantity = random.Next(1, 10),
            TotalPrice = (decimal)(random.NextDouble() * 500),
            CreatedAt = DateTime.UtcNow.AddDays(-random.Next(1, 30))
        })
        .ToList();

    await context.Orders.AddRangeAsync(orders);
    await context.SaveChangesAsync();
}
public class BatchDeleteRequest
{
    public List<int> Ids { get; set; } = new();
}