
using System.ComponentModel.DataAnnotations;

namespace BolumKurulu.Web.Models;

public class KurulUyesi
{
    public int Id { get; set; }

    [Required]
    public int KurulId { get; set; }

    public Kurul Kurul { get; set; } = null!;

    [Required]
    public int OgretimElemaniId { get; set; }

    public OgretimElemani OgretimElemani { get; set; } = null!;

    [Required]
    [StringLength(50)]
    [Display(Name = "Kuruldaki Görevi")]
    public string Gorev { get; set; } = "Üye";

    [Display(Name = "Başlangıç Tarihi")]
    [DataType(DataType.Date)]
    public DateTime BaslangicTarihi { get; set; } = DateTime.Today;

    [Display(Name = "Bitiş Tarihi")]
    [DataType(DataType.Date)]
    public DateTime? BitisTarihi { get; set; }

    [Display(Name = "Aktif")]
    public bool Aktif { get; set; } = true;
}
