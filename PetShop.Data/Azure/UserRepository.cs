using Azure;
using Azure.Identity;
using Azure.Data.Tables;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Petshop.Common.Settings;
using PetShop.Model;
using System.Net;
using System.Security;

namespace PetShop.Data.Azure
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

        public async Task<UserEntity?> Create(UserEntity user)
        {
            try
            {
                var tableClient = new TableClient(
                new Uri(_azureSettings.StorageURI),
                    AzureTableName,
                    new DefaultAzureCredential()
                );

                var response = await tableClient.AddEntityAsync(user);
                if (response.Status == 204)
                {
                    return user;
                }
                _logger.LogWarning($"Unexpected Response status: [{response.Status}]");
                return null;
            }
            catch (RequestFailedException ex)
            {
                _logger.LogError($"Duplicated User. Domain: '{user.PartitionKey}', username: '{user.RowKey}'. Message: {ex.Message}");
                throw new Exception($"Duplicated User. Domain: '{user.PartitionKey}', username: '{user.RowKey}'. Status: {ex.Status}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Add User: Domain: {user.domain}, User {user.username}. Message {ex.Message}");
                throw new Exception($"Error Add User: Domain: {user.domain}, User {user.username}. Message {ex.Message}", ex);
            }
        }

        public async Task<bool> Delete(UserEntity user)
        {
            try
            {
                var tableClient = new TableClient(
                    new Uri(_azureSettings.StorageURI),
                    AzureTableName,
                    //new DefaultAzureCredential()
                    new DefaultAzureCredential()
                    );

                var response = await tableClient.DeleteEntityAsync(user.PartitionKey, user.RowKey);

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

        public async Task<UserEntity?> Retrieve(string domain, string username)
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
