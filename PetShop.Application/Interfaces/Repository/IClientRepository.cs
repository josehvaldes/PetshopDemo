using PetShop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetShop.Application.Interfaces.Repository
{
    public interface IClientRepository
    {
        Task<Client?> Create(Client client);
        Task<Client?> Retrieve(string taxNumberEnd, string taxNumber);
        Task<bool> Update(Client client);
        Task<bool> Delete(Client client);
    }
}
