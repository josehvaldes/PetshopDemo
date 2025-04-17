using Azure.Data.Tables;
using Azure.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PetShop.Application.Interfaces.Repository;
using PetShop.Application.Requests;
using PetShop.Application.Settings;
using PetShop.Domain.Entities;
using PetShop.Infrastructure.Extensions;
using PetShop.Infrastructure.Mockup;


namespace PetShop.Infrastructure.Repository
{
    public class SetupRepository : ISetupRepository
    {
        private readonly AzureSettings _azureSettings;
        private readonly ILogger<SetupRepository> _logger;

        public SetupRepository(IOptions<AzureSettings> settings, ILogger<SetupRepository> logger)
        {
            _azureSettings = settings.Value;
            _logger = logger;
        }


        public async Task<CallResponse> Setup()
        {
            var response = new CallResponse();
            var saleResult = await SetupSales(response);
            var productResult = await SetupProducts(response);
            var userResult = await SetupUser(response);
            var clientResult = await SetupClient(response);

            if (saleResult && productResult && userResult && clientResult)
            {
                response.AddMessage("Process completed");
            }

            return response;
        }

        private async Task<bool> SetupSales(CallResponse response)
        {
            try
            {
                //CREATE Sales
                var tableClient = new TableClient(
                        new Uri(_azureSettings.StorageURI),
                        SaleRepository.AzureTableName,
                        new DefaultAzureCredential()
                    );

                var tableItem = await tableClient.CreateIfNotExistsAsync();
                if (tableItem != null)
                {
                    var item = tableItem.Value;
                    response.AddMessage($"Sales table{(item == null ? " NOT " : " ")}ready. Status: {tableItem.GetRawResponse().Status}");
                }
            }
            catch (Exception e)
            {
                response.AddMessage($"Create 'Sales' Table Failed. {e.Message}");
            }

            return true;
        }


        private async Task<bool> SetupClient(CallResponse response)
        {
            try
            {
                //CREATE ClientTable
                var tableClient = new TableClient(
                        new Uri(_azureSettings.StorageURI),
                        ClientRepository.AzureTableName,
                        new DefaultAzureCredential()
                    );

                var tableItem = await tableClient.CreateIfNotExistsAsync();
                if (tableItem != null)
                {
                    var item = tableItem.Value;
                    response.AddMessage($"Client table{(item == null ? " NOT " : " ")}ready. Status: {tableItem.GetRawResponse().Status}");

                    if (tableItem.GetRawResponse().Status == 204)
                    {
                        foreach (var client in ClientMockup._clientMockups)
                        {
                            var entity = client.ToEntity();
                            var result = await tableClient.AddEntityAsync(entity);
                            response.AddMessage($"Create Client '{client.fullname}' {(result.Status == 204 ? "Success" : "Failed")}. Status:{result.Status}");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                response.AddMessage($"Create 'Client' Table Failed. {e.Message}");
            }
            return true;
        }

        private async Task<bool> SetupProducts(CallResponse response)
        {
            try
            {
                //CREATE Products Table
                var tableClient = new TableClient(
                        new Uri(_azureSettings.StorageURI),
                        ProductRepository.AzureTableName,
                        new DefaultAzureCredential()
                    );

                var tableItem = await tableClient.CreateIfNotExistsAsync();
                if (tableItem != null)
                {
                    var item = tableItem.Value;
                    response.AddMessage($"Product table{(item == null ? " NOT " : " ")}ready. Status: {tableItem.GetRawResponse().Status}");

                    if (tableItem.GetRawResponse().Status == 204)
                    {
                        foreach (var product in ProductMockup._productMockups)
                        {
                            var entity = product.ToEntity();
                            var result = await tableClient.AddEntityAsync(entity);
                            response.AddMessage($"Create Product '{product.name}' {(result.Status == 204 ? "Success" : "Failed")}. Status:{result.Status}");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                response.AddMessage($"Create 'Product' Table Failed. {e.Message}");
            }

            return true;
        }

        private async Task<bool> SetupUser(CallResponse response)
        {
            try
            {
                //CREATE USERS
                var tableClient = new TableClient(
                        new Uri(_azureSettings.StorageURI),
                        UserRepository.AzureTableName,
                        new DefaultAzureCredential()
                    );

                var tableItem = await tableClient.CreateIfNotExistsAsync();
                if (tableItem != null)
                {
                    var item = tableItem.Value;
                    response.AddMessage($"User table{(item == null ? " NOT " : " ")}ready. Status: {tableItem.GetRawResponse().Status}");
                    if (tableItem.GetRawResponse().Status == 204)
                    {
                        foreach (var user in UserMockup._mockupUsers)
                        {
                            var entity = user.ToEntity();
                            var result = await tableClient.AddEntityAsync(entity);
                            response.AddMessage($"Create User '{user.username}' {(result.Status == 204 ? "Success" : "Failed")}. Status: {result.Status}");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                response.AddMessage($"Create 'User' Table Failed. {e.Message}");
            }

            return true;
        }
    }
}
