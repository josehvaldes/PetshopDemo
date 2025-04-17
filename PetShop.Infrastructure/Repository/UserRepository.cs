using Azure;
using Azure.Identity;
using Azure.Data.Tables;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PetShop.Application.Settings;
using PetShop.Domain.Entities;
using PetShop.Infrastructure.Extensions;
using PetShop.Infrastructure.Entities;
using PetShop.Application.Interfaces.Repository;

namespace PetShop.Infrastructure.Repository
{
    public class UserRepository : IUserRepository
    {
        internal static readonly string AzureTableName = "Users";
        private readonly AzureSettings _azureSettings;
        private readonly ILogger<UserRepository> _logger;
        public UserRepository(IOptions<AzureSettings> settings, ILogger<UserRepository> logger)
        {
            _azureSettings = settings.Value;
            _logger = logger;
        }

        public async Task<User?> Create(User user)
        {
            var entity = user.ToEntity();
            entity.ETag = ETag.All;
            try
            {
                var tableClient = new TableClient(
                new Uri(_azureSettings.StorageURI),
                    AzureTableName,
                    new DefaultAzureCredential()
                );

                var response = await tableClient.AddEntityAsync(entity);
                if (response.Status == 204)
                {
                    return user;
                }
                _logger.LogWarning($"Unexpected Response status: [{response.Status}]");
                return null;
            }
            catch (RequestFailedException ex)
            {
                _logger.LogError($"Duplicated User. Domain: '{entity.PartitionKey}', username: '{entity.RowKey}'. Message: {ex.Message}");
                throw new Exception($"Duplicated User. Domain: '{entity.PartitionKey}', username: '{entity.RowKey}'. Status: {ex.Status}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Add User: Domain: {user.domain}, User {user.username}. Message {ex.Message}");
                throw new Exception($"Error Add User: Domain: {user.domain}, User {user.username}. Message {ex.Message}", ex);
            }
        }

        public async Task<bool> Delete(User user)
        {
            var entity = user.ToEntity();
            try
            {
                var tableClient = new TableClient(
                    new Uri(_azureSettings.StorageURI),
                    AzureTableName,
                    //new DefaultAzureCredential()
                    new DefaultAzureCredential()
                    );

                var response = await tableClient.DeleteEntityAsync(entity.PartitionKey, entity.RowKey);

                if (response.Status == 204)
                {
                    return true;
                }
                _logger.LogWarning($"Delete User Unexpected Response status: [{response.Status}]");
                return false;
            }
            catch (Exception e)
            {
                _logger.LogError($"Delete User failed: {e.Message}");
                throw new Exception($"Delete User failed: {e.Message}", e);
            }
        }

        public async Task<User?> Retrieve(string domain, string username)
        {
            try
            {
                var tableClient = new TableClient(
                    new Uri(_azureSettings.StorageURI),
                    AzureTableName,
                    //new DefaultAzureCredential()
                    new DefaultAzureCredential()
                    );

                var entity = await tableClient.GetEntityIfExistsAsync<UserEntity>(domain, username);
                if (entity.HasValue)
                {
                    return entity.Value;
                }
                else
                {
                    _logger.LogWarning($"User not found: {username}. domain:{domain}");
                    return null;
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Azure access failed {e.Message}");
                throw new Exception($"Azure User access failed {e.Message}", e);
            }

        }
    }
}
