using Microsoft.EntityFrameworkCore;
using PetShop.Application.Interfaces.Repository.Products;
using PetShop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PetShop.Infrastructure.EF.Sqlite
{
    public class ProductQueryable : IProductQueryable
    {
        private readonly PetShopContext _dbContext = null!;

        public ProductQueryable(PetShopContext petshopContext)
        {
            _dbContext = petshopContext;
        }

        public async Task<IQueryable<Product>> GetQueryable()
        {
            return await Task.FromResult(_dbContext.Products);
        }

        public Product? Retrieve(string domain, string name)
        {
            return _dbContext.Products.Where(x => x.domain == domain && x.name == name).FirstOrDefault();
        }

    }
}
