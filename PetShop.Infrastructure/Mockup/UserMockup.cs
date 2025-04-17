using Azure;
using PetShop.Application.Interfaces.Repository;
using PetShop.Domain.Entities;

namespace PetShop.Infrastructure.Mockup
{
    public class UserMockup : IUserRepository
    {

        internal static List<User> _mockupUsers = new List<User>() {
            new User() {
                    guid= "4e42250c-ca60-4180-803a-c587e4954352",
                    email="admin@petshotdemo.com",
                    username="admin",
                    domain="bo",
                    //ETag= ETag.All,
                    hash= "AQAAAAIAAYagAAAAECD2PqzkBFb3UWjvpVeB+pEPgA3n4Uq6wP3LDDdJ6fz78xOWXdZoJAaONH1oIvNfMA==",
                    roles = "Administrator,User"
                }
        };

        public async Task<User?> Create(User user)
        {
            var response = await Task.Run(() =>
            {

                if (!_mockupUsers.Where(x => x.domain == user.domain && x.username == user.username).Any())
                {
                    _mockupUsers.Add(user);
                    return user;
                }
                else
                {
                    throw new InvalidDataException($"Duplicated User: Username {user.username}, domain: {user.domain}");
                }
            });

            return response;
        }

        public async Task<bool> Delete(User user)
        {
            return await Task.Run(() =>
            {
                var list = _mockupUsers.Where(x => x.username == user.username);
                if (list.Any())
                {
                    _mockupUsers.Remove(list.First());
                    return true;
                }
                else
                {
                    return false;
                }
            });
        }

        public async Task<User?> Retrieve(string domain, string username)
        {
            var user = await Task.Run(() =>
            {
                return _mockupUsers.Where(x => x.username == username && x.domain == domain).FirstOrDefault();
            });

            return user;
        }
    }
}
