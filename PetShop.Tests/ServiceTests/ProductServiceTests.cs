using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using PetShop.Application.Interfaces.Repository;
using PetShop.Application.Requests;
using PetShop.Application.Services;
using PetShop.Domain.Entities;
using PetShop.Tests.Mocks;

namespace PetShop.Tests.ServiceTests
{
    [TestFixture]
    public class ProductServiceTests
    {
        private TestLogger<ProductService> _loggerMock = new TestLogger<ProductService>();
        private IProductRepository _productRepositoryMock = null!;

        private ProductService CreateProductService() 
        {
            return new ProductService(_productRepositoryMock, _loggerMock);
        }

        [SetUp]
        public void SetUp() 
        {
            _loggerMock = new TestLogger<ProductService>();
            _productRepositoryMock = Substitute.For<IProductRepository>();
        }

        [Test]
        public void Test_Create_Product_Valid_GUID()
        {
            var request = new ProductRequest()
            {
            };

            var product = ProductFixture.GetProduct();

            _productRepositoryMock.Create(Arg.Any<Product>()).Returns(product);

            var productService = CreateProductService();
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

            _productRepositoryMock.Retrieve(Arg.Any<string>(), Arg.Any<string>()).Returns(product);
            _productRepositoryMock.Delete(Arg.Any<Product>()).Returns(true);

            var productService = CreateProductService(); 
            var entityResult = productService.Delete(domain, name).Result;
            entityResult.Should().Be(true);
        }

        [Test]
        public void Test_Delete_Product_Failed()
        {
            var product = ProductFixture.GetProduct();
            var domain = product.domain;
            var name = product.name;
            
            _productRepositoryMock.Retrieve(Arg.Any<string>(), Arg.Any<string>()).Returns((Product?)null);
            _productRepositoryMock.Delete(Arg.Any<Product>()).Returns(true);

            var productService = CreateProductService();
            var entityResult = productService.Delete(domain, name).Result;
            entityResult.Should().Be(false);
            _loggerMock.Messages.Should().HaveCount(1);
            _loggerMock.Messages.Should().Contain($"Delete failed. Product not Found. Domain {domain}, Name {name}");
        }
    }
}
