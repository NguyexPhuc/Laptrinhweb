using Microsoft.EntityFrameworkCore;

namespace Web_BanHang.Models
{
    // Compatibility wrapper: map legacy BanhangdbContext to scaffolded FashionEcommerceDbContext
    public class BanhangdbContext : FashionEcommerceDbContext
    {
        public BanhangdbContext(DbContextOptions<FashionEcommerceDbContext> options) : base(options) { }
    }
}