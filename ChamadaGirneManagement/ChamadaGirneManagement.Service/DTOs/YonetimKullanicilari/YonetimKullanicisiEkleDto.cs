using System.ComponentModel.DataAnnotations;

namespace ChamadaGirneManagement.Service.DTOs.YonetimKullanicilari;

public class YonetimKullanicisiEkleDto
{
    [Required(ErrorMessage = "Kullanıcı adı zorunludur.")]
    public string KullaniciAdi { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ad soyad zorunludur.")]
    public string AdSoyad { get; set; } = string.Empty;

    [Required(ErrorMessage = "Şifre zorunludur.")]
    public string Sifre { get; set; } = string.Empty;
}