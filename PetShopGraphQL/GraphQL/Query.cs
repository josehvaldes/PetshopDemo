using PetShop.Application.Interfaces.Services;
using PetShop.Domain.Entities;
using System;

namespace PetShopGraphQL.GraphQL
{
    /// <summary>
    /// Query class for retrieving products from the Pet Shop.
    /// </summary>
    public class Query
    {

        /// <summary>
        /// 
        /// Query sample in graphql:
        //  query {
        //  products(domain: "bo", type: "cat")
        //        {
        //            guid
        //            category
        //            domain
        //             name
        //    }
        //  }
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="domain"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [UseFiltering]
        [UseSorting]
        public IQueryable<Product>  GetProducts([Service] IProductService repository, string domain, string type)
            => repository.RetrieveAvailablesList(domain, type).Result.AsQueryable();


        /// <summary>
        /// Query sample for ProductByName method
        /// query {
        //        productByName(domain: "bo", type: "cat", name: "wiskas")
        //        {
        //          guid
        //          category
        //          domain
        //          name
        //        }
        //}
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="domain"></param>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public Product? GetProductByName([Service] IProductService repository, string domain, string type, string name)
            => repository.RetrieveAvailablesList(domain, type).Result.FirstOrDefault(x => x.name == name);
    }
}
