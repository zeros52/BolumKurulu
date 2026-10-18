using BolumKurulu.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BolumKurulu.Web.Controllers;

[Authorize(Roles = "Admin")]
public class KullaniciController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private static readonly string[] Roller = { "Admin", "Goruntuleyici" };

    public KullaniciController(UserManager<IdentityUser> userManager) => _userManager = userManager;

    public async Task<IActionResult> Index()
    {
        var users = await _userManager.Users.OrderBy(u => u.Email).ToListAsync();
        var list = new List<KullaniciViewModel>();

        foreach (var u in users)
        {
            var roles = await _userManager.GetRolesAsync(u);
            list.Add(new KullaniciViewModel
            {
                Id = u.Id,
                Email = u.Email ?? "",
                Rol = roles.FirstOrDefault() ?? "Rol yok"
            });
        }

        ViewBag.Roller = Roller;
        ViewBag.MevcutKullaniciId = _userManager.GetUserId(User);
        return View(list);
    }

    public IActionResult Create()
    {
        ViewBag.Roller = Roller;
        return View(new KullaniciOlusturViewModel());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(KullaniciOlusturViewModel model)
    {
        ViewBag.Roller = Roller;

        if (!Roller.Contains(model.Rol))
            ModelState.AddModelError(nameof(model.Rol), "Geçersiz rol.");

        if (!ModelState.IsValid) return View(model);

        var user = new IdentityUser { UserName = model.Email, Email = model.Email, EmailConfirmed = true };
        var result = await _userManager.CreateAsync(user, model.Password);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);
            return View(model);
        }

        await _userManager.AddToRoleAsync(user, model.Rol);
        TempData["Mesaj"] = $"{model.Email} oluşturuldu.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> RolDegistir(string id, string rol)
    {
        if (!Roller.Contains(rol)) return BadRequest();

        if (id == _userManager.GetUserId(User))
        {
            TempData["Hata"] = "Kendi rolünüzü değiştiremezsiniz.";
            return RedirectToAction(nameof(Index));
        }

        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        var mevcut = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, mevcut);
        await _userManager.AddToRoleAsync(user, rol);

        TempData["Mesaj"] = $"{user.Email} kullanıcısının rolü {rol} olarak güncellendi.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Sil(string id)
    {
        if (id == _userManager.GetUserId(User))
        {
            TempData["Hata"] = "Kendi hesabınızı silemezsiniz.";
            return RedirectToAction(nameof(Index));
        }

        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        await _userManager.DeleteAsync(user);
        TempData["Mesaj"] = $"{user.Email} silindi.";
        return RedirectToAction(nameof(Index));
    }
}