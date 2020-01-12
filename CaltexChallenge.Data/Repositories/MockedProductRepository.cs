using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaltexChallenge.Data.Repositories
{
    public class MockedProductRepository : IProductRepository
    {
        // Obviously, the entities need to come from a proper datastore, e.g. database.
        private static readonly IDictionary<string, Product> Products = new[]
        {
            new Product { Id = "PRD01", Name = "Vortex 95", Category = "Fuel", UnitPrice = 1.2m},
            new Product { Id = "PRD02", Name = "Vortex 98", Category = "Fuel", UnitPrice = 1.3m},
            new Product { Id = "PRD03", Name = "Diesel", Category = "Fuel", UnitPrice = 1.1m},
            new Product { Id = "PRD04", Name = "Twix 55g", Category = "Shop", UnitPrice = 2.3m},
            new Product { Id = "PRD05", Name = "Mars 72g", Category = "Shop", UnitPrice = 5.1m},
            new Product { Id = "PRD06", Name = "SNICKERS 72G", Category = "Shop", UnitPrice = 3.4m},
            new Product { Id = "PRD07", Name = "Bounty 3 63g", Category = "Shop", UnitPrice = 6.9m},
            new Product { Id = "PRD08", Name = "Snickers 50g", Category = "Shop", UnitPrice = 4.0m}

        }.ToDictionary(item => item.Id, item => item);

        // I declared all methods in the repositories "async" even though the actual implementation is synchronous. 
        // In a real-world scenario fetching data from a datastore, e.g. database, would be done in a asynchronous fashion allowing the request handling thread to be returned back to the aspnet threadpool while the data is retrieved from datastore
        public async Task<IEnumerable<Product>> GetAsync()
        {
            return Products.Values;
        }

        public async Task<Product> GetAsync(string id)
        {
            if (Products.TryGetValue(id, out Product product))
            {
                return product;
            }
            return null;
        }
        public async Task<IEnumerable<Product>> GetAsync(IEnumerable<string> ids)
        {
            var products = new List<Product>();
            ids.ToList().ForEach(id =>
            {
                // Since we are using a dictionary here as a data store, a product can be looked up in O(1).
                if (Products.TryGetValue(id, out Product product))
                {
                    products.Add(product);
                }
            });
            return products;
        }
    }
}
