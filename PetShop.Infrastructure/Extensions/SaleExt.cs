using PetShop.Domain.Entities;
using PetShop.Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetShop.Infrastructure.Extensions
{
    public static class SaleExt
    {
        public static SaleEntity ToEntity(this Sale sale) 
        {
            return new SaleEntity
            {
                productname = sale.productname,
                clienttaxnum = sale.clienttaxnum,
                username = sale.username,
                quantity = sale.quantity,
                price = sale.price,
                domain = sale.domain,
                saleid = sale.saleid
            };
        }
    }
}
