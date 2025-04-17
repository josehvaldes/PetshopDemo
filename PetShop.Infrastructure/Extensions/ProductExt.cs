using PetShop.Domain.Entities;
using PetShop.Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetShop.Infrastructure.Extensions
{
    public static class ProductExt
    {

        public static ProductEntity ToEntity(this Product product) 
        {
            return new ProductEntity() { 
                category = product.category, 
                description = product.description, 
                domain = product.domain, 
                guid = product.guid, 
                name = product.name, 
                pettype = product.pettype, 
                stock = product.stock, 
                unitaryprice = product.unitaryprice 
            };
        }
    }
}
