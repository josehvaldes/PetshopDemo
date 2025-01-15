using Microsoft.Extensions.Primitives;
using PetShop.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetShop.Service
{
    public interface IClientService
    {
        Task<ClientEntity?> Create(ClientRequest request);
        Task<ClientEntity?> Retrieve(string taxNumber);
        Task<bool> Update(ClientRequest request);
        Task<bool> Delete(string taxnumber);
    }
}
