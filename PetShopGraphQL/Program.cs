using Microsoft.EntityFrameworkCore;
using PetShop.Application.Interfaces.Repository.Products;
using PetShop.Application.Interfaces.Services;
using PetShop.Application.Services;
using PetShop.Infrastructure.Mockup;
using PetShopGraphQL.GraphQL;
using HotChocolate.AspNetCore; // Ensure this namespace is included
using HotChocolate.AspNetCore.Extensions;
using PetShop.Application.Interfaces.Repository;
using PetShopGraphQL;
using MongoDB.Driver;
using Microsoft.Extensions.Options;
using Petshop.Infrastructure.MongoDB; // Ensure this namespace is included

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();


builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDB"));

var mongodbSettings = builder.Configuration.GetSection("MongoDB");


// Register MongoDB client as a singleton
builder.Services.AddSingleton<IMongoClient>(serviceProvider =>
{
    var settings = serviceProvider.GetRequiredService<IOptions<MongoDbSettings>>().Value;
    return new MongoClient(settings.ConnectionString);
});

// Configure Entity Framework Core with SQLite
//EF code commented out
//builder.Services.AddDbContext<PetShopContext>();

/// Add Singleton services for testing purposes
builder.Services.AddScoped<IProductCommand, ProductCommandMockup>();
builder.Services.AddScoped<IProductQuery, ProductQueryMockup>();
//builder.Services.AddScoped<IProductQueryable, ProductQueryable>();
//builder.Services.AddScoped< ISaleQueryable, SaleQueryable> ();
builder.Services.AddScoped<IProductService, ProductService>();

// Register your MongoDB service (from previous example)
builder.Services.AddScoped<ISaleQueryable>( sp=> new SaleQueryable( sp.GetRequiredService<IMongoClient>(), 
    mongodbSettings.GetValue<string>("DatabaseName") ?? string.Empty));

builder.Services.AddScoped<IProductQueryable>(sp => new ProductQueryable(sp.GetRequiredService<IMongoClient>(), 
    mongodbSettings.GetValue<string>("DatabaseName") ?? string.Empty));

//EF code commented out
// Add services to the container
builder.Services
    .AddGraphQLServer()
    //.RegisterDbContextFactory<PetShopContext>()
    .AddQueryType<Query>()
    .AddType<QProduct>()
    .AddMutationType<Mutation>()
    .AddProjections()
    .AddFiltering()
    .AddSorting();


var app = builder.Build();

//EF code commented out
//using (var scope = app.Services.CreateScope())
//{
//    var db = scope.ServiceProvider.GetRequiredService<PetShopContext>();
//    db.Database.EnsureCreated();
//    ProductDataInitializer.LazyInitializer(db);
//}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGraphQL(); // Maps to /graphql by default

app.UseAuthorization();

app.MapControllers();

app.Run();
