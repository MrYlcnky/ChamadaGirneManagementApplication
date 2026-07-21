using System.ComponentModel.DataAnnotations;

namespace ChamadaGirneManagement.Service.DTOs.UygulamaGecisleri;

public class GecisKoduDogrulaDto
{
    [Required(ErrorMessage = "Geçiş kodu zorunludur.")]
    public string Kod { get; set; } = string.Empty;

    [Required(ErrorMessage = "Uygulama kodu zorunludur.")]
    public string UygulamaKodu { get; set; } = string.Empty;

    [Required(ErrorMessage = "İstemci anahtarı zorunludur.")]
    public string IstemciAnahtari { get; set; } = string.Empty;
}