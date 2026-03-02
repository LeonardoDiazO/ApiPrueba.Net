using System.ComponentModel.DataAnnotations;

namespace DemoTallerApi.DTOs
{
    public class UpdateProductDto
    {
        [Required(ErrorMessage = "El ID del producto es requerido")]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del producto es requerido")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 100 caracteres")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
        public string? Description { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "El stock debe ser mayor o igual a 0")]
        public int Stock { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0")]
        public decimal Price { get; set; }

        [StringLength(50, ErrorMessage = "La categoría no puede exceder 50 caracteres")]
        public string? Category { get; set; }
    }
}
