
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BolumKurulu.Web.Models;

public class KurulUyesiEkleViewModel
{
    [Required]
    public int KurulId { get; set; }

    public string KurulAdi { get; set; } = string.Empty;

    [Required(ErrorMessage = "Öğretim elemanı seçiniz.")]
    [Range(1, int.MaxValue,
        ErrorMessage = "Geçerli bir öğretim elemanı seçiniz.")]
    public int OgretimElemaniId { get; set; }

    [Required(ErrorMessage = "Görev seçiniz.")]
    public string Gorev { get; set; } = "Üye";

    [Required]
    [DataType(DataType.Date)]
    public DateTime BaslangicTarihi { get; set; }
        = DateTime.Today;

    [DataType(DataType.Date)]
    public DateTime? BitisTarihi { get; set; }

    public List<SelectListItem> OgretimElemanlari { get; set; }
        = new();
}
