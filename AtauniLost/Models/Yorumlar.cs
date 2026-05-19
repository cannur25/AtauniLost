using System;
using System.Collections.Generic;

namespace AtauniLost.Models;

public partial class Yorumlar
{
    public int YorumId { get; set; }

    public int? IlanId { get; set; }

    public int? KullaniciId { get; set; }

    public string? Icerik { get; set; }

    public DateTime? Tarih { get; set; }

    public virtual Ilanlar? Ilan { get; set; }

    public virtual Kullanicilar? Kullanici { get; set; }
}
