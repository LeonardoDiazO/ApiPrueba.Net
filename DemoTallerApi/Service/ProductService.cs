using DemoTallerApi.Data;
using DemoTallerApi.Models;

namespace DemoTallerApi.Service
{
    public class ProductService : IProductService
    {
        private readonly StoreDBContext _context;

        public ProductService(StoreDBContext context)
        {
            this._context = context;
        }

        public void CreateProduct(ProductModel product)
        {
            ProducEntity producEntity = new()
            {
                Name = product.Name,
                Description = product.Description,
                Stock = product.Stock,
                CreateDate = DateTime.Now,
            };

            _context.Products.Add(producEntity);
            _context.SaveChanges();

        }
        public void UpdateProduct(ProductModel product)
        {

        }
        public void DeleteProduct(int id)
        {
            ProducEntity productToDelete = this.GetProductById(id);
            _context.Products.Remove(productToDelete);
            _context.SaveChanges();
        }
        public ProducEntity GetProductById(int id)
        {
            return _context.Products.Find(id);
        }
        public List<ProducEntity> GetAllProduct()
        {
            return _context.Products.ToList();
        }
    }
}
