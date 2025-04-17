using Asp.Versioning;
using Azure.Identity;
using FluentValidation;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.Options;
using Microsoft.FeatureManagement;
using Microsoft.OpenApi.Models;
using PetShop.Application.Interfaces.Repository;
using PetShop.Application.Interfaces.Services;
using PetShop.Application.Services;
using PetShop.Application.Settings;
using PetShop.Infrastructure.Mockup;
using PetShop.Infrastructure.Repository;
using PetShopSalesAPI.Auth;
using PetShopSalesAPI.Validators;
using Serilog;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationInsightsTelemetry();

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(),
        new HeaderApiVersionReader("X-Api-Version")
    );
}).AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API - V1", Version = "v1.0" });
    c.SwaggerDoc("v2", new OpenApiInfo { Title = "My API - V2", Version = "v2.0" });
});


var apisetting = builder.Configuration.GetSection("ApiSettings");
var azureSettings = builder.Configuration.GetSection("azureSettings");

builder.Services.Configure<ApiSettings>(apisetting);
builder.Services.Configure<AzureSettings>(azureSettings);

string endpoint = azureSettings.GetValue<string>("AppConfiguration") ?? throw new InvalidOperationException("The setting 'azureSettings:AppConfiguration' was not found.");

builder.Configuration.AddAzureAppConfiguration(options =>
{
    options.Connect(new Uri(endpoint), new DefaultAzureCredential())
            // Load all keys that start with `TestApp:` and have no label
            //.Select("TestApp:*", LabelFilter.Null)

            // Configure to reload configuration if the registered sentinel key is modified
            .ConfigureRefresh(refreshOptions =>
                refreshOptions.Register("mlpetshopapp", 
                refreshAll: true));
    // Load all feature flags with no label
    options.UseFeatureFlags( 
        featureFlagOptions => {
            //update the refresh to 20 seconds
            featureFlagOptions.SetRefreshInterval(TimeSpan.FromSeconds(20));
        });
});

builder.Services.AddAzureAppConfiguration();
builder.Services.AddFeatureManagement();

var featureManager = new FeatureManager( new ConfigurationFeatureDefinitionProvider(builder.Configuration));

if (await featureManager.IsEnabledAsync("azureconnection"))
{
    AddAzureToScope(builder.Services);
}
else
{
    AddMocksToScope(builder.Services);
}

builder.Services.AddTransient<IPasswordHasher<AuthUser>, PasswordHasher<AuthUser>>();
builder.Services.AddScoped<IPasswdHasher, PasswdHasher>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserAuthentication, UserAuthentication>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<ISaleService, SaleService>();

builder.Services.AddScoped<ISetupService, SetupService>();
builder.Services.AddScoped<ISetupRepository, SetupRepository>();

builder.Services.AddValidatorsFromAssemblyContaining<SalesRequestValidator>();

//Add support to logging with SERILOG
builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration);
    configuration.WriteTo.ApplicationInsights(
            services.GetRequiredService<Microsoft.ApplicationInsights.Extensibility.TelemetryConfiguration>(), TelemetryConverter.Traces
            );
});

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAzureAppConfiguration();
app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();

void AddAzureToScope(IServiceCollection services) 
{
    services.AddScoped<IUserRepository, UserRepository>();
    services.AddScoped<IProductRepository, ProductRepository>();
    services.AddScoped<IClientRepository, ClientRepository>();
    services.AddScoped<ISaleRepository, SaleRepository>();
}

void AddMocksToScope(IServiceCollection services)
{
    services.AddScoped<IUserRepository, UserMockup>();
    services.AddScoped<IProductRepository, ProductMockup>();
    services.AddScoped<IClientRepository, ClientMockup>();
    services.AddScoped<ISaleRepository, SaleMockup>();
}