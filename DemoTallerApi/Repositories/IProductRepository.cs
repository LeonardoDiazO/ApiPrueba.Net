using DemoTallerApi.Models;

namespace DemoTallerApi.Repositories
{
    public interface IProductRepository : IRepository<ProducEntity>
    {
        Task<IEnumerable<ProducEntity>> GetActiveProductsAsync();
        Task<IEnumerable<ProducEntity>> GetProductsByCategoryAsync(string category);
        Task<IEnumerable<ProducEntity>> GetPagedProductsAsync(int pageNumber, int pageSize);
    }
}
