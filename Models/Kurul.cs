
using System.ComponentModel.DataAnnotations;

namespace BolumKurulu.Web.Models;

public class Kurul
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Kurul adı zorunludur.")]
    [StringLength(150)]
    [Display(Name = "Kurul Adı")]
    public string KurulAdi { get; set; } = string.Empty;

    [StringLength(500)]
    [Display(Name = "Açıklama")]
    public string? Aciklama { get; set; }

    [Display(Name = "Aktif")]
    public bool Aktif { get; set; } = true;

    public DateTime OlusturulmaTarihi { get; set; } = DateTime.Now;

    public ICollection<KurulUyesi> Uyeler { get; set; }
        = new List<KurulUyesi>();
}
