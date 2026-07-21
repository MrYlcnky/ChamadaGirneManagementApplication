using System.ComponentModel.DataAnnotations;

namespace ChamadaGirneManagement.Service.DTOs.Uygulamalar;

public class UygulamaEkleDto
{
    [Required(ErrorMessage = "Uygulama adı zorunludur.")]
    public string UygulamaAdi { get; set; } = string.Empty;

    [Required(ErrorMessage = "Uygulama kodu zorunludur.")]
    public string UygulamaKodu { get; set; } = string.Empty;

    [Range(1, 65535, ErrorMessage = "Port numarası 1 ile 65535 arasında olmalıdır.")]
    public int Port { get; set; }

    [Required(ErrorMessage = "Geçiş yolu zorunludur.")]
    public string GecisYolu { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Sıra numarası 1 veya daha büyük olmalıdır.")]
    public int SiraNo { get; set; }

    [Required(ErrorMessage = "İstemci anahtarı zorunludur.")]
    public string IstemciAnahtari { get; set; } = string.Empty;

    [Required(ErrorMessage = "Hedef kullanıcı adı zorunludur.")]
    public string HedefKullaniciAdi { get; set; } = string.Empty;
}