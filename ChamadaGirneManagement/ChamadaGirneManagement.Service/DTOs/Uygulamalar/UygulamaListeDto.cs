namespace ChamadaGirneManagement.Service.DTOs.Uygulamalar;

public class UygulamaListeDto
{
    public int Id { get; set; }

    public string UygulamaAdi { get; set; } = string.Empty;

    public string UygulamaKodu { get; set; } = string.Empty;

    public int Port { get; set; }

    public string GecisYolu { get; set; } = string.Empty;

    public int SiraNo { get; set; }
    public string HedefKullaniciAdi { get; set; } = string.Empty;
}