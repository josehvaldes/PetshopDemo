using PetShop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetShop.Infrastructure.EF.Sqlite
{
    public static class ProductData
    {

        public static List<Product> ProductMockups = new List<Product>() {

            new Product(){
                guid="19a31d90-8123-4288-b9a6-e704aadde359",
                pettype="cat",
                name="wiskas",
                description="food for cats",
                category="food",
                stock = 20,
                unitaryprice = 79.9,
                domain = "bo",
            },
            new Product(){
                guid="6bf3c70c-9705-4b57-98a1-37596844bd5b",
                pettype="cat",
                name="purina gormet",
                description="premium food for cats",
                category="food",
                stock = 0,
                unitaryprice = 169.9,
                domain = "bo",
            },
            new Product(){
                guid="88ba55eb-6a15-46bc-82b0-9d0d4741efb0",
                pettype="dog",
                name="proplan senior",
                description="food for senior dogs",
                category="food",
                stock = 50,
                unitaryprice = 110.5,
                domain = "us",
            },
            new Product(){
                guid="b9d3c921-712f-4a3a-80c4-819fd88356a9",
                pettype="dog",
                name="Royal Canine",
                description="premium food for dogs",
                category="food",
                stock = 0,
                unitaryprice = 149.9,
                domain = "us",
            }
        };

        /// <summary>
        /// Lazy initializer for the Product data.
        /// </summary>
        /// <param name="context"></param>
        public static void LazyInitializer(PetShopContext context ) 
        {
            if (!context.Products.Any())
            {
                context.Products.AddRange(ProductMockups);
                context.SaveChanges();
            }
        }
    }
}
