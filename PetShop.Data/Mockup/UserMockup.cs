using Azure;
using PetShop.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetShop.Data.Mockup
{
    public class UserMockup : IUserRepository
    {

        internal static List<UserEntity> _mockupUsers = new List<UserEntity>() {
            new UserEntity() {
                    guid= "4e42250c-ca60-4180-803a-c587e4954352",
                    email="admin@petshotdemo.com",
                    RowKey="admin",
                    PartitionKey="bo",
                    ETag= ETag.All,
                    hash= "AQAAAAIAAYagAAAAECD2PqzkBFb3UWjvpVeB+pEPgA3n4Uq6wP3LDDdJ6fz78xOWXdZoJAaONH1oIvNfMA==",
                    Timestamp = new DateTimeOffset(),
                    roles = "Administrator,User"
                }
        };

        public async Task<UserEntity?> Create(UserEntity user)
        {
            var response = await Task.Run(() =>
            {

                if (!_mockupUsers.Where(x => x.domain == user.domain && x.username == user.username).Any())
                {
                    user.Timestamp = DateTimeOffset.Now;
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

        public async Task<bool> Delete(UserEntity user)
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

        public async Task<UserEntity?> Retrieve(string domain, string username)
        {
            var user = await Task.Run(() =>
            {
                return _mockupUsers.Where(x => x.RowKey == username && x.PartitionKey == domain).FirstOrDefault();
            });

            return user;
        }
    }
}
