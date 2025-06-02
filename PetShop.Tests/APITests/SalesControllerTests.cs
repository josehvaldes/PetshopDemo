using FluentValidation;
using Microsoft.Extensions.Logging;
using PetShopAPI.Controllers;
using Microsoft.FeatureManagement;
using NUnit.Framework;
using PetShop.Tests.ServiceTests;

using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using PetShopSalesAPI.Auth;
using PetShop.Application.Requests;
using PetShop.Domain.Entities;
using PetShop.Application.Interfaces.Services;
using NSubstitute;
using PetShop.Application.Services;
using NSubstitute.ExceptionExtensions;
using PetShopSalesAPI.Validators;
using PetShopSaleAPI.Controllers;


namespace PetShop.Tests.APITests
{
    
    public class SalesControllerTests
    {
        private ILogger<SalesController> _logger = null!;
        private IValidator<SalesRequest> _salesRequestValidator = null!;
        private ISaleService _saleService = null!;
        private IFeatureManager _featureManager = null!;

        private SalesController CreateController()
        {
            var controller = new SalesController(_logger, _salesRequestValidator, _saleService, _featureManager);
            var context = new DefaultHttpContext();
            context.Items.Add("User", new AuthUser { UserName = "local", Domain = string.Empty });
            controller.ControllerContext.HttpContext = context;
            return controller;
        }

        [SetUp]
        public void SetUp()
        {
            _logger = new TestLogger<SalesController>();
            _salesRequestValidator = Substitute.For<IValidator<SalesRequest>>();
            _saleService = Substitute.For<ISaleService>();
            _featureManager = Substitute.For<IFeatureManager>();
        }


        [Test]
        public void RetrieveSales_WithValidDomain_ReturnsOkWithSalesList()
        {
            string domain = "bo";
            List<Sale> list = new List<Sale>() { new Sale() { domain="bo", saleid = "1" } };
            _saleService.RetrieveList(Arg.Any<string>()).Returns(list);

            var salesController = CreateController();


            var result = salesController.RetrieveSales(domain).Result;
            result.Should().BeOfType<OkObjectResult>();
            OkObjectResult objectResult = (OkObjectResult)result;
            objectResult.StatusCode.Should().Be(200);
            IEnumerable<Sale> value = (IEnumerable<Sale>)objectResult.Value;
            value.Should().NotBeNullOrEmpty();
            value.Should().HaveCount(1);
            value.First().domain.Should().Be("bo");
        }


        [Test]
        public void RetrieveSales_ServiceThrowsException_ReturnsInternalServerError()
        {
            string domain = "bo";
            _saleService.RetrieveList(Arg.Any<string>()).Throws(new Exception("Service error"));

            var salesController = CreateController();

            var result = salesController.RetrieveSales(domain).Result;

            result.Should().BeOfType<ObjectResult>();
            var objectResult = (ObjectResult)result;
            objectResult.StatusCode.Should().Be(500);
            dynamic obj = objectResult.Value;
            //obj.Should().NotBeNull();
            string[] errors = obj.Error;
            errors.Should().NotBeNullOrEmpty();
            errors.Should().Contain("Service error");

        }

        [TestCase("Completed", 201, typeof(CreatedResult))]
        [TestCase("Error in service", 400, typeof(BadRequestObjectResult))]
        public void CreateSales_Test(string message, int expectedStatusCode, Type expectedResultType)
        {
            var request = new SalesRequest { Client = new ClientRequest { FullName = "", TaxNumber = "" } };
            var response = new CallResponse();
            if (message != "Completed") response.AddMessage(message);

            _salesRequestValidator.ValidateAsync(request, CancellationToken.None)
                .Returns(new FluentValidation.Results.ValidationResult());

            _saleService.Create(request).Returns(response);

            var salesController = CreateController();

            var result = salesController.CreateSale(request).Result;

            result.Should().BeOfType(expectedResultType);
            var objectResult = result as ObjectResult;
            objectResult.StatusCode.Should().Be(expectedStatusCode);
        }

    }
}
