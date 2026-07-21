namespace ChamadaGirneManagement.Domain.Entities;

public class Uygulama
{
    public int Id { get; set; }

    public required string UygulamaAdi { get; set; } 

    public required string UygulamaKodu { get; set; } 

    public int Port { get; set; }

    public required string GecisYolu { get; set; } 

    public int SiraNo { get; set; }

    public required string IstemciAnahtarHash { get; set; }
    public required string HedefKullaniciAdi { get; set; }
}