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
    public class ProductRepository : IProductRepository
    {
        internal static readonly string AzureTableName = "Products";
        private readonly AzureSettings _azureSettings;
        private readonly ILogger<ProductRepository> _logger;

        public ProductRepository(IOptions<AzureSettings> settings, ILogger<ProductRepository> logger)
        {
            _azureSettings = settings.Value;
            _logger = logger;
        }

        public async Task<IEnumerable<ProductEntity>> RetrieveList(string domain, string type)
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

        public async Task<IEnumerable<ProductEntity>> RetrieveAvailablesList(string domain, string type)
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

        public async Task<ProductEntity?> Retrieve(string domain, string name)
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

        public async Task<bool> Update(ProductEntity product)
        {
            try
            {
                var tableClient = new TableClient(
                    new Uri(_azureSettings.StorageURI),
                    AzureTableName,
                    new DefaultAzureCredential());

                var response = await tableClient.UpdateEntityAsync(product, product.ETag);

                if (response.Status == 204)
                {
                    return true;
                }
                _logger.LogWarning($"Update Unexpected Response status: [{response.Status}]");
                return false;
            }
            catch (Exception e)
            {
                _logger.LogError($"Update Product failed: {e.Message}");
                throw new Exception($"Update Product failed: {e.Message}", e);
            }
        }

        public async Task<ProductEntity?> Create(ProductEntity product)
        {
            try
            {
                var tableClient = new TableClient(
                new Uri(_azureSettings.StorageURI),
                AzureTableName,
                new DefaultAzureCredential());

                var response = await tableClient.AddEntityAsync(product);
                if (response.Status == 204)
                {
                    return product;
                }

                _logger.LogWarning($"Create Product Unexpected Response status: [{response.Status}]");
                return null;
            }
            catch (RequestFailedException ex)
            {
                _logger.LogError($"Duplicated Product. Domain: '{product.PartitionKey}', name: '{product.RowKey}'. Message: {ex.Message}");
                throw new Exception($"Duplicated Product. Domain: '{product.PartitionKey}', name: '{product.RowKey}'. Status: {ex.Status}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Create Product: Domain: {product.domain}, Name: {product.name}. Message {ex.Message}");
                throw new Exception($"Error Create Product: Domain: {product.domain}, Name: {product.name}. Message {ex.Message}", ex);
            }
        }

        public async Task<bool> Delete(ProductEntity product)
        {
            try
            {
                var tableClient = new TableClient(
                    new Uri(_azureSettings.StorageURI),
                    AzureTableName,
                    new DefaultAzureCredential());

                var response = await tableClient.DeleteEntityAsync(product.PartitionKey, product.RowKey);

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
                throw new Exception($"Delete client failed: {e.Message}", e);
            }
        }
    }
}
