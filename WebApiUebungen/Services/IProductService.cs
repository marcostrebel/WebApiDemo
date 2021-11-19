using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WebApiUebungen.Entities;
using WebApiUebungen.Models;

namespace WebApiUebungen.Services
{
    public interface IProductService
    {
        Task Create(Product product, CancellationToken cancellationToken = default);

        Task Update(int productId, Product product, CancellationToken cancellationToken = default);

        Task<Product> GetProductByIdAsync(int productId, CancellationToken cancellationToken = default);

        Task<Product> GetProductByNrAsync(string productNr, CancellationToken cancellationToken = default);

        Task<List<Product>> GetProducts(ProductSearchOptions searchOptions, Pagination pagination, CancellationToken cancellationToken = default);

        Task DeleteAsync(int productId, CancellationToken cancellationToken = default);
    }
}