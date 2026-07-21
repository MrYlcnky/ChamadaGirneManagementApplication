using System.ComponentModel.DataAnnotations;

namespace ChamadaGirneManagement.Service.DTOs.YonetimKullanicilari;

public class YonetimKullanicisiSifreDegistirDto
{
    [Required(ErrorMessage = "Mevcut şifre zorunludur.")]
    public string MevcutSifre { get; set; } = string.Empty;

    [Required(ErrorMessage = "Yeni şifre zorunludur.")]
    public string YeniSifre { get; set; } = string.Empty;
}