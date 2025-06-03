using Microsoft.Extensions.Logging;
using PetShop.Application.Interfaces.Repository;
using PetShop.Application.Interfaces.Repository.Products;
using PetShop.Application.Interfaces.Services;
using PetShop.Application.Requests;
using PetShop.Domain.Entities;

namespace PetShop.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductCommand _productCommand;
        private readonly IProductQuery _productQuery;
        private readonly IProductQueryable _productQueryable;

        private readonly ILogger<ProductService> _logger;

        public ProductService(IProductCommand productCommand, IProductQuery productQuery, IProductQueryable productQueryable, ILogger<ProductService> logger) 
        {
            _productCommand = productCommand;
            _productQuery = productQuery;
            _logger = logger;
            _productQueryable = productQueryable;
        }

        public async Task<Product?> Create(ProductRequest request)
        {
            var entity = new Product();
            entity.category = request.Category;
            entity.description = request.Description;
            entity.pettype = request.PetType;
            entity.stock = request.Stock;
            entity.unitaryprice = request.UnitaryPrice;
            entity.name = request.Name;
            entity.domain = request.Domain;
            entity.guid = Guid.NewGuid().ToString();
            return await _productCommand.Create(entity);
        }

        public async Task<bool> Delete(string domain, string name)
        {
            var product = await _productQuery.Retrieve(domain, name);
            if (product != null)
            {
                return await _productCommand.Delete(product);
            }
            else
            {
                _logger.LogWarning($"Delete failed. Product not Found. Domain {domain}, Name {name}");
                return false;
            }
        }
        public async Task<bool> Update(ProductRequest request)
        {
            var entity = await _productQuery.Retrieve(request.Domain, request.Name);
            if (entity != null)
            {
                entity.category = request.Category;
                entity.pettype = request.PetType;
                entity.description = request.Description;
                entity.stock = request.Stock;
                entity.unitaryprice = request.UnitaryPrice;
                return await _productCommand.Update(entity);
            }
            return false;
        }

        public async Task<Product?> Retrieve(string domain, string name)
        {
            return await _productQuery.Retrieve(domain, name);
        }

        public async Task<IEnumerable<Product>> RetrieveAllList(string domain, string type)
        {
            return await _productQuery.RetrieveList(domain, type);
        }

        public async Task<IEnumerable<Product>> RetrieveAvailablesList(string domain, string type)
        {
            return await _productQuery.RetrieveAvailablesList(domain, type);
        }

        public IQueryable<Product> GetQueryableProducts()
        {
            return _productQueryable.GetQueryable();
        }
    }
}
