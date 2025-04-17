using PetShop.Domain.Entities;
using PetShop.Infrastructure.Mockup;

namespace PetShop.Tests.Mocks
{
    public static class ProductFixture
    {

        public static Product GetProduct() 
        {
            var product = new Product()
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

        public static IEnumerable<Product> GetProductList() 
        {
            return ProductMockup._productMockups;
        }
    }
}
