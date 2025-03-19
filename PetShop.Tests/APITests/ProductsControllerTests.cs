using FluentAssertions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using PetShop.Model;
using PetShop.Service;
using PetShop.Tests.Mocks;
using PetShop.Tests.ServiceTests;
using PetShopAPI.Controllers;
using PetShopAPI.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetShop.Tests.APITests
{
    public class ProductsControllerTests
    {

        private ILogger<ProductsController> _loggerMock;
        private Mock<IProductService> _productServiceMock;
        private IValidator<ProductRequest> _productRequestValidator;

        public ProductsControllerTests() 
        {
            _productServiceMock = new Mock<IProductService>();
            _loggerMock = new TestLogger<ProductsController>();
            _productRequestValidator = new ProductRequestValidator();
        }

        [Test]
        public void GetProducts_AvailablesOnly_Success() 
        {
            var mockList = ProductFixture.GetProductList().Where( x=> x.domain == "us" && x.pettype == "dog" && x.stock >0).ToList();
            string domain = "bo";
            string type = "dog";

            _productServiceMock.Setup(m => m.RetrieveAvailablesList(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(mockList);

            var controller = new ProductsController(_productServiceMock.Object, _loggerMock, _productRequestValidator);
            var result = controller.GetProducts(domain, type, true).Result;

            result.Should().BeOfType<OkObjectResult>();
            OkObjectResult objectResult = (OkObjectResult)result;
            objectResult.StatusCode.Should().Be(200);
            objectResult.Value.Should().BeOfType<List<ProductEntity>>();

            IEnumerable<ProductEntity> list = (List<ProductEntity>)objectResult.Value;
            list.Count().Should().Be(1);
        }

        [Test]
        public void GetProducts_RetriveAll_Success()
        {
            var mockList = ProductFixture.GetProductList().Where(x => x.domain == "us" && x.pettype == "dog").ToList();

            _productServiceMock.Setup(m => m.RetrieveAvailablesList(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(mockList);

            var controller = new ProductsController(_productServiceMock.Object, _loggerMock, _productRequestValidator);

            string domain = "bo";
            string type = "dog";

            var result = controller.GetProducts(domain, type, true).Result;

            result.Should().BeOfType<OkObjectResult>();
            OkObjectResult objectResult = (OkObjectResult)result;
            objectResult.StatusCode.Should().Be(200);
            objectResult.Value.Should().BeOfType<List<ProductEntity>>();

            IEnumerable<ProductEntity> list = (List<ProductEntity>)objectResult.Value;
            list.Count().Should().Be(2);

        }
    }
}
