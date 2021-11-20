using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebApiUebungen.Entities;
using WebApiUebungen.Models;

namespace WebApiUebungen.Services
{
    public class ProductService : IProductService
    {
        private readonly WebApiDemoDbContext dbContext;

        public ProductService(WebApiDemoDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task Create(Product product, CancellationToken cancellationToken)
        {
            var existing = await dbContext.Product
                .Where(p => p.ProductNr == product.ProductNr)
                .AnyAsync(cancellationToken);

            if (existing)
                throw new ArgumentException("ProductNr exists already");

            await dbContext.AddAsync(product, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task Update(int productId, Product product, CancellationToken cancellationToken)
        {
            var existing = await GetProductByIdAsync(productId, cancellationToken);

            if (existing is null)
                throw new Exception("Product not found");

            var anyOther = await dbContext.Product
                .Where(p => p.ProductNr == product.ProductNr && p.Id != productId)
                .AnyAsync(cancellationToken);

            if (anyOther)
                throw new ArgumentException("ProductNr exists already");

            existing.ProductName = product.ProductName;
            existing.Price = product.Price;
            existing.Color = product.Color;
            existing.Size = product.Size;

            await dbContext.SaveChangesAsync(cancellationToken);

            product.Id = existing.Id;
        }

        public Task<List<Product>> GetProducts(ProductSearchOptions searchOptions, Pagination pagination, CancellationToken cancellationToken)
        {
            var query = dbContext.Product
                .Where(p => searchOptions.Color == null || p.Color.Contains(searchOptions.Color))
                .Where(p => searchOptions.Size == null || p.Size == searchOptions.Size);

            if (pagination.PageNumber > 0)
                query = query.Skip((pagination.PageNumber - 1) * pagination.PageSize);

            if (pagination.PageSize > 0)
                query = query.Take(pagination.PageSize);

            return query.ToListAsync(cancellationToken);
        }

        public Task<Product> GetProductByIdAsync(int productId, CancellationToken cancellationToken)
        {
            return dbContext.Product
                .SingleOrDefaultAsync(p => p.Id == productId, cancellationToken);
        }

        public Task<Product> GetProductByNrAsync(string productNr, CancellationToken cancellationToken)
        {
            return dbContext.Product
                .SingleOrDefaultAsync(p => p.ProductNr == productNr, cancellationToken);
        }

        public async Task DeleteAsync(int productId, CancellationToken cancellationToken = default)
        {
            var product = await GetProductByIdAsync(productId, cancellationToken);

            if (product is null)
                return;

            dbContext.Remove(product);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
