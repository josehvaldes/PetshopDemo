using Azure;
using Azure.Data.Tables;
using Azure.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PetShop.Application.Interfaces.Repository;
using PetShop.Application.Interfaces.Repository.Products;
using PetShop.Application.Settings;
using PetShop.Domain.Entities;
using PetShop.Infrastructure.Entities;
using PetShop.Infrastructure.Extensions;

namespace PetShop.Infrastructure.Repository
{
    public class ProductQueryRepository : IProductQuery
    {
        internal static readonly string AzureTableName = "Products";
        private readonly AzureSettings _azureSettings;
        private readonly ILogger<ProductCommandRepository> _logger;

        public ProductQueryRepository(IOptions<AzureSettings> settings, ILogger<ProductCommandRepository> logger)
        {
            _azureSettings = settings.Value;
            _logger = logger;
        }

        public async Task<IEnumerable<Product>> RetrieveList(string domain, string type)
        {
            try
            {
                var list = new List<ProductEntity>();

                var tableClient = new TableClient(
                    new Uri(_azureSettings.StorageURI),
                    AzureTableName,
                    new DefaultAzureCredential());

                AsyncPageable<ProductEntity> queryResultsFilter = tableClient.QueryAsync<ProductEntity>(filter: $"PartitionKey eq '{domain}' and pettype eq '{type}'", maxPerPage: 50);

                await foreach (Page<ProductEntity> page in queryResultsFilter.AsPages())
                {
                    foreach (ProductEntity qEntity in page.Values)
                    {
                        list.Add(qEntity);
                    }
                }

                return list;
            }
            catch (Exception e)
            {
                _logger.LogError($"Products RetrieveList failed: {e.Message}");
                throw new Exception($"Products RetrieveList failed: {e.Message}", e);
            }
        }

        public async Task<IEnumerable<Product>> RetrieveAvailablesList(string domain, string type)
        {
            try
            {
                var list = new List<ProductEntity>();
                var tableClient = new TableClient(
                    new Uri(_azureSettings.StorageURI),
                    AzureTableName,
                    new DefaultAzureCredential());

                AsyncPageable<ProductEntity> queryResultsFilter = tableClient.QueryAsync<ProductEntity>(filter: $"PartitionKey eq '{domain}' and pettype eq '{type}' and stock gt '{0}'", maxPerPage: 50);

                await foreach (Page<ProductEntity> page in queryResultsFilter.AsPages())
                {
                    foreach (ProductEntity qEntity in page.Values)
                    {
                        list.Add(qEntity);
                    }
                }

                return list;
            }
            catch (Exception e)
            {
                _logger.LogError($"Products RetrieveAvailablesList failed: {e.Message}");
                throw new Exception($"Products RetrieveAvailablesList failed: {e.Message}", e);
            }
        }

        public async Task<Product?> Retrieve(string domain, string name)
        {
            try
            {
                var tableClient = new TableClient(
                    new Uri(_azureSettings.StorageURI),
                    AzureTableName,
                    new DefaultAzureCredential());

                var entity = await tableClient.GetEntityIfExistsAsync<ProductEntity>(domain, name);
                if (entity.HasValue)
                {
                    return entity.Value;
                }
                else
                {
                    _logger.LogWarning($"Product {name} on {domain} not found.");
                    return null;
                }

            }
            catch (Exception e)
            {
                _logger.LogError($"Product Retrieve failed: {e.Message}");
                throw new Exception($"Product Retrieve failed: {e.Message}", e);
            }
        }

    }
}
