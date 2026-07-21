using ChamadaGirneManagement.Service.Common;
using ChamadaGirneManagement.Service.DTOs.UygulamaGecisleri;

namespace ChamadaGirneManagement.Service.Interfaces.Services;

public interface IUygulamaGecisService
{
    Task<ServiceResponse<GecisKoduOlusturmaSonucuDto>> GecisKoduOlusturAsync(
        int yonetimKullaniciId,
        string yonetimDonusAdresi,
        GecisKoduOlusturDto dto
    );

    Task<ServiceResponse<GecisKoduDogrulamaSonucuDto>> GecisKoduDogrulaAsync(
        GecisKoduDogrulaDto dto
    );

    Task<ServiceResponse<int>> SuresiGecenKodlariTemizleAsync();
}