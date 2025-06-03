using PetShop.Application.Interfaces.Repository.Products;
using PetShop.Domain.Entities;

namespace PetShop.Infrastructure.EF.Sqlite
{
    public class ProductCommand : IProductCommand
    {
        private readonly PetShopContext _dbContext = null!;

        public ProductCommand(PetShopContext petshopContext)
        {
            _dbContext = petshopContext;
        }        

        public async Task<Product?> Create(Product product)
        {
            _dbContext.Products.Add(product);
            await _dbContext.SaveChangesAsync();

            return product;
        }

        public async Task<bool> Delete(Product product)
        {
            var dbProduct = _dbContext.Products.Find(product.guid);

            if (dbProduct!=null) 
            {
                _dbContext.Products.Remove(dbProduct);
                await _dbContext.SaveChangesAsync();

                return true;
            }

            return false;
        }

        public async Task<bool> Update(Product product)
        {
            var dbProduct = _dbContext.Products.Find(product.guid);

            if (dbProduct != null)
            {
                dbProduct.category = product.category;
                dbProduct.description = product.description;
                dbProduct.pettype = product.pettype;
                dbProduct.stock = product.stock;
                dbProduct.unitaryprice = product.unitaryprice;
                dbProduct.name = product.name;
                dbProduct.domain = product.domain;
                _dbContext.Products.Update(dbProduct);

                await _dbContext.SaveChangesAsync();
                return true;
            }

            return false;
        }
    }
}
