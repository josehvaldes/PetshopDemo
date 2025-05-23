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
    public class SaleRepository : ISaleRepository
    {
        internal static readonly string AzureTableName = "Sales";
        private readonly AzureSettings _azureSettings;
        private readonly ILogger<SaleRepository> _logger;

        public SaleRepository(IOptions<AzureSettings> settings, ILogger<SaleRepository> logger)
        {
            _logger = logger;
            _azureSettings = settings.Value;
        }

        public async Task<bool> Create(Sale sale)
        {
            var entity = sale.ToEntity();
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
                _logger.LogWarning($"Unexpected Response status: [{response.Status}]");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Add Sale. PartitionKey: {entity.PartitionKey}, RowKey {entity.RowKey}. Message {ex.Message}");
                throw new Exception($"Error Add Sale. PartitionKey: {entity.PartitionKey}, RowKey {entity.RowKey}. Message {ex.Message}", ex);
            }
        }

        public async Task<bool> Delete(Sale sale)
        {
            var entity = sale.ToEntity();
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
                _logger.LogError($"Delete Sale failed: {e.Message}");
                throw new Exception($"Delete Sale failed: {e.Message}", e);
            }
        }


        public async Task<IEnumerable<Sale>> RetrieveList(string domain)
        {
            try
            {
                var list = new List<SaleEntity>();

                var tableClient = new TableClient(
                    new Uri(_azureSettings.StorageURI),
                    AzureTableName,
                    new DefaultAzureCredential());

                AsyncPageable<SaleEntity> queryResultsFilter = tableClient.QueryAsync<SaleEntity>(filter: $"PartitionKey eq '{domain}'", maxPerPage: 50);

                await foreach (Page<SaleEntity> page in queryResultsFilter.AsPages())
                {
                    foreach (SaleEntity qEntity in page.Values)
                    {
                        list.Add(qEntity);
                    }
                }

                return list;
            }
            catch (Exception e)
            {
                _logger.LogError($"Sales RetrieveList failed: {e.Message}");
                throw new Exception($"Sales RetrieveList failed: {e.Message}", e);
            }
        }
    }
}
