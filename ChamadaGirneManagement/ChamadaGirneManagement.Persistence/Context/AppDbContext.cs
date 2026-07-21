using ChamadaGirneManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChamadaGirneManagement.Persistence.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<YonetimKullanicisi> YonetimKullanicilari { get; set; }
    public DbSet<Uygulama> Uygulamalar { get; set; }
    public DbSet<UygulamaGecisKodu> UygulamaGecisKodlari { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // YonetimKullanicisi
        modelBuilder.Entity<YonetimKullanicisi>(entity =>
        {
            entity.ToTable("YonetimKullanicilari");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.KullaniciAdi)
                .IsRequired()
                .HasMaxLength(100);

            entity.HasIndex(x => x.KullaniciAdi)
                .IsUnique();

            entity.Property(x => x.SifreHash)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(x => x.AdSoyad)
                .IsRequired()
                .HasMaxLength(150);
            modelBuilder.Entity<YonetimKullanicisi>().HasData(
                new YonetimKullanicisi
                {
                    Id = 1,
                    KullaniciAdi = "IT",
                    SifreHash = "$2a$11$nB8kNi06IPQiG//LeYdNqe10O54oTT9NvQ5QASZ641yBgrbT9mZlq", // It!!2025
                    AdSoyad = "Sistem Yöneticisi"
                }
            );
        });

        // Uygulama
        modelBuilder.Entity<Uygulama>(entity =>
        {
            entity.ToTable("Uygulamalar");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.UygulamaAdi)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(x => x.UygulamaKodu)
                .IsRequired()
                .HasMaxLength(100);

            entity.HasIndex(x => x.UygulamaKodu)
                .IsUnique();

            entity.Property(x => x.GecisYolu)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(x => x.IstemciAnahtarHash)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(x => x.HedefKullaniciAdi)
    .IsRequired()
    .HasMaxLength(100);
        });

        // UygulamaGecisKodu
        modelBuilder.Entity<UygulamaGecisKodu>(entity =>
        {
            entity.ToTable("UygulamaGecisKodlari");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.KodHash)
                .IsRequired()
                .HasMaxLength(128);

            entity.HasIndex(x => x.KodHash)
                .IsUnique();

            entity.Property(x => x.YonetimDonusAdresi)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(x => x.KullanildiMi)
                .HasDefaultValue(false);

            entity.HasOne(x => x.YonetimKullanicisi)
                .WithMany()
                .HasForeignKey(x => x.YonetimKullaniciId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.Uygulama)
                .WithMany()
                .HasForeignKey(x => x.UygulamaId)
                .OnDelete(DeleteBehavior.Cascade);
        });


    }
}