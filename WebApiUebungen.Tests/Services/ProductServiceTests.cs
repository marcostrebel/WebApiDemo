using Moq;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebApiUebungen.Entities;
using WebApiUebungen.Services;
using Xunit;

namespace WebApiUebungen.Tests.Services
{
    public class ProductServiceTests
    {
        [Theory]
        [ClassData(typeof(ExistingProductTestData))]
        public void Create_ExistingProduct_ShouldThrowException(Product product, Type exception, string message)
        {
            // Arrange
            var cancellation = CancellationToken.None;

            var products = new List<Product>() { product };

            var dbMock = new Mock<WebApiDemoDbContext>();
            var productsMock = dbMock.SetupDbSetMock(db => db.Product, products, cancellation);

            var service = new ProductService(dbMock.Object);

            // Act
            Func<Task> test = async () => await service.CreateAsync(product, cancellation);

            // Assert
            var ex = test.ShouldThrow(exception);
            ex.Message.ShouldBe(message);

            dbMock.Verify(db => db.AddAsync(It.IsAny<Product>(), cancellation), Times.Never);
            dbMock.Verify(db => db.SaveChangesAsync(cancellation), Times.Never);
        }

        private class ExistingProductTestData : TheoryData<Product, Type, string>
        {
            public ExistingProductTestData()
            {
                var p1 = new Product() { ProductNr = "Test" };

                Add(p1, typeof(ArgumentException), "ProductNr exists already");
            }
        }

        [Theory]
        [ClassData(typeof(NonExistingProductTestData))]
        public void Update_NonExistingProduct_ShouldThrowException(List<Product> source, int productId)
        {
            // Arrange
            var cancellation = CancellationToken.None;

            var product = new Product
            {
                ProductNr = "Demo-Prod",
                Price = 1,
                Size = "standard",
                Color = "none"
            };

            var dbMock = new Mock<WebApiDemoDbContext>();
            var productsMock = dbMock.SetupDbSetMock(db => db.Product, source, cancellation);

            var service = new ProductService(dbMock.Object);

            // Act
            Func<Task> test = async () => await service.UpdateAsync(productId, product, cancellation);

            // Assert
            var ex = test.ShouldThrow(typeof(Exception));
            ex.Message.ShouldContain("Product not found");

            dbMock.Verify(db => db.SaveChangesAsync(cancellation), Times.Never);
        }

        private class NonExistingProductTestData : TheoryData<List<Product>, int>
        {
            public NonExistingProductTestData()
            {
                var product1 = new Product { Id = 1 };
                var product2 = new Product { Id = 2 };
                var product3 = new Product { Id = 3 };

                Add(new List<Product>(), 0);
                Add(new List<Product>(), 1);
                Add(new List<Product>() { product1 }, 2);
                Add(new List<Product>() { product1, product2, product3 }, 4);
            }
        }
    }
}
