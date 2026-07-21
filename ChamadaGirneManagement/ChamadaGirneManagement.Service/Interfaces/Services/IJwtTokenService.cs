using ChamadaGirneManagement.Domain.Entities;
using ChamadaGirneManagement.Service.Common;

namespace ChamadaGirneManagement.Service.Interfaces.Services;

public interface IJwtTokenService
{
    TokenSonucu TokenOlustur(YonetimKullanicisi kullanici);
}