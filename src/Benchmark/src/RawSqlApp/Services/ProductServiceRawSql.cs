using Npgsql;
using Shared;
using Dapper;

namespace RawSqlApp.Services
{
    public class ProductServiceRawSql
    {
        private readonly string _connectionString;

        public ProductServiceRawSql(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            
            const string sql = @"
                SELECT p.*, c.* 
                FROM Products p 
                LEFT JOIN Categories c ON p.Category_Id = c.Id 
                WHERE p.Id = @Id";
            
            var product = await connection.QueryAsync<Product, Category, Product>(
                sql,
                (product, category) => 
                {
                    product.CategoryId = category.Id;
                    return product;
                },
                new { Id = id },
                splitOn: "Id"
            );
            
            return product.FirstOrDefault();
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            
            const string sql = @"
                INSERT INTO Products (Name, Price, Category_Id, Created_At) 
                VALUES (@Name, @Price, @CategoryId, @CreatedAt) 
                RETURNING *";
            
            product.CreatedAt = DateTime.UtcNow;
            var createdProduct = await connection.QuerySingleAsync<Product>(sql, product);
            createdProduct.CategoryId = product.CategoryId;
            
            return createdProduct;
        }

        public async Task<List<Product>> CreateProductsBatchAsync(List<Product> products)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            
            const string sql = @"
                INSERT INTO Products (Name, Price, Category_Id, Created_At) 
                VALUES (@Name, @Price, @CategoryId, @CreatedAt) 
                RETURNING *";
            
            foreach (var product in products)
            {
                product.CreatedAt = DateTime.UtcNow;
            }
            
            var createdProducts = new List<Product>();
            foreach (var product in products)
            {
                var createdProduct = await connection.QuerySingleAsync<Product>(sql, product);
                createdProducts.Add(createdProduct);
            }
            
            return createdProducts;
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            
            const string sql = "DELETE FROM Products WHERE Id = @Id";
            var affectedRows = await connection.ExecuteAsync(sql, new { Id = id });
            
            return affectedRows > 0;
        }

        public async Task<int> DeleteProductsBatchAsync(List<int> ids)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            
            const string sql = "DELETE FROM Products WHERE Id = ANY(@Ids)";
            var affectedRows = await connection.ExecuteAsync(sql, new { Ids = ids.ToArray() });
            
            return affectedRows;
        }

        public async Task CleanupTestDataAsync()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            
            const string sql = @"
                DELETE FROM Products 
                WHERE Name LIKE 'TestProduct_%' 
                   OR Name LIKE 'BatchProduct_%' 
                   OR Name LIKE 'TempDelete_%' 
                   OR Name LIKE 'TempBatchDelete_%'";
            
            await connection.ExecuteAsync(sql);
        }
    }
}