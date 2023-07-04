using DemoTallerApi.Models;

namespace DemoTallerApi.Service
{
    public interface IProductService
    {
        void CreateProduct(ProductModel product);
        void DeleteProduct(int id);
        List<ProducEntity> GetAllProduct();
        ProducEntity GetProductById(int id);
        void UpdateProduct(ProductModel product);
    }
}