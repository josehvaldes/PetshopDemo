using PetShop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetShop.Application.Interfaces.Repository.Products
{
    public interface IProductQuery
    {
        Task<IEnumerable<Product>> RetrieveList(string domain, string type);
        Task<IEnumerable<Product>> RetrieveAvailablesList(string domain, string type);
        Task<Product?> Retrieve(string domain, string name);
    }
}
