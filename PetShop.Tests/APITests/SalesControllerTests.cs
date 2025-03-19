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
        private ILogger<SalesController> _logger;
        private Mock<IValidator<SalesRequest>> _salesRequestValidator;
        private Mock<ISaleService> _saleService;
        private Mock<IFeatureManager> _featureManager;

        public SalesControllerTests ()
        {
            _logger = new TestLogger<SalesController>();
            _salesRequestValidator = new Mock<IValidator<SalesRequest>>();
            _saleService = new Mock<ISaleService>();
            _featureManager = new Mock<IFeatureManager>();
        }

        [Test]
        public void RetrieveSales_Success()
        {
            string domain = "bo";
            List<SaleEntity> list = new List<SaleEntity>() { new SaleEntity() { domain="bo", saleid = "1" } };
            _saleService.Setup(m => m.RetrieveList(domain)).ReturnsAsync(list);

            var salesController = new SalesController(_logger, _salesRequestValidator.Object, _saleService.Object, _featureManager.Object);

            var result = salesController.RetrieveSales(domain).Result;
            result.Should().BeOfType<OkObjectResult>();
            OkObjectResult objectResult = (OkObjectResult)result;
            objectResult.StatusCode.Should().Be(200);
            IEnumerable<SaleEntity> value = (IEnumerable<SaleEntity>)objectResult.Value;
            value.Should().HaveCount(1);
            value.First().domain.Should().Be("bo");
        }

        [Test]
        public void CreateSales_Success() 
        {
            

            SalesRequest? request = new SalesRequest() { Client = new ClientRequest() { FullName="", TaxNumber ="" } };
            CallResponse? response = new CallResponse() { };

            _salesRequestValidator.Setup(m => m.ValidateAsync(It.IsAny<SalesRequest>(),It.IsAny<CancellationToken>()))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());

            _saleService.Setup(m => m.Create(request)).ReturnsAsync(response);

            var salesController = new SalesController( _logger, _salesRequestValidator.Object, _saleService.Object, _featureManager.Object );

            var context = new DefaultHttpContext();
            context.Items.Add("User", new User() { UserName = "local", Domain = string.Empty });

            salesController.ControllerContext.HttpContext = context ;

            var result = salesController.CreateSale(request).Result;

            result.Should().BeOfType<OkObjectResult>();
            OkObjectResult objectResult = (OkObjectResult)result;
            objectResult.StatusCode.Should().Be(200);
            dynamic value = objectResult.Value;
            var status = (string)value.status;
            status.Should().Be("Completed");
        }

        [Test]
        public void CreateSales_BadRequest()
        {
            SalesRequest? request = new SalesRequest() { Client = new ClientRequest() { FullName = "", TaxNumber = "" } };
            CallResponse? response = new CallResponse() { };
            response.AddMessage("Error in service");

            _salesRequestValidator.Setup(m => m.ValidateAsync(It.IsAny<SalesRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());

            _saleService.Setup(m => m.Create(request)).ReturnsAsync(response);
            
            var context = new DefaultHttpContext();
            context.Items.Add("User", new User() { UserName = "local", Domain = string.Empty });

            var salesController = new SalesController(_logger, _salesRequestValidator.Object, _saleService.Object, _featureManager.Object);
            salesController.ControllerContext.HttpContext = context;

            var result = salesController.CreateSale(request).Result;

            result.Should().BeOfType<BadRequestObjectResult>();
            BadRequestObjectResult objectResult = (BadRequestObjectResult)result;
            objectResult.StatusCode.Should().Be(400);
            dynamic value = objectResult.Value;
            var errorArray = (List<string>)value.Error;
            Assert.That(errorArray, Has.Exactly(1).EqualTo("Error in service"));
        }

    }
}
