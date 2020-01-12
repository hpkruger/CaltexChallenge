using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaltexChallenge.Data.Repositories
{
    public class MockedPointsPromotionRepository : IPointsPromotionRepository
    {
        // Obviously, the entities need to come from a proper datastore, e.g. database.
        private static readonly IDictionary<string, PointsPromotion> PointsPromotions = new[]
        {
            new PointsPromotion { Id = "PP001", Name = "New Year Promo", StartDate = DateTime.Parse("1 Jan 2020"), EndDate = DateTime.Parse("30 Jan 2020"), Category = "Any", Points = 2},
            new PointsPromotion { Id = "PP002", Name = "Fuel Promo", StartDate = DateTime.Parse("5 Feb 2020"), EndDate = DateTime.Parse("15 Feb 2020"), Category = "Fuel", Points = 3},
            new PointsPromotion { Id = "PP003", Name = "Shop Promo ", StartDate = DateTime.Parse("1 Mar 2020"), EndDate = DateTime.Parse("20 Mar 2020"), Category = "Shop", Points = 4},

        }.ToDictionary(item => item.Id, item => item);

        // I declared all methods in the repositories "async" even though the actual implementation is synchronous. 
        // In a real-world scenario fetching data from a datastore, e.g. database, would be done in a asynchronous fashion allowing the request handling thread to be returned back to the aspnet threadpool while the data is retrieved from datastore
        public async Task<IEnumerable<PointsPromotion>> GetAsync()
        {
            return PointsPromotions.Values;
        }

        public async Task<PointsPromotion> GetAsync(string id)
        {
            if (PointsPromotions.TryGetValue(id, out PointsPromotion pointsPromotion))
            {
                return pointsPromotion;
            }
            return null;
        }
    }
}
