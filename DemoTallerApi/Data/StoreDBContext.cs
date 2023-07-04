using DemoTallerApi.Models;
using Microsoft.EntityFrameworkCore;

namespace DemoTallerApi.Data
{
    public class StoreDBContext : DbContext
    {
        public StoreDBContext(DbContextOptions<StoreDBContext> options) : base(options)
        {
        }
        public DbSet<ProducEntity> Products { get; set; }
    }
}
