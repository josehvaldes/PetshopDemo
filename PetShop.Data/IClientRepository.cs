using PetShop.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetShop.Data
{
    public interface IClientRepository
    {

        Task<ClientEntity?> Create(ClientEntity client);
        Task<ClientEntity?> Retrieve(string taxNumberEnd, string taxNumber);
        Task<bool> Update(ClientEntity client);
        Task<bool> Delete(ClientEntity client);
    }
}
