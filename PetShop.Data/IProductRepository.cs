using PetShop.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetShop.Data
{
    public interface IProductRepository
    {
        Task<IEnumerable<ProductEntity>> RetrieveList(string domain, string type);
        Task<IEnumerable<ProductEntity>> RetrieveAvailablesList(string domain, string type);
        Task<ProductEntity?> Retrieve(string domain, string name);
        Task<ProductEntity?> Create(ProductEntity product);
        Task<bool> Update(ProductEntity product);
        Task<bool> Delete(ProductEntity product);
    }
}
