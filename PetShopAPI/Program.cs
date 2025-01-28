using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Petshop.Common.Settings;
using PetShop.Data;
using PetShop.Data.Azure;
using PetShop.Data.Mockup;
using PetShop.Service;
using PetShopAPI.Auth;
using PetShopAPI.Middlewares;
using PetShopAPI.Models;
using PetShopAPI.Validators;
using Serilog;
using System;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<AzureSettings>(builder.Configuration.GetSection("AzureSettings"));


var apisetting = builder.Configuration.GetSection("ApiSettings");
builder.Services.Configure<ApiSettings>(apisetting);

var mode = apisetting.GetValue<string>("mode");

if (mode != null && mode == "azure")
{
    builder.Services.AddScoped<IUserRepository, UserRepository>();
    builder.Services.AddScoped<IProductRepository, ProductRepository>();
    builder.Services.AddScoped<IClientRepository, ClientRepository>();
    builder.Services.AddScoped<ISaleRepository, SaleRepository>();
}
else 
{
    builder.Services.AddScoped<IUserRepository, UserMockup>();
    builder.Services.AddScoped<IProductRepository, ProductMockup>();
    builder.Services.AddScoped<IClientRepository, ClientMockup>();
    builder.Services.AddScoped<ISaleRepository, SaleMockup>();
}

builder.Services.AddTransient<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<IPasswdHasher, PasswdHasher>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthUser, AuthUser>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<ISaleService, SaleService>();

builder.Services.AddScoped<ISetupService, SetupService>();
builder.Services.AddScoped<ISetupRepository, SetupRepository>();

builder.Services.AddValidatorsFromAssemblyContaining<AuthenticationRequestValidator>();

//Add support to logging with SERILOG
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseMiddleware<JwtMiddleware>();

app.MapControllers();

app.Run();
