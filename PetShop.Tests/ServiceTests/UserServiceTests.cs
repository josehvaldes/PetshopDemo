using Azure;
using Microsoft.AspNetCore.Identity;
using NUnit.Framework;
using PetShop.Model;
using PetShopAPI.Auth;
using FluentAssertions;
using PetShop.Service;
using Moq;
using PetShop.Data;

namespace PetShop.Tests.ServiceTests
{
    [TestFixture]
    public class UserServiceTests
    {

        [Test]
        public void Test_Generate_Valid_Hash()
        {
            var identityHasher = new PasswordHasher<User>();
            var passwdHasher = new PasswdHasher(identityHasher);
            var hash = passwdHasher.HashPassword(new User { Id = "4e42250c-ca60-4180-803a-c587e4954352" }, "852963");
            hash.Should().NotBeNullOrEmpty();
        }

        [Test]
        public void Test_Verify_Hash()
        {
            string hash = "AQAAAAIAAYagAAAAELFW042L6ij5KVd4wf925JTkd0oimQMgkehOyVXgFv2P4iRPPRQun4CTA3VK5FX6hg==";
            string password = "852963";
            var identityHasher = new PasswordHasher<User>();
            var passwdHasher = new PasswdHasher(identityHasher);
            var roles = new[] { "Administrator", "User" };
            var isValid = passwdHasher.VerifyPassword(new User { Id = "4e42250c-ca60-4180-803a-c587e4954352", Roles = roles },
                hash, password);
            isValid.Should().Be(true);
        }


        [Test]
        public void Test_Create_UserEntity_Valid_GUID() 
        {
            var entity = new UserEntity()
            {
                email = "admin@petshotdemo.com",
                RowKey = "admin",
                PartitionKey = "bo",
                hash = "==",
            };

            var userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(m => m.Create(It.IsAny<UserEntity>())).Returns(Task.FromResult<UserEntity?>(entity));

            var userService = new UserService(userRepositoryMock.Object);
            var entityResult = userService.Create(entity).Result;

            entityResult.Should().NotBeNull();
            entityResult.guid.Should().NotBeNullOrEmpty();
        }

        [Test]
        public void Test_Create_UserEntity_Role_User()
        {
            var entity = new UserEntity()
            {
                email = "admin@petshotdemo.com",
                RowKey = "admin",
                PartitionKey = "bo",
                hash = "==",
            };

            var userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(m => m.Create(It.IsAny<UserEntity>())).ReturnsAsync(entity);

            var userService = new UserService(userRepositoryMock.Object);
            var entityResult = userService.Create(entity).Result;

            entityResult.Should().NotBeNull();
            entityResult.roles.Should().Be("User");
        }

        [Test]
        public void Test_Delete_UserEntity_NonAdministrator()
        {
            var entity = new UserEntity()
            {
                email = "usertest@petshotdemo.com",
                RowKey = "usertest",
                PartitionKey = "bo",
                hash = "==",
                roles = "User"
            };

            var userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(m => m.Retrieve(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(entity);
            userRepositoryMock.Setup(m => m.Delete(It.IsAny<UserEntity>())).ReturnsAsync(true);
            var userService = new UserService(userRepositoryMock.Object);

            var entityResult = userService.Delete("bo", "usertest").Result;

            entityResult.Should().NotBeNull();
            entityResult.Messages.Should().BeEmpty();            
        }

        [Test]
        public void Test_Delete_UserEntity_AdministratorFail()
        {
            var entity = new UserEntity()
            {
                email = "admin@petshotdemo.com",
                RowKey = "admin",
                PartitionKey = "bo",
                hash = "==",
                roles = "Administrator"
            };

            var userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(m => m.Retrieve(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(entity);
            userRepositoryMock.Setup(m => m.Delete(It.IsAny<UserEntity>())).ReturnsAsync(true);
            var userService = new UserService(userRepositoryMock.Object);

            var entityResult = userService.Delete("bo", "admin").Result;

            entityResult.Should().NotBeNull();
            entityResult.Messages.Should().Contain("User bo/admin can't be deleted");
        }

        [Test]
        public void Test_Delete_UserEntity_NotFound()
        {
            var userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(m => m.Retrieve(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult<UserEntity?>(null));
            var userService = new UserService(userRepositoryMock.Object);
            var entityResult = userService.Delete("bo", "admin").Result;

            entityResult.Should().NotBeNull();
            entityResult.Messages.Should().Contain("User bo/admin not found");
        }

        [Test]
        public void Test_Delete_UserEntity_Failed()
        {
            var entity = new UserEntity()
            {
                email = "admin@petshotdemo.com",
                RowKey = "admin",
                PartitionKey = "bo",
                hash = "#",
                roles = "User"
            };

            var userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(m => m.Retrieve(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult<UserEntity?>(entity));
            var userService = new UserService(userRepositoryMock.Object);
            userRepositoryMock.Setup(m => m.Delete(It.IsAny<UserEntity>())).ReturnsAsync(false);
            var entityResult = userService.Delete("bo", "admin").Result;

            entityResult.Should().NotBeNull();
            entityResult.Messages.Should().Contain("Deleting failed for bo/admin. See logs for more details.");
        }
    }
}
