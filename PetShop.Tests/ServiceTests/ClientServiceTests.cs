using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using PetShop.Application.Interfaces.Repository;
using PetShop.Application.Interfaces.Services;
using PetShop.Application.Requests;
using PetShop.Application.Services;
using PetShop.Domain.Entities;

namespace PetShop.Tests.ServiceTests
{
    [TestFixture]
    public class ClientServiceTests
    {
        private TestLogger<ClientService> _loggerMock = new TestLogger<ClientService>();
        private IClientRepository _clientRepository = null!;
        private IClientService CreateClientService() 
        {
            var clientService = new ClientService(_loggerMock, _clientRepository);
            return clientService;
        }

        [SetUp]
        public void SetUp() 
        {
            _loggerMock = new TestLogger<ClientService>();
            _clientRepository = Substitute.For<IClientRepository>();
        }

        [Test]
        public void Test_Create_Client_Valid_GUID() 
        {
            var request = new ClientRequest()
            {
                TaxNumber = "654123",
                FullName = "Test test"
            };

            var entity = new Client() 
            {
                guid = Guid.NewGuid().ToString(),
                fullname = request.FullName,
                taxnumber = request.TaxNumber,
            };

            _clientRepository.Create(Arg.Any<Client>()).Returns(true);

            var clientService = CreateClientService();
            var entityResult = clientService.Create(request).Result;

            entityResult.Should().NotBeNull();
            entityResult.guid.Should().NotBeNullOrEmpty();
            entityResult.taxnumberend.Should().Be("3");

        }

        [Test]
        public void Test_Create_Client_Failed_Message()
        {
            var request = new ClientRequest()
            {
                TaxNumber = "654123",
                FullName = "Test test"
            };

            var entity = new Client()
            {
                guid = Guid.NewGuid().ToString(),
                fullname = request.FullName,
                taxnumber = request.TaxNumber,
            };

            _clientRepository.Create(Arg.Any<Client>()).Returns(false);

            var clientService = CreateClientService();
            var entityResult = clientService.Create(request).Result;

            entityResult.Should().BeNull();
            _loggerMock.Messages.Should().HaveCount(1);
        }

        [Test]
        public void Test_Delete_Client()
        {
            var entity = new Client()
            {
                guid = Guid.NewGuid().ToString(),
                fullname = "test",
                taxnumber = "654321",
            };

            _clientRepository.Delete(Arg.Any<Client>()).Returns(true);
            _clientRepository.Retrieve(Arg.Any<string>(), Arg.Any<string>()).Returns(entity);

            var clientService = CreateClientService();
            var entityResult = clientService.Delete("654321").Result;
            entityResult.Should().Be(true);
        }

        [Test]
        public void Test_Delete_Client_Failed()
        {
            var taxNumber = "654321";
            
            _clientRepository.Retrieve(Arg.Any<string>(), Arg.Any<string>()).Returns((Client?)null);
            _clientRepository.Delete(Arg.Any<Client>()).Returns(true);

            var clientService = CreateClientService(); 
            var entityResult = clientService.Delete(taxNumber).Result;
            entityResult.Should().Be(false);
            _loggerMock.Messages.Should().HaveCount(1);
            _loggerMock.Messages.Should().Contain($"Delete failed. Client not Found: {taxNumber}");
        }
    }
}
