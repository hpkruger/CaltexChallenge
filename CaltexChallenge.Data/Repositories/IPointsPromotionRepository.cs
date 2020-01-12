using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CaltexChallenge.Data.Repositories
{
    public interface IPointsPromotionRepository
    {
        Task<IEnumerable<PointsPromotion>> GetAsync();
        Task<PointsPromotion> GetAsync(string id);

        //@todo: add additional CRUD methods
    }
}
