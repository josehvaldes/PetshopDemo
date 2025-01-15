using PetShop.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetShop.Service
{
    public interface ISaleService
    {
        Task<CallResponse> Create(SalesRequest request);
        Task<IEnumerable<SaleEntity>> RetrieveList(string domain);
    }
}
