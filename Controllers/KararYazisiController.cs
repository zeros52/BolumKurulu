using BolumKurulu.Web.Data;
using BolumKurulu.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BolumKurulu.Web.Controllers;

[Authorize(Roles = "Admin,Goruntuleyici")]
public class KararYazisiController : Controller
{
    private readonly ApplicationDbContext _context;

    public KararYazisiController(ApplicationDbContext context) => _context = context;

    public async Task<IActionResult> Index()
    {
        var list = await _context.KararYazilari
            .OrderByDescending(k => k.Yil)
            .ThenByDescending(k => k.Sayi)
            .ToListAsync();
        return View(list);
    }

    public async Task<IActionResult> Details(int id)
    {
        var model = await _context.KararYazilari.FindAsync(id);
        return model == null ? NotFound() : View(model);
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Create() => View(new KararYazisi { Tarih = DateTime.Today });

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([Bind("Tarih,Ilgi,Gundem,KararMetni")] KararYazisi model)
    {
        if (!ModelState.IsValid) return View(model);

        var yil = model.Tarih.Year;

        // Sayı yıl bazında otomatik artar; çakışma olursa birkaç kez yeniden dener
        for (var deneme = 0; deneme < 3; deneme++)
        {
            var son = await _context.KararYazilari
                .Where(k => k.Yil == yil)
                .MaxAsync(k => (int?)k.Sayi) ?? 0;

            model.Yil = yil;
            model.Sayi = son + 1;
            model.Durum = KararDurumu.Taslak;

            _context.Add(model);
            try
            {
                await _context.SaveChangesAsync();
                TempData["Mesaj"] = $"Karar {model.KararNo} oluşturuldu.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                _context.Entry(model).State = EntityState.Detached;
            }
        }

        ModelState.AddModelError(string.Empty, "Numara çakışması oldu, lütfen tekrar deneyin.");
        return View(model);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id)
    {
        var model = await _context.KararYazilari.FindAsync(id);
        if (model == null) return NotFound();

        if (model.Durum != KararDurumu.Taslak)
        {
            TempData["Hata"] = "Yalnızca taslak durumundaki kararlar düzenlenebilir.";
            return RedirectToAction(nameof(Index));
        }
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Ilgi,Gundem,KararMetni")] KararYazisi form)
    {
        if (id != form.Id) return NotFound();

        var model = await _context.KararYazilari.FindAsync(id);
        if (model == null) return NotFound();

        if (model.Durum != KararDurumu.Taslak)
        {
            TempData["Hata"] = "Yalnızca taslak durumundaki kararlar düzenlenebilir.";
            return RedirectToAction(nameof(Index));
        }

        if (!ModelState.IsValid)
        {
            form.Yil = model.Yil;
            form.Sayi = model.Sayi;
            form.Tarih = model.Tarih;
            return View(form);
        }

        // Yıl, sayı ve tarih değişmez; yalnızca içerik güncellenir
        model.Ilgi = form.Ilgi;
        model.Gundem = form.Gundem;
        model.KararMetni = form.KararMetni;

        await _context.SaveChangesAsync();
        TempData["Mesaj"] = $"Karar {model.KararNo} güncellendi.";
        return RedirectToAction(nameof(Index));
    }
}