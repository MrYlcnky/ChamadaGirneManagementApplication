using System.ComponentModel.DataAnnotations;

namespace ChamadaGirneManagement.Service.DTOs.UygulamaGecisleri;

public class GecisKoduOlusturDto
{
    [Range(1, int.MaxValue, ErrorMessage = "Uygulama seçimi zorunludur.")]
    public int UygulamaId { get; set; }
}