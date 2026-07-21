namespace ChamadaGirneManagement.Service.DTOs.UygulamaGecisleri;

public class GecisKoduDogrulamaSonucuDto
{
    public int YonetimKullaniciId { get; set; }

    public string YonetimKullaniciAdi { get; set; } = string.Empty;

    public string YonetimKullaniciAdSoyad { get; set; } = string.Empty;

    public string HedefKullaniciAdi { get; set; } = string.Empty;

    public bool TamYetkiliYonetimGirisi { get; set; }

    public string YonetimDonusAdresi { get; set; } = string.Empty;
}