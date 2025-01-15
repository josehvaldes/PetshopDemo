using PetShop.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetShop.Data
{
    public interface ISaleRepository
    {
        Task<SaleEntity?> Create(SaleEntity entity);
        Task<bool> Delete(SaleEntity entity);

        Task<IEnumerable<SaleEntity>> RetrieveList(string domain);
    }
}
