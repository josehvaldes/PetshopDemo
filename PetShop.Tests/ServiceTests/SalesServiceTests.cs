using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using PetShop.Application.Interfaces.Repository;
using PetShop.Application.Interfaces.Repository.Products;
using PetShop.Application.Interfaces.Services;
using PetShop.Application.Requests;
using PetShop.Application.Services;
using PetShop.Domain.Entities;

namespace PetShop.Tests.ServiceTests
{
    [TestFixture]
    public class SalesServiceTests
    {

        private IUserService _userServiceMock = null!;
        private IProductCommand _productCommandMock = null!;
        private IProductQuery _productQueryMock = null!;
        private IClientService _clientServiceMock = null!;
        private ISaleRepository _salesRepositoryMock = null!;
        private TestLogger<SaleService> _loggerMock = null!;


        private SaleService CreateSaleService() 
        {
            return new SaleService(_userServiceMock,
                _productCommandMock, 
                _productQueryMock,
                _clientServiceMock,
                _salesRepositoryMock,
                _loggerMock);
        }

        [SetUp]
        public void SetUp()
        {
            // Setup code if needed
            _loggerMock = new TestLogger<SaleService>();
            _userServiceMock = Substitute.For<IUserService>();
            _productCommandMock = Substitute.For<IProductCommand>();
            _productQueryMock = Substitute.For<IProductQuery>();
            _clientServiceMock = Substitute.For<IClientService>();
            _salesRepositoryMock = Substitute.For<ISaleRepository>();
        }

        [Test]
        public void Test_CreateSale_UserNotFound() 
        {
            var User = new User();
            var Product = new Product();

            _userServiceMock.Retrieve(Arg.Any<string>(), Arg.Any<string>()).Returns(Task.FromResult<User?>(null));
            _productQueryMock.Retrieve(Arg.Any<string>(), Arg.Any<string>()).Returns(Product);

            var saleService = CreateSaleService();

            var request = new SalesRequest() { 
                Domain="bo",
                Username="admin",
                Client = new ClientRequest() 
            };

            var response = saleService.Create(request).Result;
            response.Should().NotBeNull();
            response.Messages.Should().HaveCount(1);
            response.Messages.Should().Contain($"User {request.Domain}/{request.Username} not found");

        }


        [Test]
        public void Test_CreateSale_ProductNotFound()
        {
            var User = new User() { };

            _userServiceMock.Retrieve(Arg.Any<string>(), Arg.Any<string>()).Returns(Task.FromResult<User?>(User));
            _productQueryMock.Retrieve(Arg.Any<string>(), Arg.Any<string>()).Returns(Task.FromResult<Product?>(null));

            var saleService = CreateSaleService();


            var request = new SalesRequest()
            {
                Domain = "bo",
                Username = "admin",
                ProductName="dog chow",
                Client = new ClientRequest()
            };

            var response = saleService.Create(request).Result;
            response.Should().NotBeNull();
            response.Messages.Should().HaveCount(1);
            response.Messages.Should().Contain($"Product {request.ProductName} doesn't exists.");

        }


        [Test]
        public void Test_CreateSale_ClientNameMismatch()
        {

            var User = new User();
            var Product = new Product();
            var client = new Client() {
                fullname="mismatch"
            };

            _userServiceMock.Retrieve(Arg.Any<string>(), Arg.Any<string>()).Returns(Task.FromResult<User?>(User));
            _productQueryMock.Retrieve(Arg.Any<string>(), Arg.Any<string>()).Returns(Task.FromResult<Product?>(Product));
            _clientServiceMock.Retrieve(Arg.Any<string>()).Returns(Task.FromResult<Client?>(client));

            var saleService = CreateSaleService();

            var request = new SalesRequest()
            {
                Domain = "bo",
                Username = "admin",
                ProductName = "dog chow",
                Client = new ClientRequest() 
                {
                    TaxNumber="123",
                    FullName="test"
                }
            };

            var response = saleService.Create(request).Result;
            response.Should().NotBeNull();
            response.Messages.Should().HaveCount(1);
            response.Messages.Should().Contain($"Client.FullName {request.Client.FullName} doesn't match.");

        }

