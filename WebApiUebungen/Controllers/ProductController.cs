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
        private static BlockingCollection<Product> products = new()
        {
            new Product
            {
                Id = 1,
                ProductNr = "TS-678",
                ProductName = "Jogging Schuhe",
                Price = 39.90m,
                Color = "weiss",
                Size = "42",
                CreatorUsername = "Max"
            },
            new Product
            {
                Id = 2,
                ProductNr = "RJ-234",
                ProductName = "Regenjacke",
                Price = 119m,
                Color = "rot",
                Size = "XL",
                CreatorUsername = "marco"
            },
            new Product
            {
                Id = 3,
                ProductNr = "WJ-334",
                ProductName = "Windjacke",
                Price = 39.90m,
                Color = "rot/blau",
                Size = "M",
                CreatorUsername = "Max"
            },
            new Product
            {
                Id = 4,
                ProductNr = "RS-007",
                ProductName = "Rucksack",
                Price = 39.90m,
                Color = "schwarz",
                Size = "Tagesrucksack",
                CreatorUsername = "marco"
            },
        };
        private readonly IProductService productService;

        public ProductController(IProductService productService)
        {
            this.productService = productService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateProductAsync([FromBody] Product product, CancellationToken cancellationToken)
        {
            try
            {
                await productService.Create(product, cancellationToken);
            }
            catch (ArgumentException ex)
            {
                return Conflict(ex.Message);
            }

            return Ok(product);
        }

        [HttpPut("{productId}")]
        public async Task<IActionResult> UpdateProductAsync(int productId, [FromBody] Product product, CancellationToken cancellationToken)
        {
            try
            {
                await productService.Update(productId, product, cancellationToken);
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

        [HttpGet]
        public Task<List<Product>> GetProductsAsync([FromQuery] ProductSearchOptions searchOptions, [FromQuery] Pagination pagination = null, CancellationToken cancellationToken = default)
        {
            return productService.GetProducts(searchOptions, pagination, cancellationToken);
        }

        [HttpGet("{productId:int}")]
        public async Task<IActionResult> GetProductByIdAsync(int productId, CancellationToken cancellationToken)
        {
            var product = await productService.GetProductByIdAsync(productId, cancellationToken);

            if (product is null)
                return NotFound();

            return Ok(product);
        }

        [HttpGet("{productNr}")]
        public async Task<IActionResult> GetProductByNrAsync(string productNr, CancellationToken cancellationToken)
        {
            var product = await productService.GetProductByNrAsync(productNr, cancellationToken);

            if (product is null)
                return NotFound();

            return Ok(product);
        }

        [HttpDelete]
        public Task DeleteByIdAsync(int productId, CancellationToken cancellationToken)
        {
            return productService.DeleteAsync(productId, cancellationToken);
        }
    }
}
