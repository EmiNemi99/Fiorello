using Fiorello.DAL;
using Fiorello.Helper;
using Fiorello.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Fiorello.Areas.Admin.Controllers
{
    [Area(nameof(Admin))]
    public class ProductController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;

        public ProductController(AppDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }
        public async Task<IActionResult> Index(int page=1)
        {
            int take = 3;
            ViewBag.PageCount = Math.Ceiling((decimal)await _db.Products.CountAsync() / take);
            ViewBag.CurrentPage = page;
            List<Product> products = await _db.Products.Skip((page-1)* take).Take(take).Include(x => x.Category).ToListAsync();
            return View(products);
        }
        public async Task<IActionResult> Create()
        {

            ViewBag.Categories = await _db.Categories.ToListAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, int catId)
        {
            ViewBag.Categories = await _db.Categories.ToListAsync();
            if (!ModelState.IsValid)
            {
                return View();
            }
            bool isExist = await _db.Products.AnyAsync(x => x.Name == product.Name);
            if (isExist)
            {
                ModelState.AddModelError("Title", "Bu adda artiq Məhsul yaradilib");
                return View();
            }
            if (product.Photo != null)
            {
                if (!product.Photo.IsImage())
                {
                    ModelState.AddModelError("Photo", "Zəhmət olmasa şəkli seçin");
                    return View();
                }
                if (product.Photo.IsOlderMaxMB())
                {
                    ModelState.AddModelError("Photo", "2MB dan çox ola bilməz");
                    return View();
                }
                string path = Path.Combine(_env.WebRootPath, "img");
                product.Image = await product.Photo.SaveFileAsync(path);
                product.CategoryId = catId;
                await _db.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Activity(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Product product = await _db.Products.FirstOrDefaultAsync(x => x.Id == id);
            if (product == null)
            {
                return BadRequest();
            }
            if (product.IsDeactive)
            {
                product.IsDeactive = false;
            }
            else
            {
                product.IsDeactive = true;
            }
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Update(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Product dbproduct = await _db.Products.FirstOrDefaultAsync(x => x.Id == id);

            if (dbproduct == null)
            {
                return BadRequest();
            }
            ViewBag.Categories = await _db.Categories.ToListAsync();
            return View(dbproduct);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, Product product, int catId)
        {
            if (id == null)
            {
                return NotFound();
            }
            Product dbproduct = await _db.Products.FirstOrDefaultAsync(x => x.Id == id);

            if (dbproduct == null)
            {
                return BadRequest();
            }
            ViewBag.Categories = await _db.Categories.ToListAsync();

            if (!ModelState.IsValid)
            {
                return View(dbproduct);
            }

            bool isExist = await _db.Products.AnyAsync(x =>x.Name == product.Name && x.Id != id);
            if (isExist)
            {
                ModelState.AddModelError("Title", "Bu adda artiq Məhsul yaradilib");
                return View(dbproduct);
            }
            if (product.Photo != null)
            {
                if (!product.Photo.IsImage())
                {
                    ModelState.AddModelError("Photo", "Zəhmət olmasa şəkli seçin");
                    return View(dbproduct);
                }
                if (product.Photo.IsOlderMaxMB())
                {
                    ModelState.AddModelError("Photo", "2MB dan çox ola bilməz");
                    return View(dbproduct);
                }
                string path = Path.Combine(_env.WebRootPath, "img");
                dbproduct.Image = await product.Photo.SaveFileAsync(path);
            }

            dbproduct.CategoryId = catId;
            dbproduct.Name = product.Name;
            dbproduct.Price = product.Price;
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        
        } 
    }
}
