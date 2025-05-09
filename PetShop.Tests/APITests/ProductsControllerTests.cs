using FluentAssertions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using PetShop.Application.Interfaces.Services;
using PetShop.Application.Requests;
using PetShop.Domain.Entities;
using PetShop.Tests.Mocks;
using PetShop.Tests.ServiceTests;
using PetShopAPI.Controllers;
using PetShopAPI.Validators;

namespace PetShop.Tests.APITests
{
    public class ProductsControllerTests
    {

        private ILogger<ProductsController> _loggerMock = null!;
        private IProductService _productServiceMock = null!;
        private IValidator<ProductRequest> _productRequestValidator = null!;


        private ProductsController CreateController()
        {
            return new ProductsController(_productServiceMock, _loggerMock, _productRequestValidator);
        }   

        [SetUp]
        public void SetUp()
        {
            _productServiceMock = Substitute.For<IProductService>();
            _loggerMock = new TestLogger<ProductsController>();
            _productRequestValidator = new ProductRequestValidator();
        }

        [Test]
        public void GetProducts_AvailablesOnly_Success() 
        {
            var mockList = ProductFixture.GetProductList().Where( x=> x.domain == "us" && x.pettype == "dog" && x.stock >0).ToList();
            string domain = "us";
            string type = "dog";

            _productServiceMock.RetrieveAvailablesList(Arg.Any<string>(), Arg.Any<string>()).Returns(mockList);

            var controller = CreateController();
            var result = controller.GetProducts(domain, type, true).Result;

            result.Should().BeOfType<OkObjectResult>();
            OkObjectResult objectResult = (OkObjectResult)result;
            objectResult.StatusCode.Should().Be(200);
            objectResult.Value.Should().BeOfType<List<Product>>();

            List<Product> list = (List<Product>)objectResult.Value;
            list.Count().Should().Be(1);
        }

        [Test]
        public void GetProducts_RetriveAll_Success()
        {
            List<Product> mockList = ProductFixture.GetProductList().Where(x => x.domain == "us" && x.pettype == "dog").ToList();

            _productServiceMock.RetrieveAllList(Arg.Any<string>(), Arg.Any<string>()).Returns(mockList);

            var controller = CreateController();

            string domain = "us";
            string type = "dog";

            var result = controller.GetProducts(domain, type, false).Result;

            result.Should().BeOfType<OkObjectResult>();
            OkObjectResult objectResult = (OkObjectResult)result;
            objectResult.StatusCode.Should().Be(200);
            objectResult.Value.Should().BeOfType<List<Product>>();
            IEnumerable<Product> list = (List<Product>)objectResult.Value;
            list.Count().Should().Be(2);
        }

        [Test]
        public void GetProducts_NoProductsAvailable_ReturnsEmptyList()
        {
            string domain = "us";
            string type = "dog";

            _productServiceMock.RetrieveAvailablesList(Arg.Any<string>(), Arg.Any<string>()).Returns(new List<Product>());

            var controller = CreateController();

            var result = controller.GetProducts(domain, type, true).Result;

            result.Should().BeOfType<OkObjectResult>();
            var objectResult = (OkObjectResult)result;
            objectResult.StatusCode.Should().Be(200);

            var list = (IEnumerable<Product>)objectResult.Value;
            list.Should().BeEmpty();
        }


        [Test]
        public void CreateProduct_InvalidRequest_ReturnsBadRequest()
        {
            var invalidRequest = new ProductRequest { Name = "", Domain = "" }; // Invalid data

            var controller = CreateController();

            var result = controller.CreateProduct(invalidRequest).Result;

            result.Should().BeOfType<UnprocessableEntityObjectResult>();
            var objectResult = (UnprocessableEntityObjectResult)result;
            objectResult.StatusCode.Should().Be(422);
        }

        [Test]
        public void DeleteProduct_ValidRequest_ReturnsNoContent()
        {
            string domain = "us";
            string name = "dog-food";

            _productServiceMock.Delete(Arg.Any<string>(), Arg.Any<string>()).Returns(true);

            var controller = CreateController();

            var result = controller.DeleteProduct(domain, name).Result;

            result.Should().BeOfType<OkObjectResult>();
        }
    }
}
