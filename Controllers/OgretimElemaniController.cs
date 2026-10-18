using BolumKurulu.Web.Data;
using BolumKurulu.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BolumKurulu.Web.Controllers;

[Authorize(Roles = "Admin,Goruntuleyici")]
public class OgretimElemaniController : Controller
{
    private readonly ApplicationDbContext _context;

    public OgretimElemaniController(ApplicationDbContext context) => _context = context;

    public async Task<IActionResult> Index(string? q)
    {
        var query = _context.OgretimElemanlari.AsQueryable();
        if (!string.IsNullOrWhiteSpace(q))
            query = query.Where(x => x.AdSoyad.Contains(q) || x.SicilNo.Contains(q) || x.Email.Contains(q));

        ViewData["q"] = q;
        return View(await query.OrderBy(x => x.AdSoyad).ToListAsync());
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Create() => View(new OgretimElemani());

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([Bind("AdSoyad,TcNo,SicilNo,Email,Aktif")] OgretimElemani model)
    {
        await CheckDuplicates(model);
        if (!ModelState.IsValid) return View(model);

        _context.Add(model);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id)
    {
        var model = await _context.OgretimElemanlari.FindAsync(id);
        return model == null ? NotFound() : View(model);
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id, [Bind("Id,AdSoyad,TcNo,SicilNo,Email,Aktif")] OgretimElemani model)
    {
        if (id != model.Id) return NotFound();

        await CheckDuplicates(model);
        if (!ModelState.IsValid) return View(model);

        _context.Update(model);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var model = await _context.OgretimElemanlari.FindAsync(id);
        return model == null ? NotFound() : View(model);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken, Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var model = await _context.OgretimElemanlari.FindAsync(id);
        if (model != null)
        {
            _context.OgretimElemanlari.Remove(model);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    private async Task CheckDuplicates(OgretimElemani m)
    {
        if (await _context.OgretimElemanlari.AnyAsync(x => x.TcNo == m.TcNo && x.Id != m.Id))
            ModelState.AddModelError(nameof(m.TcNo), "Bu T.C. kimlik numarası zaten kayıtlı.");

        if (await _context.OgretimElemanlari.AnyAsync(x => x.SicilNo == m.SicilNo && x.Id != m.Id))
            ModelState.AddModelError(nameof(m.SicilNo), "Bu sicil numarası zaten kayıtlı.");
    }
}