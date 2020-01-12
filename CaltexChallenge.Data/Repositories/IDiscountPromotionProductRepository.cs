using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CaltexChallenge.Data.Repositories
{
    public interface IDiscountPromotionProductRepository
    {
        Task<IEnumerable<DiscountPromotionProduct>> GetAsync();

        //@todo: add additional CRUD methods
    }
}
