using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebApiUebungen.Entities;
using WebApiUebungen.Models;
using WebApiUebungen.Services;

namespace WebApiUebungen.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService productService;

        public ProductController(IProductService productService)
        {
            this.productService = productService;
        }

        /// <summary>
        /// Creates a new product.
        /// </summary>
        /// <param name="product"><see cref="Product"/> to insert.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateProductAsync([FromBody] Product product, CancellationToken cancellationToken)
        {
            try
            {
                await productService.CreateAsync(product, cancellationToken);
            }
            catch (ArgumentException ex)
            {
                return Conflict(ex.Message);
            }

            return Ok(product);
        }

        /// <summary>
        /// Updates an existing product.
        /// </summary>
        /// <param name="productId">ID of the <see cref="Product"/> to update.</param>
        /// <param name="product">The new values of the <see cref="Product"/>.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut("{productId}", Name = nameof(UpdateProductAsync))]
        [ProducesDefaultResponseType(typeof(Product))]
        [ProducesResponseType(typeof(void), 404)]
        [ProducesResponseType(typeof(void), 409)]
        public async Task<IActionResult> UpdateProductAsync(int productId, [FromBody] Product product, CancellationToken cancellationToken)
        {
            try
            {
                await productService.UpdateAsync(productId, product, cancellationToken);
            }
            catch (ArgumentException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception)
            {
                return NotFound();
            }

            return Ok(product);
        }

        /// <summary>
        /// Finds products with the given <paramref name="searchOptions"/>.
        /// </summary>
        /// <param name="searchOptions"></param>
        /// <param name="pagination"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet(Name = nameof(GetProductsAsync))]
        [ProducesDefaultResponseType(typeof(List<Product>))]
        public Task<List<Product>> GetProductsAsync([FromQuery] ProductSearchOptions searchOptions, [FromQuery] Pagination pagination = null, CancellationToken cancellationToken = default)
        {
            return productService.GetProducts(searchOptions, pagination, cancellationToken);
        }

        /// <summary>
        /// Finds a product by its ID.
        /// </summary>
        /// <param name="productId">ID of the <see cref="Product"/> to retrieve.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet("{productId:int}", Name = nameof(GetProductByIdAsync))]
        [ProducesDefaultResponseType(typeof(Product))]
        [ProducesResponseType(typeof(void), 404)]
        public async Task<IActionResult> GetProductByIdAsync(int productId, CancellationToken cancellationToken)
        {
            var product = await productService.GetProductByIdAsync(productId, cancellationToken);

            if (product is null)
                return NotFound();

            return Ok(product);
        }

        /// <summary>
        /// Finds a product by its number.
        /// </summary>
        /// <param name="productNr"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet("{productNr}", Name = nameof(GetProductByNrAsync))]
        [ProducesDefaultResponseType(typeof(Product))]
        [ProducesResponseType(typeof(void), 404)]
        public async Task<IActionResult> GetProductByNrAsync(string productNr, CancellationToken cancellationToken)
        {
            var product = await productService.GetProductByNrAsync(productNr, cancellationToken);

            if (product is null)
                return NotFound();

            return Ok(product);
        }

        /// <summary>
        /// Removes the product with the given <paramref name="productId"/>.
        /// </summary>
        /// <param name="productId">ID of the <see cref="Product"/> to delete.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [Authorize]
        [HttpDelete("{productId}", Name = nameof(DeleteByIdAsync))]
        [ProducesDefaultResponseType(typeof(void))]
        [ProducesResponseType(typeof(void), 404)]
        public Task DeleteByIdAsync(int productId, CancellationToken cancellationToken)
        {
            return productService.DeleteAsync(productId, cancellationToken);
        }
    }
}
