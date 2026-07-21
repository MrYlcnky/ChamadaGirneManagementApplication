using ChamadaGirneManagement.Service.Common;
using ChamadaGirneManagement.Service.DTOs.KimlikDogrulama;
using ChamadaGirneManagement.Service.Interfaces.Repositories;
using ChamadaGirneManagement.Service.Interfaces.Services;
using BC = BCrypt.Net.BCrypt;

namespace ChamadaGirneManagement.Service.Services;

public class KimlikDogrulamaService : IKimlikDogrulamaService
{
    private readonly IYonetimKullanicisiRepository _kullaniciRepository;
    private readonly IJwtTokenService _jwtTokenService;

    public KimlikDogrulamaService(
        IYonetimKullanicisiRepository kullaniciRepository,
        IJwtTokenService jwtTokenService
    )
    {
        _kullaniciRepository = kullaniciRepository;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<ServiceResponse<GirisSonucuDto>> GirisAsync(
        GirisDto dto
    )
    {
        var kullaniciAdi = dto.KullaniciAdi.Trim();

        var kullanici =
            await _kullaniciRepository.GetByKullaniciAdiAsync(
                kullaniciAdi
            );

        if (kullanici is null)
        {
            return ServiceResponse<GirisSonucuDto>.Failure(
                "Kullanıcı adı veya şifre hatalıdır."
            );
        }

        var sifreDogruMu = BC.Verify(
            dto.Sifre,
            kullanici.SifreHash
        );

        if (!sifreDogruMu)
        {
            return ServiceResponse<GirisSonucuDto>.Failure(
                "Kullanıcı adı veya şifre hatalıdır."
            );
        }

        var tokenSonucu = _jwtTokenService.TokenOlustur(kullanici);

        var girisSonucu = new GirisSonucuDto
        {
            YonetimKullaniciId = kullanici.Id,
            KullaniciAdi = kullanici.KullaniciAdi,
            AdSoyad = kullanici.AdSoyad,
            Token = tokenSonucu.Token,
            SonKullanmaTarihi = tokenSonucu.SonKullanmaTarihi
        };

        return ServiceResponse<GirisSonucuDto>.Success(
            girisSonucu,
            "Giriş işlemi başarılı."
        );
    }
}