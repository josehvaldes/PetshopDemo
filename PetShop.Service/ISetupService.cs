using PetShop.Model;

namespace PetShop.Service
{
    public interface ISetupService
    {
        Task<CallResponse> Setup();
    }
}