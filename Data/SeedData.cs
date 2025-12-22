using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using Web_BanHang.Models;

namespace Web_BanHang.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            string[] roles = new[] { "Admin", "Staff", "Customer" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            var adminEmail = "admin@local";
            var admin = await userManager.FindByEmailAsync(adminEmail);
            if (admin == null)
            {
                admin = new ApplicationUser
                {
                    UserName = "admin",
                    Email = adminEmail,
                    EmailConfirmed = true,
                    FullName = "Administrator"
                };
                var result = await userManager.CreateAsync(admin, "Admin@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                }
            }

            // If there are existing custom users (legacy table), we cannot convert hashed passwords reliably.
            // Recommended: mark existing users to require password reset on next login and send reset emails.
            // (Implement a migration or user flow separately.)

            // Seed sample categories & products
            var db = serviceProvider.GetService<BanhangdbContext>();
            if (db != null)
            {
                if (!db.Categories.Any())
                {
                    var c1 = new Category { CategoryName = "Giày", Slug = "giay" };
                    var c2 = new Category { CategoryName = "Quần áo", Slug = "quan-ao" };
                    db.Categories.AddRange(c1, c2);
                    db.SaveChanges();
                }

                if (!db.Products.Any())
                {
                    var cat = db.Categories.First();
                    db.Products.Add(new Product { ProductName = "Giày chạy bộ A", BasePrice = 100m, CategoryId = cat.CategoryId, ImageUrl = "/images/sample1.jpg", Slug = "giay-chay-bo-a", IsActive = true });
                    db.Products.Add(new Product { ProductName = "Áo thể thao B", BasePrice = 50m, CategoryId = cat.CategoryId, ImageUrl = "/images/sample2.jpg", Slug = "ao-the-thao-b", IsActive = true });
                    db.SaveChanges();
                }
            }
        }
    }
}