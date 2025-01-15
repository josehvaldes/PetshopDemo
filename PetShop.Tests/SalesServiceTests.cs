using FluentAssertions;
using Moq;
using NUnit.Framework;
using PetShop.Data;
using PetShop.Model;
using PetShop.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetShop.Tests
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

            var userEntity = new UserEntity();
            var productEntity = new ProductEntity();

            userServiceMock.Setup(m=> m.Retrieve(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult<UserEntity?>(null));
            productRepositoryMock.Setup( m=> m.Retrieve(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult<ProductEntity?>(productEntity));

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

            var userEntity = new UserEntity();

            userServiceMock.Setup(m => m.Retrieve(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult<UserEntity?>(userEntity));
            productRepositoryMock.Setup(m => m.Retrieve(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult<ProductEntity?>(null));

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

            var userEntity = new UserEntity();
            var productEntity = new ProductEntity();
            var clientEntity = new ClientEntity() {
                fullname="mismatch"
            };

            userServiceMock.Setup(m => m.Retrieve(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult<UserEntity?>(userEntity));
            productRepositoryMock.Setup(m => m.Retrieve(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult<ProductEntity?>(productEntity));
            clientServiceMock.Setup(m => m.Retrieve(It.IsAny<string>())).Returns(Task.FromResult<ClientEntity?>(clientEntity));

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

            var userEntity = new UserEntity();
            var productEntity = new ProductEntity() { 
                stock=10,
                name= "dog chow"
            };
            var clientEntity = new ClientEntity()
            {
                fullname = "test"
            };

            userServiceMock.Setup(m => m.Retrieve(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult<UserEntity?>(userEntity));
            productRepositoryMock.Setup(m => m.Retrieve(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult<ProductEntity?>(productEntity));
            clientServiceMock.Setup(m => m.Retrieve(It.IsAny<string>())).Returns(Task.FromResult<ClientEntity?>(clientEntity));

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
            response.Messages.Should().Contain($"Stock unavailable. Product {productEntity.name} stock: {productEntity?.stock}");

        }


        [Test]
        public void Test_CreateSale_PriceMismath()
        {

            var userServiceMock = new Mock<IUserService>();
            var productRepositoryMock = new Mock<IProductRepository>();
            var clientServiceMock = new Mock<IClientService>();
            var salesRepositoryMock = new Mock<ISaleRepository>();
            var loggerMock = new TestLogger<ISaleService>();

            var userEntity = new UserEntity();
            var productEntity = new ProductEntity()
            {
                stock = 10,
                unitaryprice = 49.9,
                name = "dog chow"
            };
            var clientEntity = new ClientEntity()
            {
                fullname = "test"
            };

            userServiceMock.Setup(m => m.Retrieve(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult<UserEntity?>(userEntity));
            productRepositoryMock.Setup(m => m.Retrieve(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult<ProductEntity?>(productEntity));
            clientServiceMock.Setup(m => m.Retrieve(It.IsAny<string>())).Returns(Task.FromResult<ClientEntity?>(clientEntity));

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

            var userEntity = new UserEntity();
            var productEntity = new ProductEntity()
            {
                stock = 10,
                unitaryprice = 49.9,
                name = "dog chow"
            };
            var clientEntity = new ClientEntity()
            {
                fullname = "test"
            };

            userServiceMock.Setup(m => m.Retrieve(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult<UserEntity?>(userEntity));
            productRepositoryMock.Setup(m => m.Retrieve(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult<ProductEntity?>(productEntity));
            clientServiceMock.Setup(m => m.Retrieve(It.IsAny<string>())).Returns(Task.FromResult<ClientEntity?>(clientEntity));

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

            var userEntity = new UserEntity();
            var productEntity = new ProductEntity()
            {
                stock = 10,
                unitaryprice = 49.9,
                name = "dog chow"
            };
            var clientEntity = new ClientEntity()
            {
                fullname = "test"
            };
            var saleEntity = new SaleEntity() {
                saleid = "98-989-9",
            };

            userServiceMock.Setup(m => m.Retrieve(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult<UserEntity?>(userEntity));
            productRepositoryMock.Setup(m => m.Retrieve(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult<ProductEntity?>(productEntity));
            clientServiceMock.Setup(m => m.Retrieve(It.IsAny<string>())).Returns(Task.FromResult<ClientEntity?>(clientEntity));
            salesRepositoryMock.Setup(m=>m.Create(It.IsAny<SaleEntity>())).Returns(Task.FromResult<SaleEntity?>(saleEntity));
            productRepositoryMock.Setup(m => m.Update(It.IsAny<ProductEntity>())).ReturnsAsync(true);

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
            productEntity.stock.Should().Be(8);

            response.SaleId.Should().Be(saleEntity.saleid);

            response.Should().NotBeNull();
            response.Messages.Should().BeEmpty();

        }
    }
}
