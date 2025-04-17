using PetShop.Application.Interfaces;
using PetShop.Application.Requests;

namespace PetShop.Application.Services
{
    public class SetupService : ISetupService
    {
        private readonly ISetupRepository _setupRepository;

        public SetupService(ISetupRepository setupRepository) 
        {
            _setupRepository = setupRepository;
        }

        public async Task<CallResponse> Setup()
        {
            return await _setupRepository.Setup();
        }
    }
}
