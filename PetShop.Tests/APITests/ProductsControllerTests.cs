using FluentAssertions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
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

        [Test]
        public void GetProducts_AvailablesOnly_Success() 
        {

            var productServiceMock = new Mock<IProductService>();
            var mockList = ProductFixture.GetProductList().Where( x=> x.domain == "us" && x.pettype == "dog" && x.stock >0).ToList();

            productServiceMock.Setup(m => m.RetrieveAvailablesList(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(mockList);

            var loggerMock = new TestLogger<ProductsController>();
            IValidator<ProductRequest> _productRequestValidator = new ProductRequestValidator();

            var controller = new ProductsController(productServiceMock.Object, loggerMock, _productRequestValidator);

            string domain = "bo";
            string type = "dog";
            
            var response = controller.GetProducts(domain, type, true).Result;

            response.Should().BeOfType<OkObjectResult>();
            OkObjectResult objectResult = (OkObjectResult)response;
            objectResult.StatusCode.Should().Be(200);
            objectResult.Value.Should().BeOfType<List<ProductEntity>>();

            IEnumerable<ProductEntity> list = (List<ProductEntity>)objectResult.Value;
            list.Count().Should().Be(1);

        }

        [Test]
        public void GetProducts_RetriveAll_Success()
        {

            var productServiceMock = new Mock<IProductService>();
            var mockList = ProductFixture.GetProductList().Where(x => x.domain == "us" && x.pettype == "dog").ToList();

            productServiceMock.Setup(m => m.RetrieveAvailablesList(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(mockList);

            var loggerMock = new TestLogger<ProductsController>();
            IValidator<ProductRequest> _productRequestValidator = new ProductRequestValidator();

            var controller = new ProductsController(productServiceMock.Object, loggerMock, _productRequestValidator);

            string domain = "bo";
            string type = "dog";

            var response = controller.GetProducts(domain, type, true).Result;

            response.Should().BeOfType<OkObjectResult>();
            OkObjectResult objectResult = (OkObjectResult)response;
            objectResult.StatusCode.Should().Be(200);
            objectResult.Value.Should().BeOfType<List<ProductEntity>>();

            IEnumerable<ProductEntity> list = (List<ProductEntity>)objectResult.Value;
            list.Count().Should().Be(2);

        }
    }
}
