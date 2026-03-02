using System.ComponentModel.DataAnnotations;

namespace DemoTallerApi.DTOs
{
    public class CreateOrderDto
    {
        [Required(ErrorMessage = "El ID de usuario es requerido")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Debe incluir al menos un item en la orden")]
        [MinLength(1, ErrorMessage = "Debe incluir al menos un item en la orden")]
        public List<CreateOrderItemDto> OrderItems { get; set; } = new();
    }

    public class CreateOrderItemDto
    {
        [Required(ErrorMessage = "El ID del producto es requerido")]
        public int ProductId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public int Quantity { get; set; }
    }
}
