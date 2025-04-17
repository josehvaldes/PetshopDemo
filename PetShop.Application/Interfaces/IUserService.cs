using PetShop.Application.Requests;
using PetShop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetShop.Application.Interfaces
{
    public interface IUserService
    {

        Task<User?> Retrieve(string domain, string username);

        Task<User?> Create(User user);

        Task<CallResponse> Delete(string domain, string username);

    }
}
