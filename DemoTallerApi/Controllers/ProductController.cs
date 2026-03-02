using DemoTallerApi.DTOs;
using DemoTallerApi.Models;
using DemoTallerApi.Repositories;
using DemoTallerApi.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;

namespace DemoTallerApi.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public ProductController(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Obtiene todos los productos activos (paginado)
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<PaginatedResponseDto<ProductDto>>> GetAllProducts(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? category = null)
        {
            if (pageNumber < 1 || pageSize < 1 || pageSize > 100)
            {
                return BadRequest("Parámetros de paginación inválidos");
            }

            IEnumerable<ProducEntity> products;
            int totalCount;

            if (!string.IsNullOrEmpty(category))
            {
                products = await _productRepository.GetProductsByCategoryAsync(category);
                totalCount = products.Count();
                products = products.Skip((pageNumber - 1) * pageSize).Take(pageSize);
            }
            else
            {
                products = await _productRepository.GetPagedProductsAsync(pageNumber, pageSize);
                totalCount = await _productRepository.CountAsync(p => p.IsActive);
            }

            var productDtos = _mapper.Map<List<ProductDto>>(products);

            var result = new PaginatedResponseDto<ProductDto>
            {
                Items = productDtos,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            return Ok(result);
        }

        /// <summary>
        /// Obtiene un producto por ID
        /// </summary>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<ProductDto>> GetProduct(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            
            if (product == null || !product.IsActive)
            {
                return NotFound(new { message = "Producto no encontrado" });
            }

            var productDto = _mapper.Map<ProductDto>(product);
            return Ok(productDto);
        }

        /// <summary>
        /// Crea un nuevo producto
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ProductDto>> CreateProduct([FromBody] CreateProductDto createProductDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var product = _mapper.Map<ProducEntity>(createProductDto);
            var createdProduct = await _productRepository.AddAsync(product);
            var productDto = _mapper.Map<ProductDto>(createdProduct);

            return CreatedAtAction(nameof(GetProduct), new { id = productDto.Id }, productDto);
        }

        /// <summary>
        /// Actualiza un producto existente
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ProductDto>> UpdateProduct(int id, [FromBody] UpdateProductDto updateProductDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != updateProductDto.Id)
            {
                return BadRequest("El ID del producto no coincide");
            }

            var existingProduct = await _productRepository.GetByIdAsync(id);
            if (existingProduct == null || !existingProduct.IsActive)
            {
                return NotFound(new { message = "Producto no encontrado" });
            }

            _mapper.Map(updateProductDto, existingProduct);
            await _productRepository.UpdateAsync(existingProduct);

            var productDto = _mapper.Map<ProductDto>(existingProduct);
            return Ok(productDto);
        }

        /// <summary>
        /// Elimina un producto (soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound(new { message = "Producto no encontrado" });
            }

            product.IsActive = false;
            product.UpdatedDate = DateTime.Now;
            await _productRepository.UpdateAsync(product);

            return NoContent();
        }
    }
}
