﻿using PetShop.Application.Interfaces.Services;
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
        /// <code>
        /// query {
        //    products(where:{ stock: { gt: 0 }})
        //    {
        //      guid
        //      category
        //      domain
        //      name
        //      stock
        //    }
        //  }
        /// </code>
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="domain"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [UseProjection]
        [UseFiltering]
        [UseSorting]        
        public IQueryable<Product>  GetProducts([Service] IProductService service)
            => service.GetQueryableProducts().Result;

        /// <summary>
        /// Query sample in graphql:
        /// 
        /// </summary>
        /// <param name="service"></param>
        /// <returns></returns>
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<Sale> GetSales([Service] IProductService service)
            => service.GetQueryableSales().Result;

        /// <summary>
        /// Query method to JOIN Sales and Products
        /// query {
        //  salesWithProduct()
        //  {
        //    sale {
        //          quantity
        //          price
        //          }
        //    product {
        //      domain
        //      name
        //    }
        //  }
        
        /// </summary>
        /// <returns></returns>
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<SaleWithProduct> GetSalesWithProduct([Service] IProductService service) 
        {

            //MongoDB does not support cross-collection joins natively in LINQ. Always materialize both collections before joining.
            //THIS CODE FAILES WITH MONGO DB
            var sales = service.GetQueryableSales().Result; //.ToList();
            var products = service.GetQueryableProducts().Result;//.ToList();
           return sales.Select( s => new SaleWithProduct() { 
               Sale = s,
               Product = products.FirstOrDefault(p => p.name == s.productname)
           }).AsQueryable();
        }

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
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public Product? GetProductByName([Service] IProductService service, string domain, string type, string name)
            => service.RetrieveAvailablesList(domain, type).Result.FirstOrDefault(x => x.name == name);


        /// <summary>
        /// Query sample for QProducts method
        /// Sample:
        /// query{
        //     qProducts {
        //     name
        //     salesByProduct
        //     {
        //      quantity
        //      price
        //      }
        //    }
        // }
        /// </summary>
        /// <param name="service"></param>
        /// <returns></returns>
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<QProduct> GetQProducts([Service] IProductService service) 
        {
            return service.GetQueryableProducts().Result.Select( p => new QProduct { 
                Id = p.guid,
                Name = p.name
            } );
        }
    }
}
