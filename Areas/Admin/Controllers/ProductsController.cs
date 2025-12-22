using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Web_BanHang.Models;

namespace Web_BanHang.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Staff")]
    public class ProductsController : Controller
    {
        private readonly BanhangdbContext _db;
        public ProductsController(BanhangdbContext db) { _db = db; }

        public IActionResult Index()
        {
            var products = _db.Products.ToList();
            return View(products);
        }

        public IActionResult Create() => View();

        [HttpPost]
        public IActionResult Create(Product model)
        {
            if (ModelState.IsValid)
            {
                model.CreatedAt = DateTime.Now;
                _db.Products.Add(model);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public IActionResult Edit(int id)
        {
            var p = _db.Products.Find(id);
            if (p == null) return NotFound();
            return View(p);
        }

        [HttpPost]
        public IActionResult Edit(Product model)
        {
            if (ModelState.IsValid)
            {
                _db.Products.Update(model);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var p = _db.Products.Find(id);
            if (p != null)
            {
                _db.Products.Remove(p);
                _db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}