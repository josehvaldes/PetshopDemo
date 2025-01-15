using PetShop.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetShop.Data
{
    public interface IUserRepository
    {
        Task<UserEntity?> Retrieve(string domain, string username);
        Task<UserEntity?> Create(UserEntity user);

        Task<bool> Delete(UserEntity user);
    }
}
