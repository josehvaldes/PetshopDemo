using PetShop.Application.Requests;

namespace PetShop.Application.Interfaces.Repository
{
    public interface ISetupRepository
    {
        Task<CallResponse> Setup();
    }
}