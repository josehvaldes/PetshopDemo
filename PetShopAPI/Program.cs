using Asp.Versioning;
using FluentValidation;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using PetShop.Application.Interfaces.Repository;
using PetShop.Application.Interfaces.Repository.Products;
using PetShop.Application.Interfaces.Services;
using PetShop.Application.Services;
using PetShop.Application.Settings;
using PetShop.Infrastructure.Mockup;
using PetShop.Infrastructure.Repository;
using PetShopAPI.Auth;
using PetShopAPI.Middlewares;
using PetShopAPI.Models;
using PetShopAPI.Validators;
using Serilog;
using System;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationInsightsTelemetry();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



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
builder.Services.Configure<AzureSettings>(azureSettings);


var apisetting = builder.Configuration.GetSection("ApiSettings");
builder.Services.Configure<ApiSettings>(apisetting);

var mode = apisetting.GetValue<string>("mode");

if (mode != null && mode == "azure")
{
    builder.Services.AddScoped<IUserRepository, UserRepository>();
    builder.Services.AddScoped<IProductCommand, ProductCommandRepository>();
    builder.Services.AddScoped<IProductQuery, ProductQueryRepository>();
    builder.Services.AddScoped<IClientRepository, ClientRepository>();
    builder.Services.AddScoped<ISaleRepository, SaleRepository>();
}
else 
{
    builder.Services.AddScoped<IUserRepository, UserMockup>();
    builder.Services.AddScoped<IProductCommand, ProductCommandMockup>();
    builder.Services.AddScoped<IProductQuery, ProductQueryMockup>();
    builder.Services.AddScoped<IClientRepository, ClientMockup>();
    builder.Services.AddScoped<ISaleRepository, SaleMockup>();
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

builder.Services.AddValidatorsFromAssemblyContaining<AuthenticationRequestValidator>();

var log = new LoggerConfiguration().CreateLogger();

//Add support to logging with SERILOG
builder.Host.UseSerilog((context, services, configuration) => 
{
    configuration.ReadFrom.Configuration(context.Configuration);
    configuration.WriteTo.ApplicationInsights(
            services.GetRequiredService<TelemetryConfiguration>(), TelemetryConverter.Traces
            );
});


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API - V1", Version = "v1.0" });
    c.SwaggerDoc("v2", new OpenApiInfo { Title = "My API - V2", Version = "v2.0" });
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
app.UseAuthorization();

if (TestingMode)
{
    app.UseMiddleware<TestingMiddleware>();
}
else 
{
    app.UseMiddleware<JwtMiddleware>();
}

app.MapControllers();

app.Run();


public partial class Program 
{
    public static bool TestingMode = false;
}