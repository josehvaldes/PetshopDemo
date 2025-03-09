using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using PetShop.Data;
using PetShop.Model;
using PetShop.Service;
using PetShopAPI.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetShop.Tests.APITests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureClient(HttpClient client)
        {
            //base.ConfigureClient(client);
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            Program.TestingMode = true;

            var productServiceMock = new Mock<IProductService>();
            var list = new List<ProductEntity>();
            productServiceMock.Setup(m => m.RetrieveAvailablesList( It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(list);

            var response = new CallResponse();
            var setupService = new Mock<ISetupService>();
            setupService.Setup( m=> m.Setup()).Returns(Task.FromResult(response));

            var setupLoginMock = new Mock<ILogger<SetupController>>();

            builder.ConfigureServices(services =>
            {
                // Replace a service with a mock
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(ISetupService));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }
                
                services.AddSingleton<ISetupService>(setupService.Object);

                //services.AddSingleton<ILogger<SetupController>>(setupLoginMock.Object);

            });
        }
    }
}
