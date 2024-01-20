using Edukatee.Helpers;
using Edukatee.Models;
using Edukatee.ViewModels.AuthVm;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Edukatee.Controllers
{
	public class AuthController : Controller
	{
		SignInManager<AppUser> _sigInManager { get; set; }
		UserManager<AppUser> _userManager { get; set; }
		RoleManager<IdentityRole> _roleManager { get; set; }
		public AuthController(SignInManager<AppUser> sigInManager, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
		{
			_sigInManager = sigInManager;
			_userManager = userManager;
			_roleManager = roleManager;
		}

		
		public IActionResult Register()
		{
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> Register(RegisterVm vm)
		{
			if (!ModelState.IsValid)
			{
				return View();
			}
			var user = new AppUser()
			{
				Name = vm.Name,
				Surname = vm.Surname,
				Email = vm.Email,
				UserName = vm.Username
			};
			var result = await _userManager.CreateAsync(user, vm.Password);
			if (!result.Succeeded)
			{
                foreach (var error in result.Errors)
                {
					ModelState.AddModelError("", error.Description);
					return View(vm);
                }
            }
			//View Bag registration
			ViewBag.RegistrationSuccess = true;
            return View();
		}
		public async Task<IActionResult> Login()
		{
			return View();
		}
		[HttpPost]
		public async Task<IActionResult>Login(string? returnUrl,LoginVm vm)
		{
			AppUser user;
			if (!ModelState.IsValid)
			{
				return View();
			}
			if (vm.UsernameOrEmail.Contains("@"))
			{
				user= await _userManager.FindByEmailAsync(vm.UsernameOrEmail);
			}
			else
			{
				user= await _userManager.FindByNameAsync(vm.UsernameOrEmail);
			}
            if (user!=null)
            {
				var result = await _sigInManager.PasswordSignInAsync(user, vm.Password, vm.IsRemember, true);
				if (!result.Succeeded)
				{
					ModelState.AddModelError("", "Invalid Login attempt");
					return View();
				}
            }
            else
            {
                ModelState.AddModelError("", "User Not Found");
                return View();
            }
            if (returnUrl != null)
			{
				return LocalRedirect(returnUrl);
			}
			return RedirectToAction("Index", "Home");
        }
		public async Task<IActionResult> Logout()
		{
			await _sigInManager.SignOutAsync();
			return RedirectToAction("Index", "Home");
		}
		public async Task<bool> CreateRoles()
		{
            foreach (var item in Enum.GetValues(typeof(Roles)))
            {
                if(!await _roleManager.RoleExistsAsync(item.ToString()))
				{
					await _roleManager.CreateAsync(new IdentityRole
					{
						Name = item.ToString(),
					});
				}
				return true;
            }
			return false;
        }
		
		public async Task<IActionResult> InitRoles()
		{
			if (HttpContext.User != null)
				if (!await _roleManager.RoleExistsAsync("Admin") != true)
					await _roleManager.CreateAsync(new IdentityRole("Admin"));
			if (!await _roleManager.RoleExistsAsync("Member") != true)
				await _roleManager.CreateAsync(new IdentityRole("Member"));
			if (await _userManager.FindByEmailAsync("admin@mail.ru") == null)
			{
				var user = new AppUser
				{
					Name = "Admin",
					Surname = "Adminov",
					Email = "admin@mail.ru",
					UserName = "admin123"
				};
				var result = await _userManager.CreateAsync(user, "Admin123!");
				await _userManager.AddToRoleAsync(user, "Admin");
			}
			return RedirectToAction(nameof(Login));
		}
	}
}
