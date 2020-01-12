using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaltexChallenge.Data.Repositories
{
    public class MockedDiscountPromotionProductRepository : IDiscountPromotionProductRepository
    {
        // Obviously, the entities need to come from a proper datastore, e.g. database.
        private static readonly IList<DiscountPromotionProduct> DiscountPromotionProducts = new[]
        {
            new DiscountPromotionProduct { DiscountPromotionId = "DP001", ProductId = "PRD01"},
            new DiscountPromotionProduct { DiscountPromotionId = "DP001", ProductId = "PRD02"}
        }.ToList();

        // I declared all methods in the repositories "async" even though the actual implementation is synchronous. 
        // In a real-world scenario fetching data from a datastore, e.g. database, would be done in a asynchronous fashion allowing the request handling thread to be returned back to the aspnet threadpool while the data is retrieved from datastore
        public async Task<IEnumerable<DiscountPromotionProduct>> GetAsync()
        {
            return DiscountPromotionProducts;
        }
    }
}
