using Fiorello.Models;
using Fiorello.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fiorello.Helper;

namespace Fiorello.Controllers
{
    public class AccountController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        public AccountController(   RoleManager<IdentityRole> roleManager,
                                     UserManager<AppUser> userManager,
                                     SignInManager<AppUser> signInManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            AppUser appUser;
            appUser= await _userManager.FindByNameAsync(loginVM.Username) ;

            if (appUser==null)
            {
                appUser = await _userManager.FindByEmailAsync(loginVM.Username);
                if (appUser==null)
                {
                    ModelState.AddModelError("", "Username,Email or password is wrong");
                    return View();
                }
            }

          Microsoft.AspNetCore.Identity.SignInResult signInResult= await _signInManager.PasswordSignInAsync(appUser, loginVM.Password, true, true);
           
            if (signInResult.IsLockedOut)
            {
                ModelState.AddModelError("", " This account has been blocked (3 deq gozleyin)");
                return View();
            }
            if (!signInResult.Succeeded)
            {
                ModelState.AddModelError("", " The Username or Password is incorrect ");
                return View();
            }
            return RedirectToAction("Index", "Home");
        } 
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Register(RegisterVM register )
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            
            AppUser appUser = await _userManager.FindByEmailAsync(register.Email);
           
           
            if (appUser !=null)
            {
                ModelState.AddModelError("Email", " This is Email is already registered ");
                return View(register);
            }
            AppUser newuser = new AppUser
            {
                    UserName=register.Username,
                    Name=register.Name,
                    Surname=register.Surname,
                    Email= register.Email 
            };
           IdentityResult identityResult= await _userManager.CreateAsync(newuser, register.Password);
            if (!identityResult.Succeeded)
            {
                foreach (IdentityError error in identityResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(register);
            }
            await _signInManager.SignInAsync(newuser, true);
            await _userManager.AddToRoleAsync(newuser,Helper.Helper.Roles.Member.ToString());
            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> LogoutAsync()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        //Member ve Admin Role yaratmaq ucun code.Bir defe istifade olunur
        //public async Task CreateRoles()
        //{
        //    if (!await _roleManager.RoleExistsAsync(Helper.Helper.Roles.Admin.ToString()))
        //    {
        //        await _roleManager.CreateAsync(new IdentityRole { Name = Helper.Helper.Roles.Admin.ToString() });
        //    };
        //    if (!await _roleManager.RoleExistsAsync(Helper.Helper.Roles.Member.ToString()))
        //    {
        //        await _roleManager.CreateAsync(new IdentityRole { Name = Helper.Helper.Roles.Member.ToString() });
        //    };
        //}

    }
}
