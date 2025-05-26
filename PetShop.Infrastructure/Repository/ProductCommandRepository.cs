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
    public class ProductCommandRepository : IProductCommand
    {
        internal static readonly string AzureTableName = "Products";
        private readonly AzureSettings _azureSettings;
        private readonly ILogger<ProductCommandRepository> _logger;

        public ProductCommandRepository(IOptions<AzureSettings> settings, ILogger<ProductCommandRepository> logger)
        {
            _azureSettings = settings.Value;
            _logger = logger;
        }


        public async Task<bool> Update(Product product)
        {
            var entity = product.ToEntity();
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
                _logger.LogError($"Update Product failed: {e.Message}");
                throw new Exception($"Update Product failed: {e.Message}", e);
            }
        }

        public async Task<Product?> Create(Product product)
        {
            var entity = product.ToEntity();
            try
            {
                var tableClient = new TableClient(
                new Uri(_azureSettings.StorageURI),
                AzureTableName,
                new DefaultAzureCredential());

                var response = await tableClient.AddEntityAsync(entity);
                if (response.Status == 204)
                {
                    return product;
                }

                _logger.LogWarning($"Create Product Unexpected Response status: [{response.Status}]");
                return null;
            }
            catch (RequestFailedException ex)
            {
                _logger.LogError($"Duplicated Product. Domain: '{entity.PartitionKey}', name: '{entity.RowKey}'. Message: {ex.Message}");
                throw new Exception($"Duplicated Product. Domain: '{entity.PartitionKey}', name: '{entity.RowKey}'. Status: {ex.Status}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Create Product: Domain: {product.domain}, Name: {product.name}. Message {ex.Message}");
                throw new Exception($"Error Create Product: Domain: {product.domain}, Name: {product.name}. Message {ex.Message}", ex);
            }
        }

        public async Task<bool> Delete(Product product)
        {
            var entity = product.ToEntity();
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
                throw new Exception($"Delete client failed: {e.Message}", e);
            }
        }
    }
}
