using Asp.Versioning;
using Microsoft.OpenApi.Models;
using Okta.AspNetCore;
using Serilog;
using Azure.Identity;
using Microsoft.Extensions.ML;
using PetShopML.Model;
using PetShopML.ModelAccess;
using PetShop.Application.Settings;

var builder = WebApplication.CreateBuilder(args);


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

var azureSettings = builder.Configuration.GetSection("AzureSettings");

string appConfigEndpoint = azureSettings.GetValue<string>("AppConfiguration")?? throw new InvalidOperationException("The setting 'azureSettings:AppConfiguration' was not found.");
string regressionBlobUri = azureSettings.GetValue<string>("RegressionBlobStorageURI") ?? throw new InvalidOperationException("The setting 'azureSettings:RegressionBlobStorageURI' was not found.");
string product988TimeSeriesUri = azureSettings.GetValue<string>("Product988TimeSeriesURI") ?? throw new InvalidOperationException("The setting 'azureSettings:RegressionBlobStorageURI' was not found.");

builder.Services.Configure<AzureSettings>(azureSettings);

//builder.Configuration.AddAzureAppConfiguration(options =>
//{
//    options.Connect(new Uri(appConfigEndpoint), new DefaultAzureCredential());
//});

var oktasetting = builder.Configuration.GetSection("mlpetshopapp:oktasettings");
var oktaDomain = oktasetting.GetValue<string>("urldomain") ?? throw new InvalidOperationException("The setting 'oktaSetting:urlDomain' was not found.");

//Add Okta authentication
builder.Services.AddAuthentication(
    options =>
    {
        options.DefaultAuthenticateScheme = OktaDefaults.ApiAuthenticationScheme;
        options.DefaultChallengeScheme = OktaDefaults.ApiAuthenticationScheme;
        options.DefaultSignInScheme = OktaDefaults.ApiAuthenticationScheme;
    }
).AddOktaWebApi(new OktaWebApiOptions()
{
    OktaDomain = oktaDomain
});

var stream = BlobStorageAccess.GetRegressionModel(regressionBlobUri);

builder.Services.AddPredictionEnginePool<SaleData, SalesPrediction>()
    .FromUri(modelName: "PershopSalesModel_V1", uri: regressionBlobUri);

builder.Services.AddPredictionEnginePool<SaleData, ProductUnitTimeSeriesPrediction>()
    .FromUri(modelName: "Product988TimeSeries", uri: product988TimeSeriesUri);


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ML API - V1", Version = "v1.0" });
    c.SwaggerDoc("v2", new OpenApiInfo { Title = "ML API - V2", Version = "v2.0" });
});

//Add support to logging with SERILOG
builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration);
    //configuration.WriteTo.ApplicationInsights(
    //        services.GetRequiredService<TelemetryConfiguration>(), TelemetryConverter.Traces
    //        );
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
