using Azure;
using FluentAssertions;
using Microsoft.Extensions.Logging;
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

namespace PetShop.Tests.ServiceTests
{
    [TestFixture]
    public class ClientServiceTests
    {
        [Test]
        public void Test_Create_Client_Valid_GUID() 
        {
            var request = new ClientRequest()
            {
                TaxNumber = "654123",
                FullName = "Test test"
            };

            var entity = new ClientEntity() 
            {
                guid = Guid.NewGuid().ToString(),
                ETag = ETag.All,
                fullname = request.FullName,
                taxnumber = request.TaxNumber,
            };
            var loggerMock = new TestLogger<ClientService>();
            var clientRepositoryMock = new Mock<IClientRepository>();
            clientRepositoryMock.Setup(m => m.Create(It.IsAny<ClientEntity>())).Returns(Task.FromResult<ClientEntity?>(entity));

            var clientService = new ClientService(loggerMock, clientRepositoryMock.Object);
            var entityResult = clientService.Create(request).Result;

            entityResult.Should().NotBeNull();
            entityResult.guid.Should().NotBeNullOrEmpty();
        }

        [Test]
        public void Test_Delete_Client()
        {
            var entity = new ClientEntity()
            {
                guid = Guid.NewGuid().ToString(),
                ETag = ETag.All,
                fullname = "test",
                taxnumber = "654321",
            };
            var loggerMock = new TestLogger<ClientService>();
            var clientRepositoryMock = new Mock<IClientRepository>();
            clientRepositoryMock.Setup(m=> m.Retrieve(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(entity);
            clientRepositoryMock.Setup(m => m.Delete(It.IsAny<ClientEntity>())).ReturnsAsync(true);

            var clientService = new ClientService(loggerMock, clientRepositoryMock.Object);
            var entityResult = clientService.Delete("654321").Result;
            entityResult.Should().Be(true);
        }

        [Test]
        public void Test_Delete_Client_Failed()
        {
            var taxNumber = "654321";
            var loggerMock = new TestLogger<ClientService>();
            var clientRepositoryMock = new Mock<IClientRepository>();
            clientRepositoryMock.Setup(m => m.Retrieve(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync((ClientEntity?)null);
            clientRepositoryMock.Setup(m => m.Delete(It.IsAny<ClientEntity>())).ReturnsAsync(true);

            var clientService = new ClientService(loggerMock, clientRepositoryMock.Object);
            var entityResult = clientService.Delete(taxNumber).Result;
            entityResult.Should().Be(false);
            loggerMock.Messages.Should().HaveCount(1);
            loggerMock.Messages.Should().Contain($"Delete failed. Client not Found: {taxNumber}");
        }
    }
}
