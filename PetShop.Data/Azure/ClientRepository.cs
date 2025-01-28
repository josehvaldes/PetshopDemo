using Azure;
using Azure.Data.Tables;
using Azure.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Petshop.Common.Settings;
using PetShop.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetShop.Data.Azure
{
    public class ClientRepository : IClientRepository
    {
        internal static readonly string AzureTableName = "Clients";
        private readonly AzureSettings _azureSettings;
        private readonly ILogger<ClientRepository> _logger;

        public ClientRepository(IOptions<AzureSettings> settings, ILogger<ClientRepository> logger)
        {
            _azureSettings = settings.Value;
            _logger = logger;
        }

        public async Task<ClientEntity?> Create(ClientEntity client)
        {
            try
            {
                var tableClient = new TableClient(
                new Uri(_azureSettings.StorageURI),
                AzureTableName,
                new DefaultAzureCredential());

                var response = await tableClient.AddEntityAsync(client);
                if (response.Status == 204)
                {
                    return client;
                }

                _logger.LogWarning($"Create Client Unexpected Response status: [{response.Status}]");
                return null;
            }
            catch (RequestFailedException ex)
            {
                _logger.LogError($"Duplicated Client. Taxnumber: '{client.PartitionKey}', Fullname: '{client.RowKey}'. Message: {ex.Message}");
                throw new Exception($"Duplicated Client. Taxnumber: '{client.taxnumber}', Fullname: '{client.fullname}'. Status: {ex.Status}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Create client: Domain: {client.taxnumber}, fullname:{client.fullname}. Message {ex.Message}");
                throw new Exception($"Error Create client: Domain: {client.taxnumber}, fullname:{client.fullname}. Message {ex.Message}", ex);
            }

        }
        public async Task<ClientEntity?> Retrieve(string taxNumberEnd, string taxNumber)
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
        public async Task<bool> Update(ClientEntity entity)
        {
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
        public async Task<bool> Delete(ClientEntity client)
        {
            try
            {
                var tableClient = new TableClient(
                    new Uri(_azureSettings.StorageURI),
                    AzureTableName,
                    new DefaultAzureCredential());

                var response = await tableClient.DeleteEntityAsync(client.PartitionKey, client.RowKey);

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
