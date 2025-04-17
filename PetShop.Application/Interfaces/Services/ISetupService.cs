using PetShop.Application.Requests;

namespace PetShop.Application.Interfaces.Services
{
    public interface ISetupService
    {
        Task<CallResponse> Setup();
    }
}