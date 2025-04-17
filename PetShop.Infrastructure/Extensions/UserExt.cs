using PetShop.Domain.Entities;
using PetShop.Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PetShop.Infrastructure.Extensions
{
    public static class UserExt
    {

        public static UserEntity ToEntity(this User user) 
        {
            return new UserEntity
            {
                guid = user.guid,
                hash = user.hash,
                email = user.email,
                roles = user.roles,
                domain = user.domain,
                username = user.username
            };
        }

    }
}
