﻿using FluentAssertions;
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetShop.Tests.APITests
{
    public class ProductsControllerTests
    {

        private ILogger<ProductsController> _loggerMock = null!;
        private Mock<IProductService> _productServiceMock = null!;
        private IValidator<ProductRequest> _productRequestValidator = null!;


        private ProductsController CreateController()
        {
            return new ProductsController(_productServiceMock.Object, _loggerMock, _productRequestValidator);
        }   

        [SetUp]
        public void SetUp()
        {
            _productServiceMock = new Mock<IProductService>();
            _loggerMock = new TestLogger<ProductsController>();
            _productRequestValidator = new ProductRequestValidator();
        }

        [Test]
        public void GetProducts_AvailablesOnly_Success() 
        {
            var mockList = ProductFixture.GetProductList().Where( x=> x.domain == "us" && x.pettype == "dog" && x.stock >0).ToList();
            string domain = "us";
            string type = "dog";

            _productServiceMock.Setup(m => m.RetrieveAvailablesList(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(mockList);

            var controller = CreateController();
            var result = controller.GetProducts(domain, type, true).Result;

            result.Should().BeOfType<OkObjectResult>();
            OkObjectResult objectResult = (OkObjectResult)result;
            objectResult.StatusCode.Should().Be(200);
            objectResult.Value.Should().BeOfType<IEnumerable<ProductEntity>>();

            IEnumerable<ProductEntity> list = (IEnumerable<ProductEntity>)objectResult.Value;
            list.Count().Should().Be(1);
        }

        [Test]
        public void GetProducts_RetriveAll_Success()
        {
            var mockList = ProductFixture.GetProductList().Where(x => x.domain == "us" && x.pettype == "dog").ToList();

            _productServiceMock.Setup(m => m.RetrieveAllList(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(mockList);

            var controller = CreateController();

            string domain = "us";
            string type = "dog";

            var result = controller.GetProducts(domain, type, false).Result;

            result.Should().BeOfType<OkObjectResult>();
            OkObjectResult objectResult = (OkObjectResult)result;
            objectResult.StatusCode.Should().Be(200);
            objectResult.Value.Should().BeOfType<List<ProductEntity>>();

            List<ProductEntity> list = (List<ProductEntity>)objectResult.Value;
            list.Count().Should().Be(2);

        }

        [Test]
        public void GetProducts_NoProductsAvailable_ReturnsEmptyList()
        {
            string domain = "us";
            string type = "dog";

            _productServiceMock.Setup(m => m.RetrieveAvailablesList(domain, type)).ReturnsAsync(new List<ProductEntity>());

            var controller = CreateController();

            var result = controller.GetProducts(domain, type, true).Result;

            result.Should().BeOfType<OkObjectResult>();
            var objectResult = (OkObjectResult)result;
            objectResult.StatusCode.Should().Be(200);

            var list = (IEnumerable<ProductEntity>)objectResult.Value;
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

            _productServiceMock.Setup(m => m.Delete(domain, name)).ReturnsAsync(true);

            var controller = CreateController();

            var result = controller.DeleteProduct(domain, name).Result;

            result.Should().BeOfType<OkObjectResult>();
        }
    }
}
