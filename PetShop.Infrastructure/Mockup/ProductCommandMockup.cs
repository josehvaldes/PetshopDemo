using PetShop.Application.Interfaces.Repository;
using PetShop.Application.Interfaces.Repository.Products;
using PetShop.Domain.Entities;

namespace PetShop.Infrastructure.Mockup
{
    public class ProductCommandMockup : IProductCommand
    {
        public static List<Product> _productMockups = ProductQueryMockup._productMockups;

        public async Task<bool> Update(Product product)
        {
            return await Task.Run(() =>
            {
                var list = _productMockups.Where(x => x.name == product.name && x.guid == product.guid);
                if (list.Any())
                {
                    var item = list.First();
                    item.stock = product.stock;
                    item.name = product.name;
                    item.unitaryprice = product.unitaryprice;
                    return true;
                }
                else
                {
                    return false;
                }
            });
        }

        public async Task<Product?> Create(Product product)
        {
            return await Task.Run(() =>
            {
                if (!_productMockups.Where(x => x.name == product.name && x.domain == product.domain).Any())
                {
                    _productMockups.Add(product);
                    return product;
                }
                else
                {
                    throw new Exception($"Duplicated client: {product.name}. Domain: {product.domain} ");
                }
            });
        }

        public async Task<bool> Delete(Product product)
        {
            return await Task.Run(() =>
            {
                var list = _productMockups.Where(x => x.name == product.name && x.domain == product.domain);
                if (list.Any())
                {
                    var item = list.First();
                    return true;
                }
                else
                {
                    return false;
                }
            });
        }
    }
}
