using Fiorello.DAL;
using Fiorello.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fiorello.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {

        private readonly AppDbContext _db;
        public CategoryController(AppDbContext db)
        {
            _db = db;
        }
        public async Task<IActionResult> Index()
        {
            List<Category> categories = await _db.Categories.ToListAsync();
            return View(categories);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            bool isExist = await _db.Categories.AnyAsync(x => x.Name == category.Name);
            if (isExist)
            {
                ModelState.AddModelError("Title", "Bu adda artiq Servis yaradilib");
                return View();
            }
            await _db.Categories.AddAsync(category);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Activity(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Category category = await _db.Categories.FirstOrDefaultAsync(x => x.Id == id);
            if (category == null)
            {
                return BadRequest();
            }
            if (category.IsDeactive)
            {
                category.IsDeactive = false;
            }
            else
            {
                category.IsDeactive = true;
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
            Category category = await _db.Categories.FirstOrDefaultAsync(x => x.Id == id);

            if (category == null)
            {
                return BadRequest();
            }
            return View(category);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, Category category)
        {
            if (id == null)
            {
                return NotFound();
            }
            Category dbcategory = await _db.Categories.FirstOrDefaultAsync(x => x.Id == id);
            if (dbcategory == null)
            {
                return BadRequest();
            }
            if (!ModelState.IsValid)
            {
                return View(dbcategory);
            }
            bool isExist = await _db.Categories.AnyAsync(x => x.Name == category.Name && x.Id != id);
            if (isExist)
            {
                ModelState.AddModelError("Title", "Bu adda artiq Servis yaradilib");
                return View(dbcategory);
            }
            dbcategory.Name = category.Name;
            dbcategory.Products = category.Products;
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Category categories = await _db.Categories.FirstOrDefaultAsync(x => x.Id == id);

            return View(categories);
        }

        
    }
}
