using Microsoft.AspNetCore.Mvc;
using Web_BanHang.Models;

namespace Web_BanHang.Controllers;

public class HomeController : Controller
{
    private readonly BanhangdbContext _db;

    public HomeController(BanhangdbContext db)
    {
        _db = db;
    }

    public IActionResult Index()
    {
        var products = _db.Products.Take(12).ToList();
        return View(products);
    }
}