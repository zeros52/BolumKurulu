
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

    public DbSet<OgretimElemani> OgretimElemanlari
    { get; set; }

    public DbSet<Kurul> Kurullar
    { get; set; }

    public DbSet<KurulUyesi> KurulUyeleri
    { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<OgretimElemani>(e =>
        {
            e.HasIndex(x => x.TcNo).IsUnique();
            e.HasIndex(x => x.SicilNo).IsUnique();
        });

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
    }
}
