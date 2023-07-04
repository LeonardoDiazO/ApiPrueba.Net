namespace DemoTallerApi.Models
{
    public class ProductModel
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
        public int Stock { get; set; }

        public DateTime CreateDate { get; set; }
    }
}
