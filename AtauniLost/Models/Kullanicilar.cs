using System;
using System.Collections.Generic;

namespace AtauniLost.Models;

public partial class Kullanicilar
{
    public int KullaniciId { get; set; }

    public string? AdSoyad { get; set; }

    public string? Email { get; set; }

    public string? Sifre { get; set; }

    public string? Rol { get; set; }

    public virtual ICollection<Ilanlar> Ilanlars { get; set; } = new List<Ilanlar>();

    public virtual ICollection<Mesajlar> MesajlarAlicis { get; set; } = new List<Mesajlar>();

    public virtual ICollection<Mesajlar> MesajlarGonderens { get; set; } = new List<Mesajlar>();

    public virtual ICollection<Yorumlar> Yorumlars { get; set; } = new List<Yorumlar>();
}
