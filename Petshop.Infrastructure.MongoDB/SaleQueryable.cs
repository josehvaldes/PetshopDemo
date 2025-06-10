using MongoDB.Driver;
using PetShop.Application.Interfaces.Repository;
using PetShop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Petshop.Infrastructure.MongoDB
{
    public class SaleQueryable : ISaleQueryable
    {

        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<Sale> _saleCollection;

        public SaleQueryable(IMongoClient mongoClient, string databaseName)
        {
            _database = mongoClient.GetDatabase(databaseName);
            _saleCollection = _database.GetCollection<Sale>("Sales");
        }

        public async Task<IQueryable<Sale>> GetQueryable()
        {
            return await Task.FromResult(_saleCollection.AsQueryable());
        }
    }
}
