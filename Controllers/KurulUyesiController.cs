
using BolumKurulu.Web.Data;
using BolumKurulu.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BolumKurulu.Web.Controllers;

[Authorize(Roles = "Admin")]
public class KurulUyesiController : Controller
{
    private readonly ApplicationDbContext _context;

    public KurulUyesiController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Index(int kurulId)
    {
        var kurul = await _context.Kurullar
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == kurulId);

        if (kurul == null)
            return NotFound();

        var uyeler = await _context.KurulUyeleri
            .AsNoTracking()
            .Include(x => x.OgretimElemani)
            .Where(x => x.KurulId == kurulId)
            .OrderBy(x => x.OgretimElemani.AdSoyad)
            .ToListAsync();

        ViewBag.KurulId = kurul.Id;
        ViewBag.KurulAdi = kurul.KurulAdi;
        ViewBag.KurulAktif = kurul.Aktif;

        return View(uyeler);
    }

    [HttpGet]
    public async Task<IActionResult> Create(int kurulId)
    {
        var kurul = await _context.Kurullar
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == kurulId);

        if (kurul == null)
            return NotFound();

        if (!kurul.Aktif)
        {
            TempData["Error"] =
                "Pasif kurullara yeni üye atanamaz.";

            return RedirectToAction(
                nameof(Index),
                new { kurulId });
        }

        var model = new KurulUyesiEkleViewModel
        {
            KurulId = kurul.Id,
            KurulAdi = kurul.KurulAdi,
            BaslangicTarihi = DateTime.Today
        };

        await OgretimElemanlariniYukle(model);

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        KurulUyesiEkleViewModel model)
    {
        var kurul = await _context.Kurullar
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == model.KurulId);

        if (kurul == null)
            return NotFound();

        if (!kurul.Aktif)
        {
            TempData["Error"] =
                "Pasif kurullara yeni üye atanamaz.";

            return RedirectToAction(
                nameof(Index),
                new { kurulId = kurul.Id });
        }

        model.KurulAdi = kurul.KurulAdi;

        if (model.Gorev != "Başkan" &&
            model.Gorev != "Üye")
        {
            ModelState.AddModelError(
                nameof(model.Gorev),
                "Geçerli bir görev seçiniz.");
        }

        if (model.BaslangicTarihi == default)
        {
            ModelState.AddModelError(
                nameof(model.BaslangicTarihi),
                "Geçerli bir başlangıç tarihi giriniz.");
        }

        if (model.BitisTarihi.HasValue &&
            model.BitisTarihi.Value.Date <
            model.BaslangicTarihi.Date)
        {
            ModelState.AddModelError(
                nameof(model.BitisTarihi),
                "Bitiş tarihi başlangıçtan önce olamaz.");
        }

        bool personelAktif = await _context.OgretimElemanlari
            .AsNoTracking()
            .AnyAsync(x =>
                x.Id == model.OgretimElemaniId &&
                x.Aktif);

        if (!personelAktif)
        {
            ModelState.AddModelError(
                nameof(model.OgretimElemaniId),
                "Aktif bir öğretim elemanı seçiniz.");
        }

        bool cakisanUyelik = await _context.KurulUyeleri
            .AnyAsync(x =>
                x.Aktif &&
                x.KurulId == model.KurulId &&
                x.OgretimElemaniId == model.OgretimElemaniId &&
                x.BaslangicTarihi <=
                    (model.BitisTarihi ?? DateTime.MaxValue) &&
                (x.BitisTarihi ?? DateTime.MaxValue) >=
                    model.BaslangicTarihi);

        if (cakisanUyelik)
        {
            ModelState.AddModelError(
                nameof(model.OgretimElemaniId),
                "Bu öğretim elemanının aynı kurulda " +
                "tarihleri çakışan aktif üyeliği zaten var.");
        }

        if (model.Gorev == "Başkan")
        {
            bool baskanVar = await _context.KurulUyeleri
                .AnyAsync(x =>
                    x.Aktif &&
                    x.KurulId == model.KurulId &&
                    x.Gorev == "Başkan" &&
                    x.BaslangicTarihi <=
                        (model.BitisTarihi ?? DateTime.MaxValue) &&
                    (x.BitisTarihi ?? DateTime.MaxValue) >=
                        model.BaslangicTarihi);

            if (baskanVar)
            {
                ModelState.AddModelError(
                    nameof(model.Gorev),
                    "Belirtilen tarihlerde kurulun " +
                    "zaten aktif bir başkanı bulunuyor.");
            }
        }

        if (!ModelState.IsValid)
        {
            await OgretimElemanlariniYukle(model);
            return View(model);
        }

        var uye = new KurulUyesi
        {
            KurulId = model.KurulId,
            OgretimElemaniId = model.OgretimElemaniId,
            Gorev = model.Gorev,
            BaslangicTarihi = model.BaslangicTarihi.Date,
            BitisTarihi = model.BitisTarihi?.Date,
            Aktif = true
        };

        _context.KurulUyeleri.Add(uye);
        await _context.SaveChangesAsync();

        TempData["Success"] =
            "Öğretim elemanı kurula başarıyla atandı.";

        return RedirectToAction(
            nameof(Index),
            new { kurulId = model.KurulId });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var uye = await _context.KurulUyeleri
            .AsNoTracking()
            .Include(x => x.Kurul)
            .Include(x => x.OgretimElemani)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (uye == null)
            return NotFound();

        var model = new KurulUyesiDuzenleViewModel
        {
            Id = uye.Id,
            KurulId = uye.KurulId,
            KurulAdi = uye.Kurul.KurulAdi,
            OgretimElemaniAdi = uye.OgretimElemani.AdSoyad,
            Gorev = uye.Gorev,
            BaslangicTarihi = uye.BaslangicTarihi,
            BitisTarihi = uye.BitisTarihi,
            Aktif = uye.Aktif
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        KurulUyesiDuzenleViewModel model)
    {
        if (id != model.Id)
            return BadRequest();

        var uye = await _context.KurulUyeleri
            .Include(x => x.Kurul)
            .Include(x => x.OgretimElemani)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (uye == null)
            return NotFound();

        model.KurulId = uye.KurulId;
        model.KurulAdi = uye.Kurul.KurulAdi;
        model.OgretimElemaniAdi =
            uye.OgretimElemani.AdSoyad;

        if (model.Gorev != "Başkan" &&
            model.Gorev != "Üye")
        {
            ModelState.AddModelError(
                nameof(model.Gorev),
                "Geçerli bir görev seçiniz.");
        }

        if (model.BaslangicTarihi == default)
        {
            ModelState.AddModelError(
                nameof(model.BaslangicTarihi),
                "Geçerli bir başlangıç tarihi giriniz.");
        }

        if (model.BitisTarihi.HasValue &&
            model.BitisTarihi.Value.Date <
            model.BaslangicTarihi.Date)
        {
            ModelState.AddModelError(
                nameof(model.BitisTarihi),
                "Bitiş tarihi başlangıçtan önce olamaz.");
        }

        if (model.Aktif && !uye.Kurul.Aktif)
        {
            ModelState.AddModelError(
                nameof(model.Aktif),
                "Pasif kuruldaki üyelik aktifleştirilemez.");
        }

        if (model.Aktif && !uye.OgretimElemani.Aktif)
        {
            ModelState.AddModelError(
                nameof(model.Aktif),
                "Pasif öğretim elemanının üyeliği aktifleştirilemez.");
        }

        if (model.Aktif)
        {
            DateTime baslangic =
                model.BaslangicTarihi.Date;

            DateTime bitis =
                model.BitisTarihi?.Date ??
                DateTime.MaxValue;

            bool cakisma = await _context.KurulUyeleri
                .AnyAsync(x =>
                    x.Id != id &&
                    x.Aktif &&
                    x.KurulId == uye.KurulId &&
                    x.OgretimElemaniId ==
                        uye.OgretimElemaniId &&
                    x.BaslangicTarihi <= bitis &&
                    (x.BitisTarihi ?? DateTime.MaxValue)
                        >= baslangic);

            if (cakisma)
            {
                ModelState.AddModelError(
                    nameof(model.BaslangicTarihi),
                    "Bu öğretim elemanının aynı kurulda " +
                    "çakışan aktif üyeliği bulunuyor.");
            }

            if (model.Gorev == "Başkan")
            {
                bool baskanVar =
                    await _context.KurulUyeleri
                    .AnyAsync(x =>
                        x.Id != id &&
                        x.Aktif &&
                        x.KurulId == uye.KurulId &&
                        x.Gorev == "Başkan" &&
                        x.BaslangicTarihi <= bitis &&
                        (x.BitisTarihi ?? DateTime.MaxValue)
                            >= baslangic);

                if (baskanVar)
                {
                    ModelState.AddModelError(
                        nameof(model.Gorev),
                        "Belirtilen tarihlerde kurulda " +
                        "başka bir aktif başkan bulunuyor.");
                }
            }
        }

        if (!ModelState.IsValid)
            return View(model);

        uye.Gorev = model.Gorev;
        uye.BaslangicTarihi =
            model.BaslangicTarihi.Date;
        uye.BitisTarihi =
            model.BitisTarihi?.Date;
        uye.Aktif = model.Aktif;

        await _context.SaveChangesAsync();

        TempData["Success"] =
            "Kurul üyeliği başarıyla güncellendi.";

        return RedirectToAction(
            nameof(Index),
            new { kurulId = uye.KurulId });
    }

    [HttpGet]
    public async Task<IActionResult> Sonlandir(int id)
    {
        var uye = await _context.KurulUyeleri
            .AsNoTracking()
            .Include(x => x.Kurul)
            .Include(x => x.OgretimElemani)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (uye == null)
            return NotFound();

        if (!uye.Aktif)
        {
            TempData["Error"] =
                "Bu üyelik zaten pasif veya sonlandırılmış.";

            return RedirectToAction(
                nameof(Index),
                new { kurulId = uye.KurulId });
        }

        return View(uye);
    }

    [HttpPost]
    [ActionName("Sonlandir")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SonlandirOnay(int id)
    {
        var uye = await _context.KurulUyeleri
            .FirstOrDefaultAsync(x => x.Id == id);

        if (uye == null)
            return NotFound();

        if (!uye.Aktif)
        {
            TempData["Error"] =
                "Bu üyelik zaten pasif veya sonlandırılmış.";

            return RedirectToAction(
                nameof(Index),
                new { kurulId = uye.KurulId });
        }

        if (uye.BaslangicTarihi.Date > DateTime.Today)
        {
            TempData["Error"] =
                "Başlangıç tarihi gelecekte olan üyelik " +
                "bugünün tarihiyle sonlandırılamaz.";

            return RedirectToAction(
                nameof(Index),
                new { kurulId = uye.KurulId });
        }

        DateTime bugun = DateTime.Today;

        DateTime bitisTarihi =
            uye.BitisTarihi.HasValue &&
            uye.BitisTarihi.Value.Date < bugun
                ? uye.BitisTarihi.Value.Date
                : bugun;

        uye.BitisTarihi = bitisTarihi;
        uye.Aktif = false;

        await _context.SaveChangesAsync();

        TempData["Success"] =
            "Kurul üyeliği başarıyla sonlandırıldı.";

        return RedirectToAction(
            nameof(Index),
            new { kurulId = uye.KurulId });
    }

    private async Task OgretimElemanlariniYukle(
        KurulUyesiEkleViewModel model)
    {
        model.OgretimElemanlari =
            await _context.OgretimElemanlari
                .AsNoTracking()
                .Where(x => x.Aktif)
                .OrderBy(x => x.AdSoyad)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.AdSoyad + " (" + x.SicilNo + ")"
                })
                .ToListAsync();
    }
}
