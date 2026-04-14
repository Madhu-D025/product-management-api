using System.ComponentModel.DataAnnotations;

namespace AuthApplication.DTOs
{
    public class ProductDto
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public bool? IsActive { get; set; } = true;
        public string? UserId { get; set; }
    }
}