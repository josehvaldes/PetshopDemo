using PetShop.Application.Requests;
using PetShop.Application.Services;
using PetShop.Domain.Entities;

namespace PetShopGraphQL.GraphQL
{
    /// <summary>
    /// Mutation class for adding new products to the Pet Shop.
    /// </summary>
    public class Mutation
    {
        public Product? AddBook(
        [Service] ProductService repository,
        string domain, string petType, string name)
        {
            var productRequest = new ProductRequest
            {
                Name = name,
                PetType = petType,
                Domain = domain,
            };

            var product = repository.Create(productRequest).Result;
            return product;
        }
    }
}
