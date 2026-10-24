
using BolumKurulu.Web.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BolumKurulu.Web.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<OgretimElemani> OgretimElemanlari { get; set; }

    public DbSet<Kurul> Kurullar { get; set; }

    public DbSet<KurulUyesi> KurulUyeleri { get; set; }

    public DbSet<KararYazisi> KararYazilari { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Öğretim elemanı benzersizlik kontrolleri
        builder.Entity<OgretimElemani>(e =>
        {
            e.HasIndex(x => x.TcNo).IsUnique();
            e.HasIndex(x => x.SicilNo).IsUnique();
        });

        // Kurul üyeliği ilişkileri
        builder.Entity<KurulUyesi>(e =>
        {
            e.HasOne(x => x.Kurul)
                .WithMany(x => x.Uyeler)
                .HasForeignKey(x => x.KurulId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.OgretimElemani)
                .WithMany()
                .HasForeignKey(x => x.OgretimElemaniId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasIndex(x => new
            {
                x.KurulId,
                x.OgretimElemaniId
            });
        });

        // Karar yazısı benzersizlik kontrolü
        builder.Entity<KararYazisi>(e =>
        {
            e.HasIndex(x => new
            {
                x.Yil,
                x.Sayi
            }).IsUnique();
        });
    }
}
