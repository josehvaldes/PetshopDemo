using Azure;
using Azure.Data.Tables;
using Azure.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PetShop.Application.Interfaces.Repository;
using PetShop.Application.Settings;
using PetShop.Domain.Entities;
using PetShop.Infrastructure.Entities;
using PetShop.Infrastructure.Extensions;

namespace PetShop.Infrastructure.Repository
{
    /// <summary>
    /// Repository for managing client entities in Azure Table Storage.
    /// </summary>
    public class ClientRepository : IClientRepository
    {
        internal static readonly string AzureTableName = "Clients";
        private readonly AzureSettings _azureSettings;
        private readonly ILogger<ClientRepository> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientRepository"/> class.
        /// </summary>
        /// <param name="settings">Azure settings for storage configuration.</param>
        /// <param name="logger">Logger for logging operations.</param>
        public ClientRepository(IOptions<AzureSettings> settings, ILogger<ClientRepository> logger)
        {
            _azureSettings = settings.Value;
            _logger = logger;
        }

        /// <summary>
        /// Creates a new client entity in Azure Table Storage.
        /// </summary>
        /// <param name="client">The client entity to create.</param>
        /// <returns>The created client entity, or null if the operation fails.</returns>
        /// <exception cref="Exception">Thrown when a duplicate client exists or an unexpected error occurs.</exception>
        public async Task<bool> Create(Client client)
        {
            var entity = client.ToEntity();
            entity.ETag = ETag.All;
            try
            {
                var tableClient = new TableClient(
                new Uri(_azureSettings.StorageURI),
                AzureTableName,
                new DefaultAzureCredential());

                var response = await tableClient.AddEntityAsync(entity);
                if (response.Status == 204)
                {
                    return true;
                }

                _logger.LogWarning($"Create Client Unexpected Response status: [{response.Status}]");
                return false;
            }
            catch (RequestFailedException ex)
            {
                _logger.LogError($"Duplicated Client. Taxnumber: '{entity.PartitionKey}', Fullname: '{entity.RowKey}'. Message: {ex.Message}");
                throw new Exception($"Duplicated Client. Taxnumber: '{entity.taxnumber}', Fullname: '{entity.fullname}'. Status: {ex.Status}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Create client: Domain: {client.taxnumber}, fullname:{client.fullname}. Message {ex.Message}");
                throw new Exception($"Error Create client: Domain: {client.taxnumber}, fullname:{client.fullname}. Message {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Retrieves a client entity from Azure Table Storage.
        /// </summary>
        /// <param name="taxNumberEnd">The partition key of the client entity.</param>
        /// <param name="taxNumber">The row key of the client entity.</param>
        /// <returns>The retrieved client entity, or null if not found.</returns>
        /// <exception cref="Exception">Thrown when an unexpected error occurs during retrieval.</exception>
        public async Task<Client?> Retrieve(string taxNumberEnd, string taxNumber)
        {
            try
            {
                var tableClient = new TableClient(
                    new Uri(_azureSettings.StorageURI),
                    AzureTableName,
                    new DefaultAzureCredential());

                var entity = await tableClient.GetEntityIfExistsAsync<ClientEntity>(taxNumberEnd, taxNumber);
                if (entity.HasValue)
                {
                    return entity.Value;
                }
                else
                {
                    _logger.LogWarning($"Client {taxNumber} not found.");
                    return null;
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Client access failed");
                throw new Exception("Client access failed", e);
            }
        }

        /// <summary>
        /// Updates an existing client entity in Azure Table Storage.
        /// </summary>
        /// <param name="entity">The client entity to update.</param>
        /// <returns>True if the update is successful, otherwise false.</returns>
        /// <exception cref="Exception">Thrown when an unexpected error occurs during the update.</exception>
        public async Task<bool> Update(Client client)
        {
            var entity = client.ToEntity();
            try
            {
                var tableClient = new TableClient(
                    new Uri(_azureSettings.StorageURI),
                    AzureTableName,
                    new DefaultAzureCredential());

                var response = await tableClient.UpdateEntityAsync(entity, entity.ETag);

                if (response.Status == 204)
                {
                    return true;
                }
                _logger.LogWarning($"Update Unexpected Response status: [{response.Status}]");
                return false;
            }
            catch (Exception e)
            {
                _logger.LogError($"Update Client failed: {e.Message}");
                throw new Exception($"Update Client failed: {e.Message}", e);
            }
        }

        /// <summary>
        /// Deletes a client entity from Azure Table Storage.
        /// </summary>
        /// <param name="client">The client entity to delete.</param>
        /// <returns>True if the deletion is successful, otherwise false.</returns>
        /// <exception cref="Exception">Thrown when an unexpected error occurs during the deletion.</exception>
        public async Task<bool> Delete(Client client)
        {
            var entity = client.ToEntity();
            try
            {
                var tableClient = new TableClient(
                    new Uri(_azureSettings.StorageURI),
                    AzureTableName,
                    new DefaultAzureCredential());

                var response = await tableClient.DeleteEntityAsync(entity.PartitionKey, entity.RowKey);

                if (response.Status == 204)
                {
                    return true;
                }
                _logger.LogWarning($"Delete Unexpected Response status: [{response.Status}]");
                return false;
            }
            catch (Exception e)
            {
                _logger.LogError($"Delete Client failed: {e.Message}");
                throw new Exception($"Delete Client failed: {e.Message}", e);
            }
        }
    }
}
