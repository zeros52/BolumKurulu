using System.ComponentModel.DataAnnotations;
using BolumKurulu.Web.Validation;

namespace BolumKurulu.Web.Models;

public class OgretimElemani
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    [Display(Name = "Ad Soyad")]
    public string AdSoyad { get; set; } = string.Empty;

    [Required, StringLength(11, MinimumLength = 11), TcKimlikNo]
    [Display(Name = "T.C. Kimlik No")]
    public string TcNo { get; set; } = string.Empty;

    [Required, StringLength(20)]
    [Display(Name = "Sicil No")]
    public string SicilNo { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(150)]
    [Display(Name = "E-posta")]
    public string Email { get; set; } = string.Empty;

    public bool Aktif { get; set; } = true;
}