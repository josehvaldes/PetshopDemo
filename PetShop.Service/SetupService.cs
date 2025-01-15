using PetShop.Data;
using PetShop.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetShop.Service
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
