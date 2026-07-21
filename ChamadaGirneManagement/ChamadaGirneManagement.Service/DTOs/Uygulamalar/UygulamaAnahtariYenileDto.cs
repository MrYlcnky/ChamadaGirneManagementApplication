using System.ComponentModel.DataAnnotations;

namespace ChamadaGirneManagement.Service.DTOs.Uygulamalar;

public class UygulamaAnahtariYenileDto
{
    [Required(ErrorMessage = "Yeni istemci anahtarı zorunludur.")]
    public string YeniIstemciAnahtari { get; set; } = string.Empty;
}