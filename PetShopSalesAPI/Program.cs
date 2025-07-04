using Asp.Versioning;
using FluentValidation;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Identity;
using Microsoft.FeatureManagement;
using Microsoft.OpenApi.Models;
using PetShop.Application.Interfaces.Repository;
using PetShop.Application.Interfaces.Repository.Products;
using PetShop.Application.Interfaces.Services;
using PetShop.Application.Services;
using PetShop.Application.Settings;
using PetShop.Infrastructure.Mockup;
using PetShop.Infrastructure.Repository;
using PetShopSalesAPI.Auth;
using PetShopSalesAPI.Configurations;
using PetShopSalesAPI.HealthChecks;
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

var isAzureAppConfigured = builder.AddAzureAppConfiguration();

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

builder.Services.ConfigureHealthChecks(builder.Configuration);



var app = builder.Build();
//HealthCheck Middleware
//app.MapHealthChecks("/api/health");

app.MapHealthChecks("/api/health", new HealthCheckOptions()
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.UseHealthChecksUI(builder =>
{
    builder.UIPath = "/healthchecks-ui";
    //builder.ResourcesPath = "/healthchecks-ui-resources";
    //builder.AddCustomStylesheet("./HealthCheck/Custom.css");
});


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if(isAzureAppConfigured)
    app.UseAzureAppConfiguration();

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();

void AddAzureToScope(IServiceCollection services) 
{
    services.AddScoped<IUserRepository, UserRepository>();
    builder.Services.AddScoped<IProductCommand, ProductCommandRepository>();
    builder.Services.AddScoped<IProductQuery, ProductQueryRepository>();
    services.AddScoped<IClientRepository, ClientRepository>();
    services.AddScoped<ISaleRepository, SaleRepository>();
}

void AddMocksToScope(IServiceCollection services)
{
    services.AddScoped<IUserRepository, UserMockup>();
    builder.Services.AddScoped<IProductCommand, ProductCommandMockup>();
    builder.Services.AddScoped<IProductQuery, ProductQueryMockup>();
    services.AddScoped<IClientRepository, ClientMockup>();
    services.AddScoped<ISaleRepository, SaleMockup>();
}