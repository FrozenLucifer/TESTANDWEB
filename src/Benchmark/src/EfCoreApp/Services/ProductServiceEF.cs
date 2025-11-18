using Microsoft.EntityFrameworkCore;
using Shared;

namespace EfCoreApp.Services
{
    public class ProductServiceEF
    {
        private readonly AppDbContext _context;

        public ProductServiceEF(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _context.Products
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
            product.CreatedAt = DateTime.UtcNow;
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<List<Product>> CreateProductsBatchAsync(List<Product> products)
        {
            foreach (var product in products)
            {
                product.CreatedAt = DateTime.UtcNow;
            }

            await _context.Products.AddRangeAsync(products);
            await _context.SaveChangesAsync();
            return products;
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return false;

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> DeleteProductsBatchAsync(List<int> ids)
        {
            var products = await _context.Products
                .Where(p => ids.Contains(p.Id))
                .ToListAsync();

            _context.Products.RemoveRange(products);
            return await _context.SaveChangesAsync();
        }

        public async Task CleanupTestDataAsync()
        {
            // Удаляем тестовые данные, созданные во время тестирования
            var testProducts = await _context.Products
                .Where(p => p.Name.StartsWith("TestProduct_") || 
                           p.Name.StartsWith("BatchProduct_") || 
                           p.Name.StartsWith("TempDelete_") || 
                           p.Name.StartsWith("TempBatchDelete_"))
                .ToListAsync();

            _context.Products.RemoveRange(testProducts);
            await _context.SaveChangesAsync();
        }
    }
}