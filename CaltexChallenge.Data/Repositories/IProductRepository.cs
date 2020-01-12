using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CaltexChallenge.Data.Repositories
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAsync();
        Task<Product> GetAsync(string id);
        Task<IEnumerable<Product>> GetAsync(IEnumerable<string> ids);

        //@todo: add additional CRUD methods
    }
}
