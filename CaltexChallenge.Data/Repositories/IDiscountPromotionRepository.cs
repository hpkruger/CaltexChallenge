using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CaltexChallenge.Data.Repositories
{
    public interface IDiscountPromotionRepository
    {
        Task<IEnumerable<DiscountPromotion>> GetAsync();
        Task<DiscountPromotion> GetAsync(string id);

        //@todo: add additional CRUD methods
    }
}
