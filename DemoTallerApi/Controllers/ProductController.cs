using DemoTallerApi.Models;
using DemoTallerApi.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DemoTallerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            this._productService = productService;
        }
        [HttpGet]
        public IActionResult GetAllProduct() 
        {
            List<ProducEntity> product =  this._productService.GetAllProduct();
            return Ok(product);
        }
        [HttpPost]
        public IActionResult CreateProduct([FromBody] ProductModel product) 
        {
            _productService.CreateProduct(product);
            return Ok();
        }
        [HttpDelete]
        public IActionResult DeleteProduct([FromQuery] int id)
        {
            _productService.DeleteProduct(id);
            return Ok();
        }
        [HttpGet]
        [Route("getProduct/{id}")]
        public IActionResult GetProduct(int id) 
        {
            ProducEntity product = _productService.GetProductById(id);
            return Ok(product);
        }

    }
}
