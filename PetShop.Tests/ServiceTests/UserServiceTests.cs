using Microsoft.AspNetCore.Identity;
using NUnit.Framework;
using PetShopAPI.Auth;
using FluentAssertions;
using Moq;
using PetShop.Domain.Entities;
using PetShop.Application.Interfaces;
using PetShop.Application.Services;

namespace PetShop.Tests.ServiceTests
{
    [TestFixture]
    public class UserServiceTests
    {

        [Test]
        public void Test_Generate_Valid_Hash()
        {
            var identityHasher = new PasswordHasher<AuthUser>();
            var passwdHasher = new PasswdHasher(identityHasher);
            var hash = passwdHasher.HashPassword(new AuthUser { Id = "4e42250c-ca60-4180-803a-c587e4954352" }, "852963");
            hash.Should().NotBeNullOrEmpty();
        }

        [Test]
        public void Test_Verify_Hash()
        {
            string hash = "AQAAAAIAAYagAAAAELFW042L6ij5KVd4wf925JTkd0oimQMgkehOyVXgFv2P4iRPPRQun4CTA3VK5FX6hg==";
            string password = "852963";
            var identityHasher = new PasswordHasher<AuthUser>();
            var passwdHasher = new PasswdHasher(identityHasher);
            var roles = new[] { "Administrator", "User" };
            var isValid = passwdHasher.VerifyPassword(new AuthUser { Id = "4e42250c-ca60-4180-803a-c587e4954352", Roles = roles },
                hash, password);
            isValid.Should().Be(true);
        }


        [Test]
        public void Test_Create_User_Valid_GUID() 
        {
            var entity = new User()
            {
                email = "admin@petshotdemo.com",
                username = "admin",
                domain = "bo",
                hash = "==",
            };

            var userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(m => m.Create(It.IsAny<User>())).Returns(Task.FromResult<User?>(entity));

            var userService = new UserService(userRepositoryMock.Object);
            var entityResult = userService.Create(entity).Result;

            entityResult.Should().NotBeNull();
            entityResult.guid.Should().NotBeNullOrEmpty();
        }

        [Test]
        public void Test_Create_User_Role_User()
        {
            var entity = new User()
            {
                email = "admin@petshotdemo.com",
                username = "admin",
                domain = "bo",
                hash = "==",
            };

            var userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(m => m.Create(It.IsAny<User>())).ReturnsAsync(entity);

            var userService = new UserService(userRepositoryMock.Object);
            var entityResult = userService.Create(entity).Result;

            entityResult.Should().NotBeNull();
            entityResult.roles.Should().Be("User");
        }

        [Test]
        public void Test_Delete_User_NonAdministrator()
        {
            var entity = new User()
            {
                email = "usertest@petshotdemo.com",
                username = "usertest",
                domain = "bo",
                hash = "==",
                roles = "User"
            };

            var userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(m => m.Retrieve(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(entity);
            userRepositoryMock.Setup(m => m.Delete(It.IsAny<User>())).ReturnsAsync(true);
            var userService = new UserService(userRepositoryMock.Object);

            var entityResult = userService.Delete("bo", "usertest").Result;

            entityResult.Should().NotBeNull();
            entityResult.Messages.Should().BeEmpty();            
        }

        [Test]
        public void Test_Delete_User_AdministratorFail()
        {
            var entity = new User()
            {
                email = "admin@petshotdemo.com",
                username = "admin",
                domain = "bo",
                hash = "==",
                roles = "Administrator"
            };

            var userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(m => m.Retrieve(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(entity);
            userRepositoryMock.Setup(m => m.Delete(It.IsAny<User>())).ReturnsAsync(true);
            var userService = new UserService(userRepositoryMock.Object);

            var entityResult = userService.Delete("bo", "admin").Result;

            entityResult.Should().NotBeNull();
            entityResult.Messages.Should().Contain("User bo/admin can't be deleted");
        }

        [Test]
        public void Test_Delete_User_NotFound()
        {
            var userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(m => m.Retrieve(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult<User?>(null));
            var userService = new UserService(userRepositoryMock.Object);
            var entityResult = userService.Delete("bo", "admin").Result;

            entityResult.Should().NotBeNull();
            entityResult.Messages.Should().Contain("User bo/admin not found");
        }

        [Test]
        public void Test_Delete_User_Failed()
        {
            var entity = new User()
            {
                email = "admin@petshotdemo.com",
                username = "admin",
                domain = "bo",
                hash = "#",
                roles = "User"
            };

            var userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(m => m.Retrieve(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult<User?>(entity));
            var userService = new UserService(userRepositoryMock.Object);
            userRepositoryMock.Setup(m => m.Delete(It.IsAny<User>())).ReturnsAsync(false);
            var entityResult = userService.Delete("bo", "admin").Result;

            entityResult.Should().NotBeNull();
            entityResult.Messages.Should().Contain("Deleting failed for bo/admin. See logs for more details.");
        }
    }
}
