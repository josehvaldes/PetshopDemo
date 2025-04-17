using FluentAssertions;
using Moq;
using NUnit.Framework;
using PetShop.Application.Interfaces;
using PetShop.Application.Requests;
using PetShop.Application.Services;
using PetShop.Domain.Entities;

namespace PetShop.Tests.ServiceTests
{
    [TestFixture]
    public class SalesServiceTests
    {

        [Test]
        public void Test_CreateSale_UserNotFound() 
        {
            
            var userServiceMock = new Mock<IUserService>();
            var productRepositoryMock = new Mock<IProductRepository>();
            var clientServiceMock = new Mock<IClientService>();
            var salesRepositoryMock = new Mock<ISaleRepository>();
            var loggerMock = new TestLogger<ISaleService>();

            var User = new User();
            var Product = new Product();

            userServiceMock.Setup(m=> m.Retrieve(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync((User?)null);
                productRepositoryMock.Setup(m => m.Retrieve(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(Product);

            var saleService = new SaleService(userServiceMock.Object, 
                productRepositoryMock.Object, 
                clientServiceMock.Object,
                salesRepositoryMock.Object,
                loggerMock);


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

            var userServiceMock = new Mock<IUserService>();
            var productRepositoryMock = new Mock<IProductRepository>();
            var clientServiceMock = new Mock<IClientService>();
            var salesRepositoryMock = new Mock<ISaleRepository>();
            var loggerMock = new TestLogger<ISaleService>();

            var User = new User();

            userServiceMock.Setup(m => m.Retrieve(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(User);
            productRepositoryMock.Setup(m => m.Retrieve(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync((Product?)null);

            var saleService = new SaleService(userServiceMock.Object,
                productRepositoryMock.Object,
                clientServiceMock.Object,
                salesRepositoryMock.Object,
                loggerMock);


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

            var userServiceMock = new Mock<IUserService>();
            var productRepositoryMock = new Mock<IProductRepository>();
            var clientServiceMock = new Mock<IClientService>();
            var salesRepositoryMock = new Mock<ISaleRepository>();
            var loggerMock = new TestLogger<ISaleService>();

            var User = new User();
            var Product = new Product();
            var client = new Client() {
                fullname="mismatch"
            };

            userServiceMock.Setup(m => m.Retrieve(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(User);
            productRepositoryMock.Setup(m => m.Retrieve(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(Product);
            clientServiceMock.Setup(m => m.Retrieve(It.IsAny<string>())).ReturnsAsync(client);

            var saleService = new SaleService(userServiceMock.Object,
                productRepositoryMock.Object,
                clientServiceMock.Object,
                salesRepositoryMock.Object,
                loggerMock);


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

            var userServiceMock = new Mock<IUserService>();
            var productRepositoryMock = new Mock<IProductRepository>();
            var clientServiceMock = new Mock<IClientService>();
            var salesRepositoryMock = new Mock<ISaleRepository>();
            var loggerMock = new TestLogger<ISaleService>();

            var User = new User();
            var Product = new Product() { 
                stock=10,
                name= "dog chow"
            };
            var Client = new Client()
            {
                fullname = "test"
            };

            userServiceMock.Setup(m => m.Retrieve(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(User);
            productRepositoryMock.Setup(m => m.Retrieve(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(Product);
            clientServiceMock.Setup(m => m.Retrieve(It.IsAny<string>())).ReturnsAsync(Client);

            var saleService = new SaleService(userServiceMock.Object,
                productRepositoryMock.Object,
                clientServiceMock.Object,
                salesRepositoryMock.Object,
                loggerMock);


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

            var userServiceMock = new Mock<IUserService>();
            var productRepositoryMock = new Mock<IProductRepository>();
            var clientServiceMock = new Mock<IClientService>();
            var salesRepositoryMock = new Mock<ISaleRepository>();
            var loggerMock = new TestLogger<ISaleService>();

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

            userServiceMock.Setup(m => m.Retrieve(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(User);
            productRepositoryMock.Setup(m => m.Retrieve(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(Product);
            clientServiceMock.Setup(m => m.Retrieve(It.IsAny<string>())).ReturnsAsync(Client);

            var saleService = new SaleService(userServiceMock.Object,
                productRepositoryMock.Object,
                clientServiceMock.Object,
                salesRepositoryMock.Object,
                loggerMock);


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

            var userServiceMock = new Mock<IUserService>();
            var productRepositoryMock = new Mock<IProductRepository>();
            var clientServiceMock = new Mock<IClientService>();
            var salesRepositoryMock = new Mock<ISaleRepository>();
            var loggerMock = new TestLogger<ISaleService>();

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

            userServiceMock.Setup(m => m.Retrieve(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(User);
            productRepositoryMock.Setup(m => m.Retrieve(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(Product);
            clientServiceMock.Setup(m => m.Retrieve(It.IsAny<string>())).ReturnsAsync(Client);

            var saleService = new SaleService(userServiceMock.Object,
                productRepositoryMock.Object,
                clientServiceMock.Object,
                salesRepositoryMock.Object,
                loggerMock);


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

            var userServiceMock = new Mock<IUserService>();
            var productRepositoryMock = new Mock<IProductRepository>();
            var clientServiceMock = new Mock<IClientService>();
            var salesRepositoryMock = new Mock<ISaleRepository>();
            var loggerMock = new TestLogger<ISaleService>();

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

            userServiceMock.Setup(m => m.Retrieve(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(User);
            productRepositoryMock.Setup(m => m.Retrieve(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(Product);
            clientServiceMock.Setup(m => m.Retrieve(It.IsAny<string>())).ReturnsAsync(Client);
            salesRepositoryMock.Setup(m=>m.Create(It.IsAny<Sale>())).Returns(Task.FromResult<Sale?>(Sale));
            productRepositoryMock.Setup(m => m.Update(It.IsAny<Product>())).ReturnsAsync(true);

            var saleService = new SaleService(userServiceMock.Object,
                productRepositoryMock.Object,
                clientServiceMock.Object,
                salesRepositoryMock.Object,
                loggerMock);


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

            response.SaleId.Should().Be(Sale.saleid);

            response.Should().NotBeNull();
            response.Messages.Should().BeEmpty();

        }
    }
}
