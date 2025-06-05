using PetShop.Domain.Entities;

namespace PetShopGraphQL.GraphQL
{
    public class SaleWithProduct
    {
        public Sale? Sale { get; set; }
        public Product? Product { get; set; }
    }
}
