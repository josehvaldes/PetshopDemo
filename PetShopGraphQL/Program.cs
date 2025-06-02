using PetShop.Application.Interfaces.Repository.Products;
using PetShop.Application.Interfaces.Services;
using PetShop.Application.Services;
using PetShop.Infrastructure.Mockup;
using PetShopGraphQL.GraphQL;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

/// Add Singleton serrvices for testing purposes
builder.Services.AddSingleton<IProductCommand, ProductCommandMockup>();
builder.Services.AddSingleton<IProductQuery, ProductQueryMockup>();
builder.Services.AddSingleton<IProductService,ProductService>();

// Add services to the container
builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>()
    .AddFiltering()
    .AddSorting();

var app = builder.Build();

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
