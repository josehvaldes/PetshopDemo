﻿using PetShop.Application.Interfaces.Repository;
using PetShop.Domain.Entities;

namespace PetShop.Infrastructure.Mockup
{
    public class ProductMockup : IProductRepository
    {
        public static List<Product> _productMockups = new List<Product>() {

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

        public async Task<IEnumerable<Product>> RetrieveList(string domain, string type)
        {
            var products = await Task.Run(() =>
            {
                return _productMockups.Where(x => x.domain == domain && x.pettype == type);
            });
            var list = products.ToList();

            return list;
        }

        public async Task<IEnumerable<Product>> RetrieveAvailablesList(string domain, string type)
        {
            var products = await Task.Run(() =>
            {
                return _productMockups.Where(x => x.domain == domain && x.pettype == type && x.stock > 0);
            });
            return products;
        }

        public async Task<Product?> Retrieve(string domain, string name)
        {
            var products = await Task.Run(() =>
            {
                return _productMockups.Where(x => x.domain == domain && x.name == name).FirstOrDefault();
            });
            return products;
        }

        public async Task<bool> Update(Product product)
        {
            return await Task.Run(() =>
            {
                var list = _productMockups.Where(x => x.name == product.name && x.guid == product.guid);
                if (list.Any())
                {
                    var item = list.First();
                    item.stock = product.stock;
                    item.name = product.name;
                    item.unitaryprice = product.unitaryprice;
                    return true;
                }
                else
                {
                    return false;
                }
            });
        }

        public async Task<Product?> Create(Product product)
        {
            return await Task.Run(() =>
            {
                if (!_productMockups.Where(x => x.name == product.name && x.domain == product.domain).Any())
                {
                    _productMockups.Add(product);
                    return product;
                }
                else
                {
                    throw new Exception($"Duplicated client: {product.name}. Domain: {product.domain} ");
                }
            });
        }

        public async Task<bool> Delete(Product product)
        {
            return await Task.Run(() =>
            {
                var list = _productMockups.Where(x => x.name == product.name && x.domain == product.domain);
                if (list.Any())
                {
                    var item = list.First();
                    return true;
                }
                else
                {
                    return false;
                }
            });
        }
    }
}
