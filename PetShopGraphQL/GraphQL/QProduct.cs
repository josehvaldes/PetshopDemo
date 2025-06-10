using PetShop.Application.Interfaces.Services;
using PetShop.Domain.Entities;

namespace PetShopGraphQL.GraphQL
{
    /// <summary>
    /// Query class for retrieving products and their associated sales from the Pet Shop.
    /// </summary>
    public class QProduct
    {
        /// <summary>
        /// ID of the product.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Name of the product.
        /// </summary>
        public string Name { get; set; } = null!;
        // other properties

        /// <summary>
        /// Retrieves the sales associated with this product.
        /// </summary>
        /// <param name="repo"></param>
        /// <returns></returns>
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<Sale> GetSalesByProduct([Service] IProductService repo)
            => repo.GetQueryableSales().Result.Where(s => s.productname == this.Name);
    }
}
