using MongoDB.Driver;
using PetShop.Application.Interfaces.Repository.Products;
using PetShop.Domain.Entities;

namespace Petshop.Infrastructure.MongoDB
{
    public class ProductQueryable : IProductQueryable
    {
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<Product> _productCollection;

        public ProductQueryable(IMongoClient mongoClient, string databaseName)
        {
            _database = mongoClient.GetDatabase(databaseName);
            _productCollection = _database.GetCollection<Product>("Products");
        }

        public async Task<IQueryable<Product>> GetQueryable()
        {
            return await Task.FromResult(_productCollection.AsQueryable()) ;
        }
    }
}
