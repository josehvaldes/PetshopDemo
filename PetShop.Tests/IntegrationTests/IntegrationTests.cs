using Azure;
using Cortex.Mediator;
using FluentAssertions;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement;
using NSubstitute;
using NUnit.Framework;
using PetShop.Application.Interfaces.Repository;
using PetShop.Application.Interfaces.Repository.Products;
using PetShop.Application.Interfaces.Services;
using PetShop.Application.Requests;
using PetShop.Application.Services;
using PetShop.Domain.Entities;
using PetShop.Infrastructure.Sqlite.Repository;
using PetShop.Tests.ServiceTests;
using PetShopSaleAPI.Controllers;
using PetShopSalesAPI.Auth;
using PetShopSalesAPI.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetShop.Tests.IntegrationTests
{
    public class IntegrationTests
    {
        private IUserService _userServiceMock = null!;
        private IProductCommand _productCommandMock = null!;
        private IProductQuery _productQueryMock = null!;
        private IClientService _clientServiceMock = null!;        
        private ISaleRepository _salesRepository = null!;
        private IFeatureManager _featureManager = null!;
        private IValidator<SalesRequest> _salesRequestValidator = null!;
        private TestLogger<SaleService> _loggerServiceMock = null!;
        private TestLogger<SalesController> _loggerControllerMock = null!;
        private IMediator _mediator = null!;

        private SaleService CreateSaleService()
        {
            return new SaleService(_userServiceMock,
                _productCommandMock,
                _productQueryMock,
                _clientServiceMock,
                _salesRepository,
                _loggerServiceMock, 
                _mediator);
        }

        /// <summary>
        /// Creates a SalesController instance with mocked dependencies and a test user context.
        /// </summary>
        /// <returns></returns>
        private SalesController CreateSaleController()
        {
            var saleService = CreateSaleService();
            var controller = new SalesController(_loggerControllerMock,
                _salesRequestValidator, 
                saleService,
                _featureManager
                );

            var context = new DefaultHttpContext();
            context.Items.Add("User", new AuthUser { UserName = "local", Domain = "testDomain" });
            controller.ControllerContext.HttpContext = context;
            return controller;
        }

        [SetUp]
        public void SetUp()
        {
            // Setup code if needed
            _loggerControllerMock = new TestLogger<SalesController>();
            _loggerServiceMock = new TestLogger<SaleService>();
            
            _userServiceMock = Substitute.For<IUserService>();
            _productCommandMock = Substitute.For<IProductCommand>();
            _productQueryMock = Substitute.For<IProductQuery>();
            _clientServiceMock = Substitute.For<IClientService>();
            _featureManager = Substitute.For<IFeatureManager>();
            _mediator = Substitute.For<IMediator>();
            
            // Initialize repositories and validators
            _salesRepository = new SaleRepository();
            _salesRequestValidator = new SalesRequestValidator();
            
        }

        /// <summary>
        /// Creates a SalesRequest object with test data for integration tests.
        /// </summary>
        /// <returns></returns>
        private SalesRequest GetSalesRequest() 
        {
            var salesRequest = new SalesRequest
            {
                Domain = "testDomain",
                Price = 499,
                Quantity = 10,
                Username = "testUser",
                ProductName = "dog chow",
                Client = new ClientRequest
                {
                    FullName = "test",
                    TaxNumber = "123456789"
                }
            };

            return salesRequest;
        }

        /// <summary>
        /// Creates a Product object with test data for integration tests.
        /// </summary>
        /// <returns></returns>
        private Product GetProduct() 
        {
            return new Product()
            {
                stock = 100,
                unitaryprice = 49.9,
                name = "dog chow"
            };
        }
        private Client GetClient() 
        {
            return new Client()
            {
                fullname = "test",
                taxnumber = "123456789"
            };
        }

        [TestCase("Completed", 201, typeof(CreatedResult))]
        public void CreateSale_Test(string message, int expectedStatusCode, Type expectedResultType) 
        {
            var user = new User();

            var product = GetProduct();
            var client = GetClient();

            _userServiceMock.Retrieve(Arg.Any<string>(), Arg.Any<string>()).Returns(Task.FromResult<User?>(user));
            _productQueryMock.Retrieve(Arg.Any<string>(), Arg.Any<string>()).Returns(Task.FromResult<Product?>(product));
            _clientServiceMock.Retrieve(Arg.Any<string>()).Returns(Task.FromResult<Client?>(client));
            _productCommandMock.Update(Arg.Any<Product>()).Returns(Task.FromResult(true));


            var saleRequest = GetSalesRequest();

            var controller = CreateSaleController();
            
            var result = controller.CreateSale(saleRequest).Result;

            result.Should().BeOfType(expectedResultType);
            var objectResult = result as ObjectResult;
            objectResult.StatusCode.Should().Be(expectedStatusCode);

            var list = _salesRepository.RetrieveList(saleRequest.Domain).Result;
            list.Count().Should().Be(1);

        }


        [TestCase("Completed", 200, typeof(OkObjectResult))]
        public void RetrieveSale_Test(string message, int expectedStatusCode, Type expectedResultType) 
        {
            CreateSale_Test(message, 201, typeof(CreatedResult));
            
            var saleRequest = GetSalesRequest();
            var controller = CreateSaleController();

            var result = controller.RetrieveSales(saleRequest.Domain).Result;
            result.Should().BeOfType<OkObjectResult>();
            OkObjectResult objectResult = (OkObjectResult)result;
            objectResult.StatusCode.Should().Be(expectedStatusCode);

            IEnumerable<Sale> value = (IEnumerable<Sale>)objectResult.Value;
            value.Should().NotBeNullOrEmpty();
            value.Should().HaveCount(1);
            value.First().domain.Should().Be(saleRequest.Domain);
        }
    }
}
