using PetShop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetShop.Infrastructure.EF.Sqlite
{
    public static class ProductDataInitializer
    {
        public static List<Sale> SalesMockups = new List<Sale>() {
            new Sale(){
                saleid = "1a2b3c4d-5e6f-7g8h-9i0j-k1l2m3n4o5p6",
                productname = "wiskas",
                clienttaxnum = "123456789",
                username = "john_doe",
                quantity = 2,
                price = 159.8, // 2 * 79.9
                domain = "bo"
            },
            new Sale (){
                saleid = "7a8b9c0d-1e2f-3g4h-5i6j-k7l8m9n0o1p2",
                productname = "proplan senior",
                clienttaxnum = "987654321",
                username = "jane_doe",
                quantity = 1,
                price = 110.5, // 1 * 110.5
                domain = "us"
            }
        };


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
            }
            if (!context.Sales.Any()) 
            {
                context.Sales.AddRange(SalesMockups);
            }

            context.SaveChanges();
        }
    }
}
