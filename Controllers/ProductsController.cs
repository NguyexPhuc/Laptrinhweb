using Microsoft.AspNetCore.Mvc;
using Web_BanHang.Models;

namespace Web_BanHang.Controllers;

public class ProductsController : Controller
{
    private readonly BanhangdbContext _db;
    public ProductsController(BanhangdbContext db) { _db = db; }

    public IActionResult Index()
    {
        var products = _db.Products.Where(p => p.IsActive == true).ToList();
        return View(products);
    }

    public IActionResult Details(int id)
    {
        // Use actual DB key (Id) to avoid EF translation issues with NotMapped properties
        var p = _db.Products.FirstOrDefault(x => x.Id == id);
        if (p == null) return NotFound();
        return View(p);
    }
}