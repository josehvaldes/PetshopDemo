using Azure;
using FluentAssertions;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement;
using NSubstitute;
using NUnit.Framework;
using PetShop.Application.Interfaces.Repository;
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
        private SalesController _salesController = null!;

        private IUserService _userServiceMock = null!;
        private IProductRepository _productRepositoryMock = null!;
        private IClientService _clientServiceMock = null!;
        
        private ISaleRepository _salesRepository = null!;
        private IFeatureManager _featureManager = null!;
        private IValidator<SalesRequest> _salesRequestValidator = null!;
        private TestLogger<SaleService> _loggerServiceMock = null!;
        private TestLogger<SalesController> _loggerControllerMock = null!;


        private SaleService CreateSaleService()
        {
            return new SaleService(_userServiceMock,
                _productRepositoryMock,
                _clientServiceMock,
                _salesRepository,
                _loggerServiceMock);
        }

        private SalesController CreateSaleController()
        {
            var saleService = CreateSaleService();
            var controller = new SalesController(_loggerControllerMock,
                _salesRequestValidator, 
                saleService,
                _featureManager
                );

            var context = new DefaultHttpContext();
            context.Items.Add("User", new AuthUser { UserName = "local", Domain = string.Empty });
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
            _productRepositoryMock = Substitute.For<IProductRepository>();
            _clientServiceMock = Substitute.For<IClientService>();
            _featureManager = Substitute.For<IFeatureManager>();

            _salesRepository = new SaleRepository();
            _salesRequestValidator = new SalesRequestValidator();
            
        }

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

        [TestCase("Completed", 200, typeof(OkObjectResult))]
        //[TestCase("Error in service", 400, typeof(BadRequestObjectResult))]
        public void CreateSale_Test(string message, int expectedStatusCode, Type expectedResultType) 
        {

            var user = new User();
            var product = new Product()
            {
                stock = 100,
                unitaryprice = 49.9,
                name = "dog chow"
            };
            var client = new Client()
            {
                fullname = "test",
                taxnumber = "123456789"
            };

            _userServiceMock.Retrieve(Arg.Any<string>(), Arg.Any<string>()).Returns(Task.FromResult<User?>(user));
            _productRepositoryMock.Retrieve(Arg.Any<string>(), Arg.Any<string>()).Returns(Task.FromResult<Product?>(product));
            _clientServiceMock.Retrieve(Arg.Any<string>()).Returns(Task.FromResult<Client?>(client));
            _productRepositoryMock.Update(Arg.Any<Product>()).Returns(Task.FromResult(true));


            var saleRequest = GetSalesRequest();

            var controller = CreateSaleController();
            
            var result = controller.CreateSale(saleRequest).Result;

            result.Should().BeOfType(expectedResultType);
            var objectResult = result as ObjectResult;
            objectResult.StatusCode.Should().Be(expectedStatusCode);
        }
    }
}
