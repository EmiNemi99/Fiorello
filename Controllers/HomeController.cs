using Fiorello.DAL;
using Fiorello.Models;
using Fiorello.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Fiorello.Controllers
{

    public class HomeController : Controller
    {
        private readonly AppDbContext _db;
        public HomeController(AppDbContext db)
        {
            _db = db;
        }


        public async Task<IActionResult> Index()
        {

            List<Product> products = await _db.Products.ToListAsync();
            List<Category> categories = await _db.Categories.Where(x => !x.IsDeactive).ToListAsync();
            HomeVM homeVM = new HomeVM
            {
                Categories= categories,
                Products=products
                
            };
            return View(homeVM);


        }
            
            
           
        



    }
}
