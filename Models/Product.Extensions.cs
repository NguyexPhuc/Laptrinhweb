using System.ComponentModel.DataAnnotations.Schema;

namespace Web_BanHang.Models
{
    public partial class Product
    {
        [NotMapped]
        public int ProductId { get => Id; set => Id = value; }

        [NotMapped]
        public string ProductName { get => Name; set => Name = value; }

        [NotMapped]
        public decimal BasePrice { get => Price; set => Price = value; }

        [NotMapped]
        public string? ImageUrl { get => Thumbnail; set => Thumbnail = value; }

        [NotMapped]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}