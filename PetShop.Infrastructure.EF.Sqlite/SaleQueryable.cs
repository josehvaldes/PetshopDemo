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

        public IQueryable<Sale> GetQueryable()
        {
            return _dbContext.Sales;
        }
    }
}
