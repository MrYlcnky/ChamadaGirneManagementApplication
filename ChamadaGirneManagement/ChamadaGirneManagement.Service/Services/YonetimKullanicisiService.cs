using ChamadaGirneManagement.Domain.Entities;
using ChamadaGirneManagement.Service.Common;
using ChamadaGirneManagement.Service.DTOs.YonetimKullanicilari;
using ChamadaGirneManagement.Service.Interfaces.Repositories;
using ChamadaGirneManagement.Service.Interfaces.Services;
using BC = BCrypt.Net.BCrypt;

namespace ChamadaGirneManagement.Service.Services;

public class YonetimKullanicisiService : IYonetimKullanicisiService
{
    private readonly IYonetimKullanicisiRepository _kullaniciRepository;

    public YonetimKullanicisiService(
        IYonetimKullanicisiRepository kullaniciRepository
    )
    {
        _kullaniciRepository = kullaniciRepository;
    }

    public async Task<ServiceResponse<List<YonetimKullanicisiListeDto>>>
        GetAllAsync()
    {
        var kullanicilar = await _kullaniciRepository.GetAllAsync();

        var kullaniciListesi = kullanicilar
            .Select(x => new YonetimKullanicisiListeDto
            {
                Id = x.Id,
                KullaniciAdi = x.KullaniciAdi,
                AdSoyad = x.AdSoyad
            })
            .ToList();

        return ServiceResponse<List<YonetimKullanicisiListeDto>>.Success(
            kullaniciListesi,
            "Yönetim kullanıcıları başarıyla getirildi."
        );
    }

    public async Task<ServiceResponse<YonetimKullanicisiDetayDto>>
        GetByIdAsync(int id)
    {
        var kullanici = await _kullaniciRepository.GetByIdAsync(id);

        if (kullanici is null)
        {
            return ServiceResponse<YonetimKullanicisiDetayDto>.Failure(
                "Yönetim kullanıcısı bulunamadı."
            );
        }

        var kullaniciDto = new YonetimKullanicisiDetayDto
        {
            Id = kullanici.Id,
            KullaniciAdi = kullanici.KullaniciAdi,
            AdSoyad = kullanici.AdSoyad
        };

        return ServiceResponse<YonetimKullanicisiDetayDto>.Success(
            kullaniciDto,
            "Yönetim kullanıcısı başarıyla getirildi."
        );
    }

    public async Task<ServiceResponse<YonetimKullanicisiDetayDto>>
        AddAsync(YonetimKullanicisiEkleDto dto)
    {
        var kullaniciAdi = dto.KullaniciAdi.Trim();

        var kullaniciAdiVarMi =
            await _kullaniciRepository.KullaniciAdiVarMiAsync(kullaniciAdi);

        if (kullaniciAdiVarMi)
        {
            return ServiceResponse<YonetimKullanicisiDetayDto>.Failure(
                "Bu kullanıcı adıyla kayıtlı başka bir yönetim kullanıcısı bulunmaktadır."
            );
        }

        var kullanici = new YonetimKullanicisi
        {
            KullaniciAdi = kullaniciAdi,
            AdSoyad = dto.AdSoyad.Trim(),
            SifreHash = BC.HashPassword(dto.Sifre)
        };

        await _kullaniciRepository.AddAsync(kullanici);

        var kullaniciDto = new YonetimKullanicisiDetayDto
        {
            Id = kullanici.Id,
            KullaniciAdi = kullanici.KullaniciAdi,
            AdSoyad = kullanici.AdSoyad
        };

        return ServiceResponse<YonetimKullanicisiDetayDto>.Success(
            kullaniciDto,
            "Yönetim kullanıcısı başarıyla eklendi."
        );
    }

    public async Task<ServiceResponse<bool>> UpdateAsync(
        int id,
        YonetimKullanicisiGuncelleDto dto
    )
    {
        var kullanici = await _kullaniciRepository.GetByIdAsync(id);

        if (kullanici is null)
        {
            return ServiceResponse<bool>.Failure(
                "Güncellenecek yönetim kullanıcısı bulunamadı."
            );
        }

        var kullaniciAdi = dto.KullaniciAdi.Trim();

        var kullaniciAdiVarMi =
            await _kullaniciRepository.KullaniciAdiVarMiAsync(
                kullaniciAdi,
                id
            );

        if (kullaniciAdiVarMi)
        {
            return ServiceResponse<bool>.Failure(
                "Bu kullanıcı adıyla kayıtlı başka bir yönetim kullanıcısı bulunmaktadır."
            );
        }

        kullanici.KullaniciAdi = kullaniciAdi;
        kullanici.AdSoyad = dto.AdSoyad.Trim();

        // Yönetici başka bir kullanıcının şifresini sıfırlamak isterse
        // DTO içerisinde yeni şifre gönderir.
        if (!string.IsNullOrWhiteSpace(dto.Sifre))
        {
            kullanici.SifreHash = BC.HashPassword(dto.Sifre);
        }

        await _kullaniciRepository.UpdateAsync(kullanici);

        return ServiceResponse<bool>.Success(
            true,
            "Yönetim kullanıcısı başarıyla güncellendi."
        );
    }

    public async Task<ServiceResponse<bool>> DeleteAsync(int id)
    {
        var kullanici = await _kullaniciRepository.GetByIdAsync(id);

        if (kullanici is null)
        {
            return ServiceResponse<bool>.Failure(
                "Silinecek yönetim kullanıcısı bulunamadı."
            );
        }

        var kullaniciSayisi = await _kullaniciRepository.CountAsync();

        if (kullaniciSayisi <= 1)
        {
            return ServiceResponse<bool>.Failure(
                "Sistemdeki son yönetim kullanıcısı silinemez."
            );
        }

        await _kullaniciRepository.DeleteAsync(kullanici);

        return ServiceResponse<bool>.Success(
            true,
            "Yönetim kullanıcısı başarıyla silindi."
        );
    }

    public async Task<ServiceResponse<bool>> SifreDegistirAsync(
        int kullaniciId,
        YonetimKullanicisiSifreDegistirDto dto
    )
    {
        var kullanici =
            await _kullaniciRepository.GetByIdAsync(kullaniciId);

        if (kullanici is null)
        {
            return ServiceResponse<bool>.Failure(
                "Yönetim kullanıcısı bulunamadı."
            );
        }

        var mevcutSifreDogruMu = BC.Verify(
            dto.MevcutSifre,
            kullanici.SifreHash
        );

        if (!mevcutSifreDogruMu)
        {
            return ServiceResponse<bool>.Failure(
                "Mevcut şifre doğru değildir."
            );
        }

        kullanici.SifreHash = BC.HashPassword(dto.YeniSifre);

        await _kullaniciRepository.UpdateAsync(kullanici);

        return ServiceResponse<bool>.Success(
            true,
            "Şifre başarıyla değiştirildi."
        );
    }
}