using System.ComponentModel.DataAnnotations;

namespace ChamadaGirneManagement.Service.DTOs.KimlikDogrulama;

public class GirisDto
{
    [Required(ErrorMessage = "Kullanıcı adı zorunludur.")]
    public string KullaniciAdi { get; set; } = string.Empty;

    [Required(ErrorMessage = "Şifre zorunludur.")]
    public string Sifre { get; set; } = string.Empty;
}