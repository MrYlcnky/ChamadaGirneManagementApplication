using ChamadaGirneManagement.Service.Common;
using ChamadaGirneManagement.Service.DTOs.KimlikDogrulama;

namespace ChamadaGirneManagement.Service.Interfaces.Services;

public interface IKimlikDogrulamaService
{
    Task<ServiceResponse<GirisSonucuDto>> GirisAsync(GirisDto dto);
}