using PetShop.Domain.Entities;
using PetShop.Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetShop.Infrastructure.Extensions
{
    public static class ClientExt
    {
        public static ClientEntity ToEntity(this Client client) 
        {
            return new ClientEntity() {
                guid = client.guid,
                fullname = client.fullname,
                taxnumber = client.taxnumber,
                taxnumberend = client.taxnumberend
            };
        }
    }
}
