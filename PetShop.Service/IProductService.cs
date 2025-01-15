using PetShop.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetShop.Service
{
    public interface IProductService
    {
        Task<IEnumerable<ProductEntity>> RetrieveAvailablesList(string domain, string type);
        Task<IEnumerable<ProductEntity>> RetrieveAllList(string domain, string type);

        Task<ProductEntity?> Retrieve(string domain, string name);

        Task<bool> Delete(string domain, string name);

        Task<bool> Update(ProductRequest request);

        Task<ProductEntity?> Create(ProductRequest request);


    }
}
