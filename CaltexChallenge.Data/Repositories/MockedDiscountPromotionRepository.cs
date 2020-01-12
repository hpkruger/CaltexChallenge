using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaltexChallenge.Data.Repositories
{
    public class MockedDiscountPromotionRepository : IDiscountPromotionRepository
    {
        // Obviously, the entities need to come from a proper datastore, e.g. database.
        private static readonly IDictionary<string, DiscountPromotion> DiscountPromotions = new[]
        {
            new DiscountPromotion { Id = "DP001", Name = "Fuel Discount Promo", StartDate = DateTime.Parse("1 Jan 2020"), EndDate = DateTime.Parse("15 Feb 2020"), DiscountPercent = 20},
            new DiscountPromotion { Id = "DP002", Name = "Happy Promo", StartDate = DateTime.Parse("2 Mar 2020"), EndDate = DateTime.Parse("20 Mar 2020"), DiscountPercent = 15},

        }.ToDictionary(item => item.Id, item => item);

        // I declared all methods in the repositories "async" even though the actual implementation is synchronous. 
        // In a real-world scenario fetching data from a datastore, e.g. database, would be done in a asynchronous fashion allowing the request handling thread to be returned back to the aspnet threadpool while the data is retrieved from datastore
        public async Task<IEnumerable<DiscountPromotion>> GetAsync()
        {
            return DiscountPromotions.Values;
        }

        public async Task<DiscountPromotion> GetAsync(string id)
        {
            if (DiscountPromotions.TryGetValue(id, out DiscountPromotion discountPromotion))
            {
                return discountPromotion;
            }
            return null;
        }
    }
}
