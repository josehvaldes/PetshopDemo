using FluentValidation;
using Microsoft.Extensions.Logging;
using PetShop.Model;
using PetShop.Service;
using PetShopAPI.Controllers;
using Microsoft.FeatureManagement;
using NUnit.Framework;
using PetShop.Tests.ServiceTests;
using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using PetShopSalesAPI.Auth;


namespace PetShop.Tests.APITests
{
    
    public class SalesControllerTests
    {
        private ILogger<SalesController> _logger = null!;
        private Mock<IValidator<SalesRequest>> _salesRequestValidator = null!;
        private Mock<ISaleService> _saleService = null!;
        private Mock<IFeatureManager> _featureManager = null!;

        private SalesController CreateController()
        {
            var controller = new SalesController(_logger, _salesRequestValidator.Object, _saleService.Object, _featureManager.Object);
            var context = new DefaultHttpContext();
            context.Items.Add("User", new User { UserName = "local", Domain = string.Empty });
            controller.ControllerContext.HttpContext = context;
            return controller;
        }

        [SetUp]
        public void SetUp()
        {
            _logger = new TestLogger<SalesController>();
            _salesRequestValidator = new Mock<IValidator<SalesRequest>>();
            _saleService = new Mock<ISaleService>();
            _featureManager = new Mock<IFeatureManager>();
        }


        [Test]
        public void RetrieveSales_WithValidDomain_ReturnsOkWithSalesList()
        {
            string domain = "bo";
            List<SaleEntity> list = new List<SaleEntity>() { new SaleEntity() { domain="bo", saleid = "1" } };
            _saleService.Setup(m => m.RetrieveList(domain)).ReturnsAsync(list);

            var salesController = CreateController();


            var result = salesController.RetrieveSales(domain).Result;
            result.Should().BeOfType<OkObjectResult>();
            OkObjectResult objectResult = (OkObjectResult)result;
            objectResult.StatusCode.Should().Be(200);
            IEnumerable<SaleEntity> value = (IEnumerable<SaleEntity>)objectResult.Value;
            value.Should().NotBeNullOrEmpty();
            value.Should().HaveCount(1);
            value.First().domain.Should().Be("bo");
        }


        [Test]
        public void RetrieveSales_ServiceThrowsException_ReturnsInternalServerError()
        {
            string domain = "bo";
            _saleService.Setup(m => m.RetrieveList(domain)).ThrowsAsync(new Exception("Service error"));

            var salesController = CreateController();

            var result = salesController.RetrieveSales(domain).Result;

            result.Should().BeOfType<ObjectResult>();
            var objectResult = (ObjectResult)result;
            objectResult.StatusCode.Should().Be(500);
            objectResult.Value.Should().Be("Service error");
        }

        [TestCase("Completed", 200, typeof(OkObjectResult))]
        [TestCase("Error in service", 400, typeof(BadRequestObjectResult))]
        public void CreateSales_Test(string message, int expectedStatusCode, Type expectedResultType)
        {
            var request = new SalesRequest { Client = new ClientRequest { FullName = "", TaxNumber = "" } };
            var response = new CallResponse();
            if (message != "Completed") response.AddMessage(message);

            _salesRequestValidator.Setup(m => m.ValidateAsync(It.IsAny<SalesRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());
            _saleService.Setup(m => m.Create(request)).ReturnsAsync(response);

            var salesController = CreateController();

            var result = salesController.CreateSale(request).Result;

            result.Should().BeOfType(expectedResultType);
            var objectResult = result as ObjectResult;
            objectResult.StatusCode.Should().Be(expectedStatusCode);
        }

    }
}
