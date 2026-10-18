using BolumKurulu.Web.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BolumKurulu.Web.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<OgretimElemani> OgretimElemanlari { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<OgretimElemani>(e =>
        {
            e.HasIndex(x => x.TcNo).IsUnique();
            e.HasIndex(x => x.SicilNo).IsUnique();
        });
    }
}