using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace PetShopSalesAPI.HealthChecks
{
    public class RemoteHealthCheck : IHealthCheck
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public RemoteHealthCheck(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var httpClient = _httpClientFactory.CreateClient())
            {
                var response = await httpClient.GetAsync("https://localhost:7025/api/health");
                if (response.IsSuccessStatusCode)
                {
                    return HealthCheckResult.Healthy($"Remote endpoints is healthy.");
                }

                return HealthCheckResult.Unhealthy("Remote endpoint is unhealthy");
            }
        }
    }
}
