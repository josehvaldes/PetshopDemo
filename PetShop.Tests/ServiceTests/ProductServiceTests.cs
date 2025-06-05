using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using PetShop.Application.Interfaces.Repository;
using PetShop.Application.Interfaces.Repository.Products;
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
        private IProductCommand _productCommandMock = null!;
        private IProductQuery _productQueryMock = null!;
        private IProductQueryable _productQueryable = null!;
        private ISaleQueryable _saleQueryableMock = null!;

        private ProductService CreateProductService() 
        {
            return new ProductService(_productCommandMock, _productQueryMock, 
                _productQueryable, _saleQueryableMock, _loggerMock);
        }

        [SetUp]
        public void SetUp() 
        {
            _loggerMock = new TestLogger<ProductService>();
            _productCommandMock = Substitute.For<IProductCommand>();
            _productQueryMock = Substitute.For<IProductQuery>();
            _productQueryable = Substitute.For<IProductQueryable>();
            _saleQueryableMock = Substitute.For<ISaleQueryable>();
        }

        [Test]
        public void Test_Create_Product_Valid_GUID()
        {
            var request = new ProductRequest()
            {
            };

            var product = ProductFixture.GetProduct();

            _productCommandMock.Create(Arg.Any<Product>()).Returns(product);

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

            _productQueryMock.Retrieve(Arg.Any<string>(), Arg.Any<string>()).Returns(product);
            _productCommandMock.Delete(Arg.Any<Product>()).Returns(true);

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

            _productQueryMock.Retrieve(Arg.Any<string>(), Arg.Any<string>()).Returns((Product?)null);
            _productCommandMock.Delete(Arg.Any<Product>()).Returns(true);

            var productService = CreateProductService();
            var entityResult = productService.Delete(domain, name).Result;
            entityResult.Should().Be(false);
            _loggerMock.Messages.Should().HaveCount(1);
            _loggerMock.Messages.Should().Contain($"Delete failed. Product not Found. Domain {domain}, Name {name}");
        }
    }
}
