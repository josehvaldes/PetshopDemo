using Azure.Data.Tables;
using Azure.Identity;
using HealthChecks.Azure.Data.Tables;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace PetShopSalesAPI.HealthChecks
{
    public static class HealthCheck
    {
        public static void ConfigureHealthChecks(this IServiceCollection services, IConfiguration configuration)
        {
            var azureSettings = configuration.GetSection("AzureSettings");
            string azureTableEndpoint = azureSettings.GetValue<string>("StorageURI") ?? throw new InvalidOperationException("The setting 'azureSettings:StorageURI' was not found.");

            services.AddSingleton(sp => new TableServiceClient(new Uri(azureTableEndpoint), new DefaultAzureCredential()));
            services.AddHealthChecks()
                .AddAzureTable(
                optionsFactory: sp =>
                {
                    //var tableServiceClient = sp.GetRequiredService<TableServiceClient>();
                    return new AzureTableServiceHealthCheckOptions()
                    {
                        TableName = "Products",
                    };
                }
                )
                .AddCheck<RemoteHealthCheck>("Remote endpoints Health Check", failureStatus: HealthStatus.Unhealthy)
                .AddCheck<MemoryHealthCheck>($"Feedback Service Memory Check", failureStatus: HealthStatus.Unhealthy, tags: new[] { "Feedback Service" });

            //services.AddHealthChecksUI();
            services.AddHealthChecksUI(opt =>
            {
                opt.SetEvaluationTimeInSeconds(10); //time in seconds between check    
                opt.MaximumHistoryEntriesPerEndpoint(60); //maximum history of checks    
                opt.SetApiMaxActiveRequests(1); //api requests concurrency    
                opt.AddHealthCheckEndpoint("feedback api", "/api/health"); //map health check api    

            })
                .AddInMemoryStorage();
        }
    }
}
