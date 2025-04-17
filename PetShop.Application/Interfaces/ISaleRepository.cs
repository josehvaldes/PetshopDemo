using PetShop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetShop.Application.Interfaces
{
    public interface ISaleRepository
    {
        Task<Sale?> Create(Sale entity);
        Task<bool> Delete(Sale entity);

        Task<IEnumerable<Sale>> RetrieveList(string domain);
    }
}
