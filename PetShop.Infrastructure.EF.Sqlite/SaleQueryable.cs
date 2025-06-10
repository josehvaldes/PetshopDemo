using PetShop.Application.Interfaces.Repository;
using PetShop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetShop.Infrastructure.EF.Sqlite
{
    public class SaleQueryable : ISaleQueryable
    {
        private readonly PetShopContext _dbContext = null!;
        public SaleQueryable(PetShopContext petshopContext)
        {
            _dbContext = petshopContext;
        }

        public async Task<IQueryable<Sale>> GetQueryable()
        {
            return await Task.FromResult(_dbContext.Sales);
        }
    }
}
