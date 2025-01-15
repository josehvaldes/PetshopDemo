using Azure;
using Microsoft.Extensions.Logging;
using PetShop.Data;
using PetShop.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetShop.Service
{
    public class ClientService : IClientService
    {
        private readonly IClientRepository _clientRepository;
        private readonly ILogger<ClientService> _logger;
        public ClientService(ILogger<ClientService> logger, IClientRepository clientRepository) 
        {
            _clientRepository = clientRepository;
            _logger = logger;
        }

        public async Task<ClientEntity?> Create(ClientRequest request)
        {
            var client = new ClientEntity();
            client.guid = Guid.NewGuid().ToString();
            client.ETag = ETag.All;
            client.fullname = request.FullName;
            client.taxnumber = request.TaxNumber;
            //set PartitionKey
            client.taxnumberend = client.taxnumber.ElementAt(client.taxnumber.Length - 1).ToString();
            return await _clientRepository.Create(client);
        }

        public async Task<bool> Delete(string taxNumber)
        {
            var taxnumberend = taxNumber.ElementAt(taxNumber.Length-1).ToString();
            var client = await _clientRepository.Retrieve(taxnumberend,taxNumber);
            if (client != null)
            {
                return await _clientRepository.Delete(client);
            }
            else 
            {
                _logger.LogWarning($"Delete failed. Client not Found: {taxNumber}");
                return false;
            }
        }

        public async Task<ClientEntity?> Retrieve(string taxNumber)
        {
            var taxNumberEnd = taxNumber.ElementAt(taxNumber.Length - 1).ToString();
            return await _clientRepository.Retrieve(taxNumberEnd,taxNumber);
        }

        public async Task<bool> Update(ClientRequest request)
        {
            var taxNumberEnd = request.TaxNumber.ElementAt(request.TaxNumber.Length - 1).ToString();
            var entity = await _clientRepository.Retrieve(taxNumberEnd, request.TaxNumber);

            if (entity != null)
            {
                entity.fullname = request.FullName;
                return await _clientRepository.Update(entity);
            }
            else 
            {
                _logger.LogWarning($"Update failed. Client {request.TaxNumber} not Found.");
                return false;
            }            
        }
    }
}
