using System.ComponentModel.DataAnnotations;

namespace BolumKurulu.Web.Models;

public enum KararDurumu
{
    Taslak = 0,
    Oylamada = 1,
    Imzada = 2,
    Arsivlendi = 3
}

public class KararYazisi
{
    public int Id { get; set; }

    [Display(Name = "Yıl")]
    public int Yil { get; set; }

    [Display(Name = "Sayı")]
    public int Sayi { get; set; }

    [Display(Name = "Tarih")]
    [DataType(DataType.Date)]
    public DateTime Tarih { get; set; } = DateTime.Today;

    [Display(Name = "Durum")]
    public KararDurumu Durum { get; set; } = KararDurumu.Taslak;

    [Display(Name = "İlgi")]
    [StringLength(500)]
    public string? Ilgi { get; set; }

    [Required, StringLength(1000)]
    [Display(Name = "Gündem")]
    public string Gundem { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Karar")]
    public string KararMetni { get; set; } = string.Empty;

    [Display(Name = "Karar No")]
    public string KararNo => $"{Yil}/{Sayi:00}";
}