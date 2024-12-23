using _11_Identity.Models;
using _11_Identity.Models.DTOs;
using _11_Identity.Models.VMs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace _11_Identity.Controllers
{
    [Authorize]
    public class RoleController : Controller
    {
        private readonly RoleManager<AppRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;
        public RoleController(RoleManager<AppRole> roleManager, UserManager<AppUser> userManager)
        {
            this._roleManager = roleManager;
            _userManager = userManager;
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(RoleCreateDTO dto)
        {
            if (ModelState.IsValid)
            {
                IdentityResult result = await _roleManager.CreateAsync(new AppRole()
                {
                    Name = dto.Name,
                    OlusmaTarihi = dto.OlusmaTarihi
                });

                if (result.Succeeded) return RedirectToAction("List");
            }
            return View(dto);
        }

        // amacım okumak oldugu icin httpget ile alırım
        public IActionResult List()
        {
            return View(_roleManager.Roles.ToList());
        }

        public async Task<IActionResult> Update(string id) // role id
        {
            AppRole appRole = await _roleManager.FindByIdAsync(id);

            if (appRole != null)
            {
                RoleUpdateDTO dto = new RoleUpdateDTO()
                {
                    ID = appRole.Id,
                    Name = appRole.Name
                };
                return View(dto);
            }
            return RedirectToAction(nameof(List));
        }

        // uzerinde islem yaptıgımız icin post metodu ile alırım.
        [HttpPost]
        public async Task<IActionResult> Update(RoleUpdateDTO dto)
        {
            if (ModelState.IsValid)
            {
                AppRole appRole = await _roleManager.FindByIdAsync(dto.ID);

                appRole.Id = dto.ID;
                appRole.Name = dto.Name;

                IdentityResult result = await _roleManager.UpdateAsync(appRole);

                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(List));
                }
                else
                {
                    TempData["mesaj"] = "kullanıcı guncelleme islemi basarisiz";
                }
            }
            return View(dto);
        }

        // kullanıcıyı delete view sayfasına goturmedigi icin post kullanmayız.
        // cat diye silmek istedigimiz icin get metodu ile yazarız.
        public async Task<IActionResult> Delete(string id)
        {
            AppRole appRole = await _roleManager.FindByIdAsync(id);

            if (appRole != null)
            {
                IdentityResult result = await _roleManager.DeleteAsync(appRole);

                if (result.Succeeded)
                {
                    TempData["mesaj"] = "kullanıcı silme islemi basarili";
                }
                else
                {
                    TempData["mesaj"] = "kullanıcı silme islemi basarisiz";
                    return RedirectToAction(nameof(List));
                }
            }
            return RedirectToAction(nameof(List));
        }

        public async Task<IActionResult> Assign(string id) // kisi id
        {
            AppUser appUser = await _userManager.FindByIdAsync(id); // user'ı buldum

            List<AppRole> allRoles = _roleManager.Roles.ToList(); // tum roller

            List<string> userRoles = await _userManager.GetRolesAsync(appUser) as List<string>; // user rolleri

            List<RoleVM> rolles = new List<RoleVM>(); // view'a herkes bu sekilde gidecek.
            allRoles.ForEach(a => rolles.Add(
                new RoleVM()
                {
                    RoleId = a.Id,
                    RoleName = a.Name,
                    HasAssign = userRoles.Contains(a.Name)
                }));
            return View(rolles);
        }

        [HttpPost]
        public async Task<IActionResult> Assign(string id, List<RoleVM> list)
        {
            AppUser appUser = await _userManager.FindByIdAsync(id); // user

            if (appUser != null)
            {
                var currentRole = await _userManager.GetRolesAsync(appUser);

                foreach (var item in list)
                {
                    if (item.HasAssign && !currentRole.Contains(item.RoleName))
                    {
                        await _userManager.AddToRoleAsync(appUser, item.RoleName);
                    }
                    else if (!item.HasAssign && currentRole.Contains(item.RoleName))
                    {
                        await _userManager.RemoveFromRoleAsync(appUser, item.RoleName);
                    }
                }
            }
            return RedirectToAction("List", "Account");
        }
    }
}
