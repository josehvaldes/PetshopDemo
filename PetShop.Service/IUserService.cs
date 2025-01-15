using PetShop.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetShop.Service
{
    public interface IUserService
    {

        Task<UserEntity?> Retrieve(string domain, string username);

        Task<UserEntity?> Create(UserEntity user);

        Task<CallResponse> Delete(string domain, string username);

    }
}
