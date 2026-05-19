using System;
using System.Collections.Generic;

namespace AtauniLost.Models;

public partial class Ilanlar
{
    public int IlanId { get; set; }

    public string? Baslik { get; set; }

    public string? FotografYolu { get; set; }

    public string? Aciklama { get; set; }

    public string? Kategori { get; set; }

    public string? Durum { get; set; }

    public string? Konum { get; set; }

    public DateTime? Tarih { get; set; }

    public bool? OnayDurumu { get; set; }

    public int? KullaniciId { get; set; }

    public virtual Kullanicilar? Kullanici { get; set; }

    public virtual ICollection<Mesajlar> Mesajlars { get; set; } = new List<Mesajlar>();

    public virtual ICollection<Yorumlar> Yorumlars { get; set; } = new List<Yorumlar>();
}
