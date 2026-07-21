using ChamadaGirneManagement.Service.Common;
using ChamadaGirneManagement.Service.DTOs.Uygulamalar;

namespace ChamadaGirneManagement.Service.Interfaces.Services;

public interface IUygulamaService
{
    Task<ServiceResponse<List<UygulamaListeDto>>> GetAllAsync();

    Task<ServiceResponse<UygulamaDetayDto>> GetByIdAsync(int id);

    Task<ServiceResponse<UygulamaDetayDto>> AddAsync(
        UygulamaEkleDto dto
    );

    Task<ServiceResponse<bool>> UpdateAsync(
        int id,
        UygulamaGuncelleDto dto
    );

    Task<ServiceResponse<bool>> DeleteAsync(int id);

    Task<ServiceResponse<bool>> UygulamaAnahtariYenileAsync(
        int id,
        UygulamaAnahtariYenileDto dto
    );
}