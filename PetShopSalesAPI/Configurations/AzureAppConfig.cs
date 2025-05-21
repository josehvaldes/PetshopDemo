using Azure.Identity;

namespace PetShopSalesAPI.Configurations
{
    public static class AzureAppConfig
    {

        public static bool AddAzureAppConfiguration(this WebApplicationBuilder builder)
        {
            var azureSettings = builder.Configuration.GetSection("AzureSettings");
            string endpoint = azureSettings.GetValue<string>("AppConfiguration") ?? string.Empty;

            if (string.IsNullOrEmpty(endpoint)) 
            {
                builder.Configuration.AddAzureAppConfiguration(options =>
                {
                    options.Connect(new Uri(endpoint), new DefaultAzureCredential())
                            // Load all keys that start with `TestApp:` and have no label

                            // Configure to reload configuration if the registered sentinel key is modified
                            .ConfigureRefresh(refreshOptions =>
                                refreshOptions.Register("mlpetshopapp",
                                refreshAll: true));
                    // Load all feature flags with no label
                    options.UseFeatureFlags(
                        featureFlagOptions =>
                        {
                            //update the refresh to 20 seconds
                            featureFlagOptions.SetRefreshInterval(TimeSpan.FromSeconds(20));
                        });
                });

                builder.Services.AddAzureAppConfiguration();

                return true;
            }

            return false;
        }

    }
}
