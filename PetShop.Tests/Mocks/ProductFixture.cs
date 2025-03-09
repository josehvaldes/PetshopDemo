using Azure;
using PetShop.Data.Mockup;
using PetShop.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetShop.Tests.Mocks
{
    public static class ProductFixture
    {

        public static ProductEntity GetProduct() 
        {
            var product = new ProductEntity()
            {
                guid = "19a31d90-8123-4288-b9a6-e704aadde359",
                pettype = "cat",
                name = "dog chow",
                description = "food for cats",
                category = "food",
                stock = 20,
                unitaryprice = 79.9,
                domain = "bo",
            }; 

            return product;
        }

        public static IEnumerable<ProductEntity> GetProductList() 
        {
            return ProductMockup._productMockups;
        }
    }
}
