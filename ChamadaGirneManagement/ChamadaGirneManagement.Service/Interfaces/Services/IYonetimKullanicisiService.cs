using ChamadaGirneManagement.Service.Common;
using ChamadaGirneManagement.Service.DTOs.YonetimKullanicilari;

namespace ChamadaGirneManagement.Service.Interfaces.Services;

public interface IYonetimKullanicisiService
{
    Task<ServiceResponse<List<YonetimKullanicisiListeDto>>> GetAllAsync();

    Task<ServiceResponse<YonetimKullanicisiDetayDto>> GetByIdAsync(int id);

    Task<ServiceResponse<YonetimKullanicisiDetayDto>> AddAsync(
        YonetimKullanicisiEkleDto dto
    );

    Task<ServiceResponse<bool>> UpdateAsync(
        int id,
        YonetimKullanicisiGuncelleDto dto
    );

    Task<ServiceResponse<bool>> DeleteAsync(int id);

    Task<ServiceResponse<bool>> SifreDegistirAsync(
        int kullaniciId,
        YonetimKullanicisiSifreDegistirDto dto
    );
}