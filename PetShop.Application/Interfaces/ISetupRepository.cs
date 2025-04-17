using PetShop.Application.Requests;

namespace PetShop.Application.Interfaces
{
    public interface ISetupRepository
    {
        Task<CallResponse> Setup();
    }
}