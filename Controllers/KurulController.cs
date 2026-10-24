
using BolumKurulu.Web.Data;
using BolumKurulu.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BolumKurulu.Web.Controllers;

[Authorize(Roles = "Admin")]
public class KurulController : Controller
{
    private readonly ApplicationDbContext _context;

    public KurulController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var kurullar = await _context.Kurullar
            .AsNoTracking()
            .OrderBy(x => x.KurulAdi)
            .ToListAsync();

        return View(kurullar);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind("KurulAdi,Aciklama,Aktif")] Kurul kurul)
    {
        if (!ModelState.IsValid)
            return View(kurul);

        kurul.KurulAdi = kurul.KurulAdi.Trim();

        if (string.IsNullOrWhiteSpace(kurul.KurulAdi))
        {
            ModelState.AddModelError(
                nameof(Kurul.KurulAdi),
                "Kurul adı boş bırakılamaz.");

            return View(kurul);
        }

        bool mevcut = await _context.Kurullar.AnyAsync(
            x => x.KurulAdi == kurul.KurulAdi);

        if (mevcut)
        {
            ModelState.AddModelError(
                nameof(Kurul.KurulAdi),
                "Bu isimde bir kurul zaten mevcut.");

            return View(kurul);
        }

        kurul.OlusturulmaTarihi = DateTime.UtcNow;

        _context.Kurullar.Add(kurul);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Kurul başarıyla oluşturuldu.";

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var kurul = await _context.Kurullar.FindAsync(id);

        if (kurul == null)
            return NotFound();

        return View(kurul);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        [Bind("Id,KurulAdi,Aciklama,Aktif")] Kurul form)
    {
        if (id != form.Id)
            return BadRequest();

        var kurul = await _context.Kurullar.FindAsync(id);

        if (kurul == null)
            return NotFound();

        if (!ModelState.IsValid)
            return View(form);

        form.KurulAdi = form.KurulAdi.Trim();

        if (string.IsNullOrWhiteSpace(form.KurulAdi))
        {
            ModelState.AddModelError(
                nameof(Kurul.KurulAdi),
                "Kurul adı boş bırakılamaz.");

            return View(form);
        }

        bool ayniIsimVar = await _context.Kurullar
            .AnyAsync(x =>
                x.Id != id &&
                x.KurulAdi == form.KurulAdi);

        if (ayniIsimVar)
        {
            ModelState.AddModelError(
                nameof(Kurul.KurulAdi),
                "Bu isimde başka bir kurul zaten mevcut.");

            return View(form);
        }

        kurul.KurulAdi = form.KurulAdi;
        kurul.Aciklama = form.Aciklama;
        kurul.Aktif = form.Aktif;

        await _context.SaveChangesAsync();

        TempData["Success"] = "Kurul başarıyla güncellendi.";

        return RedirectToAction(nameof(Index));
    }
}
