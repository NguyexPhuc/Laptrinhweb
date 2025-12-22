using System.ComponentModel.DataAnnotations.Schema;

namespace Web_BanHang.Models
{
    public partial class Category
    {
        [NotMapped]
        public int CategoryId { get => Id; set => Id = value; }

        [NotMapped]
        public string? CategoryName { get => Name; set => Name = value; }
    }
}