        [Test]
        public void Test_CreateSale_StockUnavailable()
        {
            var User = new User();
            var Product = new Product() { 
                stock=10,
                name= "dog chow"
            };
            var Client = new Client()
            {
                fullname = "test"
            };

            _userServiceMock.Retrieve(Arg.Any<string>(), Arg.Any<string>()).Returns(Task.FromResult<User?>(User));
            _productQueryMock.Retrieve(Arg.Any<string>(), Arg.Any<string>()).Returns(Task.FromResult<Product?>(Product));
            _clientServiceMock.Retrieve(Arg.Any<string>()).Returns(Task.FromResult<Client?>(Client));

            var saleService = CreateSaleService();

            var request = new SalesRequest()
            {
                Domain = "bo",
                Username = "admin",
                ProductName = "dog chow",
                Quantity = 12,
                Client = new ClientRequest()
                {
                    TaxNumber = "123",
                    FullName = "test"
                }
            };

            var response = saleService.Create(request).Result;
            response.Should().NotBeNull();
            response.Messages.Should().HaveCount(1);
            response.Messages.Should().Contain($"Stock unavailable. Product {Product.name} stock: {Product?.stock}");

        }


        [Test]
        public void Test_CreateSale_PriceMismath()
        {

            var User = new User();
            var Product = new Product()
            {
                stock = 10,
                unitaryprice = 49.9,
                name = "dog chow"
            };
            var Client = new Client()
            {
                fullname = "test"
            };

            _userServiceMock.Retrieve(Arg.Any<string>(), Arg.Any<string>()).Returns(Task.FromResult<User?>(User));
            _productQueryMock.Retrieve(Arg.Any<string>(), Arg.Any<string>()).Returns(Task.FromResult<Product?>(Product));
            _clientServiceMock.Retrieve(Arg.Any<string>()).Returns(Task.FromResult<Client?>(Client));

            var saleService = CreateSaleService(); ;


            var request = new SalesRequest()
            {
                Domain = "bo",
                Username = "admin",
                ProductName = "dog chow",
                Quantity = 2,
                Price = 90.8,
                Client = new ClientRequest()
                {
                    TaxNumber = "123",
                    FullName = "test"
                }
            };
            var expectedPrice = 99.8;
            var response = saleService.Create(request).Result;
            response.Should().NotBeNull();
            response.Messages.Should().HaveCount(1);
            response.Messages.Should().Contain($"Price doesn't match. Expected price (Unitary Price x Quantity): {expectedPrice}");

        }

        [Test]
        public void Test_CreateSale_Sales_NotCreated()
        {
            var User = new User();
            var Product = new Product()
            {
                stock = 10,
                unitaryprice = 49.9,
                name = "dog chow"
            };
            var Client = new Client()
            {
                fullname = "test"
            };

            _userServiceMock.Retrieve(Arg.Any<string>(), Arg.Any<string>()).Returns(Task.FromResult<User?>(User));
            _productQueryMock.Retrieve(Arg.Any<string>(), Arg.Any<string>()).Returns(Task.FromResult<Product?>(Product));
            _clientServiceMock.Retrieve(Arg.Any<string>()).Returns(Task.FromResult<Client?>(Client));

            var saleService = CreateSaleService();


            var request = new SalesRequest()
            {
                Domain = "bo",
                Username = "admin",
                ProductName = "dog chow",
                Quantity = 2,
                Price = 99.8,
                Client = new ClientRequest()
                {
                    TaxNumber = "123",
                    FullName = "test"
                }
            };
            var response = saleService.Create(request).Result;
            response.Messages.Should().HaveCount(1);
            response.Messages.Should().Contain($"Sales was not created. Review logs for more details");
        }


        [Test]
        public void Test_CreateSale_Sales_Success()
        {
            var User = new User();
            var Product = new Product()
            {
                stock = 10,
                unitaryprice = 49.9,
                name = "dog chow"
            };
            var Client = new Client()
            {
                fullname = "test"
            };
            var Sale = new Sale() {
                saleid = "98-989-9",
            };

            _userServiceMock.Retrieve(Arg.Any<string>(), Arg.Any<string>()).Returns(Task.FromResult<User?>(User));
            _productQueryMock.Retrieve(Arg.Any<string>(), Arg.Any<string>()).Returns(Task.FromResult<Product?>(Product));
            _clientServiceMock.Retrieve(Arg.Any<string>()).Returns(Task.FromResult<Client?>(Client));
            _salesRepositoryMock.Create(Arg.Any<Sale>()).Returns(Task.FromResult<bool>(true));
            _productCommandMock.Update(Arg.Any<Product>()).Returns(Task.FromResult(true));


            var saleService = CreateSaleService();

            var request = new SalesRequest()
            {
                Domain = "bo",
                Username = "admin",
                ProductName = "dog chow",
                Quantity = 2,
                Price = 99.8,
                Client = new ClientRequest()
                {
                    TaxNumber = "123",
                    FullName = "test"
                }
            };
            var response = saleService.Create(request).Result;

            //verify reduced stock
            Product.stock.Should().Be(8);

            response.SaleId.Should().NotBeNullOrEmpty();

            response.Should().NotBeNull();
            response.Messages.Should().BeEmpty();

        }
    }
}
