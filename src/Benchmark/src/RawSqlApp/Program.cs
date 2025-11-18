// using RawSqlApp.Services;
//
// var builder = WebApplication.CreateBuilder(args);
//
// var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING")
//                        ?? "Host=localhost;Database=benchmark_db;Username=postgres;Password=password;Port=5432";
//
// builder.Services.AddScoped<ProductServiceRawSql>(_ => new ProductServiceRawSql(connectionString));
//
// var app = builder.Build();
//
// app.MapGet("/products/{id}",
//     async (int id, ProductServiceRawSql service) =>
//     {
//         var product = await service.GetProductByIdAsync(id);
//         return product != null ? Results.Ok(product) : Results.NotFound();
//     });
// app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }));
//
// app.Run("http://*:5001");


using Microsoft.AspNetCore.Mvc;
using RawSqlApp.Services;
using Shared;

var builder = WebApplication.CreateBuilder(args);

var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING")
                       ?? "Host=localhost;Database=benchmark_db;Username=postgres;Password=password;Port=5432";

builder.Services.AddScoped<ProductServiceRawSql>(_ => new ProductServiceRawSql(connectionString));

var app = builder.Build();

app.MapGet("/products/{id}",
    async (int id, ProductServiceRawSql service) =>
    {
        var product = await service.GetProductByIdAsync(id);
        return product != null ? Results.Ok(product) : Results.NotFound();
    });

app.MapPost("/products",
    async ([FromBody] Product product, ProductServiceRawSql service) =>
    {
        var createdProduct = await service.CreateProductAsync(product);
        return Results.Created($"/products/{createdProduct.Id}", createdProduct);
    });

app.MapPost("/products/batch",
    async ([FromBody] List<Product> products, ProductServiceRawSql service) =>
    {
        var createdProducts = await service.CreateProductsBatchAsync(products);
        return Results.Created("/products/batch", new { insertedCount = createdProducts.Count });
    });

app.MapDelete("/products/{id}",
    async (int id, ProductServiceRawSql service) =>
    {
        var deleted = await service.DeleteProductAsync(id);
        return deleted ? Results.Ok() : Results.NotFound();
    });

app.MapDelete("/products/batch",
    async ([FromBody] BatchDeleteRequest request, ProductServiceRawSql service) =>
    {
        var deletedCount = await service.DeleteProductsBatchAsync(request.Ids);
        return Results.Ok(new { deletedCount });
    });

app.MapDelete("/test/cleanup",
    async (ProductServiceRawSql service) =>
    {
        await service.CleanupTestDataAsync();
        return Results.Ok();
    });

app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }));

app.Run("http://*:5001");

public class BatchDeleteRequest
{
    public List<int> Ids { get; set; } = new();
}