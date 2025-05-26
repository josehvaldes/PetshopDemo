using PetShop.Application.Requests;
using PetShop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetShop.Application.Interfaces.Repository.Products
{
    public interface IProductCommand
    {
        Task<Product?> Create(Product product);
        Task<bool> Update(Product product);
        Task<bool> Delete(Product product);
    }
}
