using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Web_BanHang.Models;

namespace Web_BanHang.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly BanhangdbContext _db;
    public ProductsController(BanhangdbContext db) { _db = db; }

    [HttpGet]
    public IActionResult Get() => Ok(_db.Products.Where(p => p.IsActive == true).Select(p => new { ProductId = p.Id, ProductName = p.Name, BasePrice = p.Price, ImageUrl = p.Thumbnail }).ToList());

    [HttpGet("{id}")]
    public IActionResult Get(int id)
    {
        var p = _db.Products.FirstOrDefault(x => x.Id == id);
        if (p == null) return NotFound();
        return Ok(new { ProductId = p.Id, ProductName = p.Name, BasePrice = p.Price, ImageUrl = p.Thumbnail, p.Description });
    }

    // Protected endpoints (Admin/Staff) for CRUD via API
    [HttpPost]
    [Authorize(Roles = "Admin,Staff")]
    public IActionResult Create([FromBody] Product model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        model.CreatedAt = DateTime.Now;
        _db.Products.Add(model);
        _db.SaveChanges();
        return CreatedAtAction(nameof(Get), new { id = model.ProductId }, model);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Staff")]
    public IActionResult Update(int id, [FromBody] Product model)
    {
        if (id != model.ProductId) return BadRequest();
        _db.Entry(model).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        _db.SaveChanges();
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,Staff")]
    public IActionResult Delete(int id)
    {
        var p = _db.Products.Find(id);
        if (p == null) return NotFound();
        _db.Products.Remove(p);
        _db.SaveChanges();
        return NoContent();
    }
}