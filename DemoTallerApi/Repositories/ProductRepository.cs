using DemoTallerApi.Data;
using DemoTallerApi.Models;
using Microsoft.EntityFrameworkCore;

namespace DemoTallerApi.Repositories
{
    public class ProductRepository : Repository<ProducEntity>, IProductRepository
    {
        public ProductRepository(StoreDBContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ProducEntity>> GetActiveProductsAsync()
        {
            return await _dbSet
                .Where(p => p.IsActive)
                .OrderByDescending(p => p.CreateDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<ProducEntity>> GetProductsByCategoryAsync(string category)
        {
            return await _dbSet
                .Where(p => p.IsActive && p.Category == category)
                .ToListAsync();
        }

        public async Task<IEnumerable<ProducEntity>> GetPagedProductsAsync(int pageNumber, int pageSize)
        {
            return await _dbSet
                .Where(p => p.IsActive)
                .OrderByDescending(p => p.CreateDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
    }
}
