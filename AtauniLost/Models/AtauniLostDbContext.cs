using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace AtauniLost.Models;

public partial class AtauniLostDbContext : DbContext
{
    public AtauniLostDbContext()
    {
    }

    public AtauniLostDbContext(DbContextOptions<AtauniLostDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Ilanlar> Ilanlars { get; set; }
    public virtual DbSet<Kullanicilar> Kullanicilars { get; set; }
    public virtual DbSet<Mesajlar> Mesajlars { get; set; }
    public virtual DbSet<Yorumlar> Yorumlars { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            // Bağlantı hatasını çözmek için adresi buraya doğrudan ekledik
            optionsBuilder.UseSqlServer("Server=.;Database=AtauniLostDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Ilanlar>(entity =>
        {
            entity.HasKey(e => e.IlanId).HasName("PK__Ilanlar__3F0793719EC83F74");
            entity.ToTable("Ilanlar");
            entity.Property(e => e.Baslik).HasMaxLength(100);
            entity.Property(e => e.Durum).HasMaxLength(20);
            entity.Property(e => e.Kategori).HasMaxLength(50);
            entity.Property(e => e.Konum).HasMaxLength(100);
            entity.Property(e => e.OnayDurumu).HasDefaultValue(false);
            entity.Property(e => e.Tarih)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Kullanici).WithMany(p => p.Ilanlars)
                .HasForeignKey(d => d.KullaniciId)
                .HasConstraintName("FK__Ilanlar__Kullani__3E52440B");
        });

        modelBuilder.Entity<Kullanicilar>(entity =>
        {
            entity.HasKey(e => e.KullaniciId).HasName("PK__Kullanic__E011F77B9C936AFA");
            entity.ToTable("Kullanicilar");
            entity.HasIndex(e => e.Email, "UQ__Kullanic__A9D1053469862F8C").IsUnique();
            entity.Property(e => e.AdSoyad).HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Rol)
                .HasMaxLength(20)
                .HasDefaultValue("Kullanici");
            entity.Property(e => e.Sifre).HasMaxLength(50);
        });

        modelBuilder.Entity<Mesajlar>(entity =>
        {
            entity.HasKey(e => e.MesajId).HasName("PK__Mesajlar__0CB8DCA070222DD6");
            entity.ToTable("Mesajlar");
            entity.Property(e => e.OkunduMu).HasDefaultValue(false);
            entity.Property(e => e.Tarih)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Alici).WithMany(p => p.MesajlarAlicis)
                .HasForeignKey(d => d.AliciId)
                .HasConstraintName("FK__Mesajlar__AliciI__46E78A0C");

            entity.HasOne(d => d.Gonderen).WithMany(p => p.MesajlarGonderens)
                .HasForeignKey(d => d.GonderenId)
                .HasConstraintName("FK__Mesajlar__Gonder__45F365D3");

            entity.HasOne(d => d.Ilan).WithMany(p => p.Mesajlars)
                .HasForeignKey(d => d.IlanId)
                .HasConstraintName("FK__Mesajlar__IlanId__47DBAE45");
        });

        modelBuilder.Entity<Yorumlar>(entity =>
        {
            entity.HasKey(e => e.YorumId).HasName("PK__Yorumlar__F2BE14E85CC11587");
            entity.ToTable("Yorumlar");
            entity.Property(e => e.Icerik).HasMaxLength(500);
            entity.Property(e => e.Tarih)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Ilan).WithMany(p => p.Yorumlars)
                .HasForeignKey(d => d.IlanId)
                .HasConstraintName("FK__Yorumlar__IlanId__412EB0B6");

            entity.HasOne(d => d.Kullanici).WithMany(p => p.Yorumlars)
                .HasForeignKey(d => d.KullaniciId)
                .HasConstraintName("FK__Yorumlar__Kullan__4222D4EF");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}