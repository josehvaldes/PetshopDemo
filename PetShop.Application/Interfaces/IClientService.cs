using PetShop.Application.Requests;
using PetShop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetShop.Application.Interfaces
{
    public interface IClientService
    {
        Task<Client?> Create(ClientRequest request);
        Task<Client?> Retrieve(string taxNumber);
        Task<bool> Update(ClientRequest request);
        Task<bool> Delete(string taxnumber);
    }
}
