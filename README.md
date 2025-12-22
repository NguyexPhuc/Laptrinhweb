# Web_BanHang — E-commerce (Sports Goods)

Short guide to run locally, apply migrations, and test role-based access.

## What I implemented
- ASP.NET Identity (ApplicationUser) with Roles: **Admin**, **Staff**, **Customer**
- Seeded **Admin** user (email: `admin@local`, username: `admin`, password: `Admin@123`) and sample categories/products
- Strong authentication / security hardening:
  - Uses **ASP.NET Identity** with secure password hashing (PBKDF2) and **increased iteration count** to make brute-force attacks harder
  - Enforces **strong password policy**, **account lockout** after failed attempts, **email confirmation** required for new accounts
  - Authentication cookies configured as **HttpOnly**, **Secure** and **SameSite=Strict**
  - DataProtection keys persisted to disk so tokens survive restarts
- Basic frontend using MVC Views (simple HTML/CSS): Home, Products, Product Details, Account (Login/Register)
- Admin area `/Admin/Products` with CRUD for products (protected by `Admin,Staff` roles)

## Local setup and migrations
1. Ensure the project builds and required tools are installed:
   - .NET SDK 7/8/9 installed
   - Optional: `dotnet-ef` tool: `dotnet tool install --global dotnet-ef`
2. Add EF design package (if not present):
   - `dotnet add package Microsoft.EntityFrameworkCore.Design`
3. Create migration and update database (from project folder):
   - `dotnet ef migrations add InitIdentityAndSchema`
   - `dotnet ef database update`

> Note: The project uses a local connection string currently set in `Program.cs` and `BanhangdbContext.cs`. You can move it to `appsettings.json` under `ConnectionStrings:DefaultConnection`.

## Run the app
- `dotnet run` (or run from Visual Studio). Open `https://localhost:5001` (or the URL shown in console).
- Login as admin: `admin@local` / `Admin@123` to access `/Admin/Products`.

## Next recommended steps
- Move the connection string into `appsettings.json` and protect it with secrets for production
- Add validation, file uploads for product images, paging, search, and unit/integration tests
- Implement REST API endpoints guarded by JWT or cookie auth for SPA/mobile clients

## Admin credentials & security
- Change the default seeded password immediately (use a secure one) before deploying to production.

If you want, I can now:
1) Add EF migrations file and run `dotnet ef database update` (I can show exact commands to run locally), or
2) Continue implementing API endpoints and tests.

Tell me which you'd like me to do next. 🎯