using _11_Identity.Models;
using _11_Identity.Models.DTOs;
using _11_Identity.Models.VMs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace _11_Identity.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly Context _context;
        private readonly IPasswordHasher<AppUser> _passwordHasher;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, Context context = null, IPasswordHasher<AppUser> passwordHasher = null)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _passwordHasher = passwordHasher;
        }

        public IActionResult Register() // kayıt islemi
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(UserDTO dto)
        {
            if (ModelState.IsValid)
            {
                AppUser appUser = new AppUser()
                {
                    UserName = dto.UserName,
                    Email = dto.Email
                };
                IdentityResult result = await _userManager.CreateAsync(appUser, dto.Password);

                if (result.Succeeded) return RedirectToAction("Login"); // login sayfasına gider
                else
                {
                    ModelState.AddModelError("Hata", "Kayıt basarısız!");
                }
            }
            return View(dto);
        }
        // get
        public IActionResult Login(string returnUrl) // kullanıcı adı, sifre, gitmek istedigi yer (url)
        {
            // gitmek istedigi bir x controllerinin/y action, yoksa? bırak anasayfa(home/index)

            returnUrl = returnUrl is null ? "~/Home/Index" : returnUrl;

            return View(new UserVM()
            {
                ReturnUrl = returnUrl
            });
        }

        [HttpPost]
        public async Task<IActionResult> Login(UserVM vm)
        {
            if (ModelState.IsValid)
            {
                AppUser user = await _userManager.FindByNameAsync(vm.UserName);
                if (user != null)
                {
                    await _signInManager.SignOutAsync();
                    SignInResult result = await _signInManager.PasswordSignInAsync(user, vm.Password, false, false);

                    if (result.Succeeded)
                    {
                        return Redirect(vm.ReturnUrl); // string bir url ister. (gidilecek yeri yazmanı bekler.)
                    }
                    ModelState.AddModelError("HATA", "Kimlik dogrulama basarısız!");

                }
            }
            return View(vm);
        }
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }

        [Authorize] // Yetki
        public async Task<IActionResult> List()
        {

            return View(_userManager.Users.ToList());
        }

        public async Task<IActionResult> Update(string id)
        {
            AppUser appUser = await _userManager.FindByIdAsync(id);
            if (appUser != null) // DTO --> view'a
            {
                UserUpdateDTO dto = new UserUpdateDTO()
                {
                    UserName = appUser.UserName,
                    Email = appUser.Email,
                    ID = appUser.Id
                };
                return View(dto);
            }
            return RedirectToAction(nameof(List));
        }

        [HttpPost]
        public async Task<IActionResult> Update(UserUpdateDTO dto)
        {
            if (ModelState.IsValid)
            {
                AppUser appUser = await _userManager.FindByIdAsync(dto.ID);
                appUser.UserName = dto.UserName;
                appUser.Email = dto.Email;
                // yeni sifre yazıldıysa degistir, yazılmadıysa aynı kalsın.
                if (!string.IsNullOrEmpty(dto.Password))
                {
                    appUser.PasswordHash = _passwordHasher.HashPassword(appUser, dto.Password);
                }
                IdentityResult result = await _userManager.UpdateAsync(appUser);
                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(List));
                }
                else
                {
                    string message = "";
                    foreach (var item in result.Errors)
                    {
                        message += $"" +
                           $"Hata Kodu : {item.Code} - Açıklama : {item.Description}";
                    }
                    TempData["mesaj"] = message;
                }
            }
            return View(dto);
        }

        public async Task<IActionResult> Delete(string id)
        {
            AppUser appUser = await _userManager.FindByIdAsync(id);

            if (appUser != null)
            {
                IdentityResult result = await _userManager.DeleteAsync(appUser);

                if (result.Succeeded)
                {
                    TempData["mesaj"] = "kullanıcı silme islemi basarili";
                }
                else TempData["mesaj"] = "kullanıcı silme islemi basarisiz";
                return RedirectToAction(nameof(List));
            }
            return RedirectToAction(nameof(List));
        }

    }
}
