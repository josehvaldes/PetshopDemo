using Azure;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using PetShop.Application.Interfaces;
using PetShop.Application.Requests;
using PetShop.Application.Services;
using PetShop.Domain.Entities;
using PetShop.Tests.Mocks;

namespace PetShop.Tests.ServiceTests
{
    [TestFixture]
    public class ProductServiceTests
    {

        [Test]
        public void Test_Create_Product_Valid_GUID()
        {
            var request = new ProductRequest()
            {
            };

            var product = ProductFixture.GetProduct();

            var loggerMock = new TestLogger<ProductService>();
            var productRepositoryMock = new Mock<IProductRepository>();
            productRepositoryMock.Setup(m => m.Create(It.IsAny<Product>())).Returns(Task.FromResult<Product?>(product));

            var productService = new ProductService(productRepositoryMock.Object, loggerMock);
            var entityResult = productService.Create(request).Result;

            entityResult.Should().NotBeNull();
            entityResult.guid.Should().NotBeNullOrEmpty();
        }

        [Test]
        public void Test_Delete_Product()
        {
            var product = ProductFixture.GetProduct();
            var domain = product.domain;
            var name = product.name;

            var loggerMock = new TestLogger<ProductService>();
            var productRepositoryMock = new Mock<IProductRepository>();
            productRepositoryMock.Setup(m => m.Retrieve(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(product);
            productRepositoryMock.Setup(m => m.Delete(It.IsAny<Product>())).ReturnsAsync(true);

            var productService = new ProductService(productRepositoryMock.Object, loggerMock);
            var entityResult = productService.Delete(domain, name).Result;
            entityResult.Should().Be(true);
        }

        [Test]
        public void Test_Delete_Product_Failed()
        {
            var product = ProductFixture.GetProduct();
            var domain = product.domain;
            var name = product.name;
            

            var loggerMock = new TestLogger<ProductService>();
            var productRepositoryMock = new Mock<IProductRepository>();
            productRepositoryMock.Setup(m => m.Retrieve(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult<Product?>(null));
            productRepositoryMock.Setup(m => m.Delete(It.IsAny<Product>())).ReturnsAsync(true);

            var productService = new ProductService(productRepositoryMock.Object, loggerMock);
            var entityResult = productService.Delete(domain, name).Result;
            entityResult.Should().Be(false);
            loggerMock.Messages.Should().HaveCount(1);
            loggerMock.Messages.Should().Contain($"Delete failed. Product not Found. Domain {domain}, Name {name}");
        }
    }
}
