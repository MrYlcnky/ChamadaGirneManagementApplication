namespace ChamadaGirneManagement.Service.DTOs.KimlikDogrulama;

public class GirisSonucuDto
{
    public int YonetimKullaniciId { get; set; }

    public string KullaniciAdi { get; set; } = string.Empty;

    public string AdSoyad { get; set; } = string.Empty;

    public string Token { get; set; } = string.Empty;

    public DateTime SonKullanmaTarihi { get; set; }
}