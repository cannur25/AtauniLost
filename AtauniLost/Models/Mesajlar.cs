using System;
using System.Collections.Generic;

namespace AtauniLost.Models;

public partial class Mesajlar
{
    public int MesajId { get; set; }

    public int? GonderenId { get; set; }

    public int? AliciId { get; set; }

    public int? IlanId { get; set; }

    public string? MesajMetni { get; set; }

    public DateTime? Tarih { get; set; }

    public bool? OkunduMu { get; set; }

    public virtual Kullanicilar? Alici { get; set; }

    public virtual Kullanicilar? Gonderen { get; set; }

    public virtual Ilanlar? Ilan { get; set; }
}
