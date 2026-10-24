
using System.ComponentModel.DataAnnotations;

namespace BolumKurulu.Web.Models;

public class KurulUyesiDuzenleViewModel
{
    public int Id { get; set; }

    public int KurulId { get; set; }

    public string KurulAdi { get; set; } = string.Empty;

    public string OgretimElemaniAdi { get; set; }
        = string.Empty;

    [Required(ErrorMessage = "Görev seçiniz.")]
    [Display(Name = "Kuruldaki Görevi")]
    public string Gorev { get; set; } = "Üye";

    [Required(ErrorMessage = "Başlangıç tarihi zorunludur.")]
    [DataType(DataType.Date)]
    [Display(Name = "Başlangıç Tarihi")]
    public DateTime BaslangicTarihi { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Bitiş Tarihi")]
    public DateTime? BitisTarihi { get; set; }

    [Display(Name = "Aktif")]
    public bool Aktif { get; set; }
}
