using Microsoft.Extensions.Logging;
using PetShop.Application.Interfaces;
using PetShop.Application.Requests;
using PetShop.Domain.Entities;

namespace PetShop.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ILogger<ProductService> _logger;

        public ProductService(IProductRepository productRepository, ILogger<ProductService> logger) 
        {
            _productRepository = productRepository;
            _logger = logger;
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
            return await _productRepository.Create(entity);
        }

        public async Task<bool> Delete(string domain, string name)
        {
            var product = await _productRepository.Retrieve(domain, name);
            if (product != null)
            {
                return await _productRepository.Delete(product);
            }
            else
            {
                _logger.LogWarning($"Delete failed. Product not Found. Domain {domain}, Name {name}");
                return false;
            }
        }
        public async Task<bool> Update(ProductRequest request)
        {
            var entity = await _productRepository.Retrieve(request.Domain, request.Name);
            if (entity != null)
            {
                entity.category = request.Category;
                entity.pettype = request.PetType;
                entity.description = request.Description;
                entity.stock = request.Stock;
                entity.unitaryprice = request.UnitaryPrice;
                return await _productRepository.Update(entity);
            }
            return false;
        }

        public async Task<Product?> Retrieve(string domain, string name)
        {
            return await _productRepository.Retrieve(domain, name);
        }

        public async Task<IEnumerable<Product>> RetrieveAllList(string domain, string type)
        {
            return await _productRepository.RetrieveList(domain, type);
        }

        public async Task<IEnumerable<Product>> RetrieveAvailablesList(string domain, string type)
        {
            return await _productRepository.RetrieveAvailablesList(domain, type);
        }        
    }
}
