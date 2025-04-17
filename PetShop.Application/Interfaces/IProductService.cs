using PetShop.Application.Requests;
using PetShop.Domain.Entities;

namespace PetShop.Application.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> RetrieveAvailablesList(string domain, string type);
        Task<IEnumerable<Product>> RetrieveAllList(string domain, string type);

        Task<Product?> Retrieve(string domain, string name);

        Task<bool> Delete(string domain, string name);

        Task<bool> Update(ProductRequest request);

        Task<Product?> Create(ProductRequest request);


    }
}
