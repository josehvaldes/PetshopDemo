using PetShop.Application.Requests;

namespace PetShop.Application.Interfaces
{
    public interface ISetupService
    {
        Task<CallResponse> Setup();
    }
}