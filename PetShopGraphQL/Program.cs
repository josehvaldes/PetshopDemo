using Microsoft.EntityFrameworkCore;
using PetShop.Application.Interfaces.Repository.Products;
using PetShop.Application.Interfaces.Services;
using PetShop.Application.Services;
using PetShop.Infrastructure.EF.Sqlite;
using PetShop.Infrastructure.Mockup;
using PetShopGraphQL.GraphQL;
using HotChocolate.AspNetCore; // Ensure this namespace is included
using HotChocolate.AspNetCore.Extensions;
using PetShop.Application.Interfaces.Repository; // Ensure this namespace is included

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Configure Entity Framework Core with SQLite
builder.Services.AddDbContext<PetShopContext>();

/// Add Singleton services for testing purposes
builder.Services.AddScoped<IProductCommand, ProductCommandMockup>();
builder.Services.AddScoped<IProductQuery, ProductQueryMockup>();
builder.Services.AddScoped<IProductQueryable, ProductQueryable>();
builder.Services.AddScoped< ISaleQueryable, SaleQueryable> ();
builder.Services.AddScoped<IProductService, ProductService>();


// Add services to the container
builder.Services
    .AddGraphQLServer()
    .RegisterDbContextFactory<PetShopContext>()
    .AddQueryType<Query>()
    .AddType<QProduct>()
    .AddMutationType<Mutation>()
    .AddProjections()
    .AddFiltering()
    .AddSorting();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PetShopContext>();
    db.Database.EnsureCreated();
    ProductDataInitializer.LazyInitializer(db);
}

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
