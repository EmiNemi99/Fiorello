using Fiorello.Models;
using Fiorello.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fiorello.Areas.Admin.Controllers
{
    [Area(nameof(Admin))]
    [Authorize(Roles ="Admin")]
    public class UsersController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public UsersController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task<IActionResult> Index()
        {
            List<UserVM> userVMs = new List<UserVM>();
            List<AppUser> users = await _userManager.Users.ToListAsync();
            foreach (AppUser user in users)
            {
                UserVM userVM = new UserVM
                {
                    Id = user.Id,
                    Name = user.Name,
                    Surname = user.Surname,
                    Email = user.Email,
                    Username = user.UserName,
                    IsDeactive = user.IsDeactive,
                    Role = (await _userManager.GetRolesAsync(user)).FirstOrDefault()
                   
                };
                userVMs.Add(userVM);
            }
            return View(userVMs);
        }
        public IActionResult Create()
        {
            List<string> roles = new List<string>();
            roles.Add(Helper.Helper.Roles.Admin.ToString());
            roles.Add(Helper.Helper.Roles.Member.ToString());
            ViewBag.Roles = roles;
            return View();
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Create(CreateUserVM register, string role)
        {
            List<string> roles = new List<string>();
            roles.Add(Helper.Helper.Roles.Admin.ToString());
            roles.Add(Helper.Helper.Roles.Member.ToString());
            ViewBag.Roles = roles;

            if (!ModelState.IsValid)
            {
                return View();
            }
            AppUser appUser = await _userManager.FindByEmailAsync(register.Email);
            if (appUser != null)
            {
                ModelState.AddModelError("Email", " This is Email is already registered ");
                return View(register);
            }
            AppUser newuser = new AppUser
            {
                UserName = register.Username,
                Name = register.Name,
                Surname = register.Surname,
                Email = register.Email
            };
            IdentityResult identityResult = await _userManager.CreateAsync(newuser, register.Password);
            if (!identityResult.Succeeded)
            {
                foreach (IdentityError error in identityResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(register);
            }
            
            await _userManager.AddToRoleAsync(newuser, role);
            return RedirectToAction("Index");
        }
    }
}
