using PetShop.Model;

namespace PetShop.Data
{
    public interface ISetupRepository
    {
        Task<CallResponse> Setup();
    }
}