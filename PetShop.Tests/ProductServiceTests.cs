using Azure;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using PetShop.Data;
using PetShop.Model;
using PetShop.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetShop.Tests
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

            var entity = new ProductEntity()
            {
                guid = Guid.NewGuid().ToString(),
                ETag = ETag.All,
            };

            var loggerMock = new TestLogger<ProductService>();
            var productRepositoryMock = new Mock<IProductRepository>();
            productRepositoryMock.Setup(m => m.Create(It.IsAny<ProductEntity>())).Returns(Task.FromResult<ProductEntity?>(entity));

            var productService = new ProductService(productRepositoryMock.Object, loggerMock);
            var entityResult = productService.Create(request).Result;

            entityResult.Should().NotBeNull();
            entityResult.guid.Should().NotBeNullOrEmpty();
        }

        [Test]
        public void Test_Delete_Product()
        {
            var domain = "bo";
            var name = "dog chow";
            var entity = new ProductEntity()
            {
                guid = Guid.NewGuid().ToString(),
                ETag = ETag.All,
                domain = domain,
                name = name
            };
            var loggerMock = new TestLogger<ProductService>();
            var productRepositoryMock = new Mock<IProductRepository>();
            productRepositoryMock.Setup(m => m.Retrieve(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(entity);
            productRepositoryMock.Setup(m => m.Delete(It.IsAny<ProductEntity>())).ReturnsAsync(true);

            var productService = new ProductService(productRepositoryMock.Object, loggerMock);
            var entityResult = productService.Delete(domain, name).Result;
            entityResult.Should().Be(true);
        }

        [Test]
        public void Test_Delete_Product_Failed()
        {
            var domain = "bo";
            var name = "dog chow";
            var entity = new ProductEntity()
            {
                guid = Guid.NewGuid().ToString(),
                ETag = ETag.All,
                domain = domain,
                name = name
            };

            var loggerMock = new TestLogger<ProductService>();
            var productRepositoryMock = new Mock<IProductRepository>();
            productRepositoryMock.Setup(m => m.Retrieve(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult<ProductEntity?>(null));
            productRepositoryMock.Setup(m => m.Delete(It.IsAny<ProductEntity>())).ReturnsAsync(true);

            var productService = new ProductService(productRepositoryMock.Object, loggerMock);
            var entityResult = productService.Delete(domain, name).Result;
            entityResult.Should().Be(false);
            loggerMock.Messages.Should().HaveCount(1);
            loggerMock.Messages.Should().Contain($"Delete failed. Product not Found. Domain {domain}, Name {name}");
        }
    }
}
