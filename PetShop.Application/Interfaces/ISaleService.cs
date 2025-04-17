using PetShop.Application.Requests;
using PetShop.Domain.Entities;

namespace PetShop.Application.Interfaces
{
    public interface ISaleService
    {
        Task<CallResponse> Create(SalesRequest request);
        Task<IEnumerable<Sale>> RetrieveList(string domain);
    }
}
