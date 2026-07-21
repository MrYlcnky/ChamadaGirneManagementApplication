using ChamadaGirneManagement.Domain.Entities;
using ChamadaGirneManagement.Service.Common;
using ChamadaGirneManagement.Service.DTOs.Uygulamalar;
using ChamadaGirneManagement.Service.Interfaces.Repositories;
using ChamadaGirneManagement.Service.Interfaces.Services;
using BC = BCrypt.Net.BCrypt;

namespace ChamadaGirneManagement.Service.Services;

public class UygulamaService : IUygulamaService
{
    private readonly IUygulamaRepository _uygulamaRepository;

    public UygulamaService(IUygulamaRepository uygulamaRepository)
    {
        _uygulamaRepository = uygulamaRepository;
    }

    public async Task<ServiceResponse<List<UygulamaListeDto>>> GetAllAsync()
    {
        var uygulamalar = await _uygulamaRepository.GetAllAsync();

        var uygulamaListesi = uygulamalar
            .Select(x => new UygulamaListeDto
            {
                Id = x.Id,
                UygulamaAdi = x.UygulamaAdi,
                UygulamaKodu = x.UygulamaKodu,
                Port = x.Port,
                GecisYolu = x.GecisYolu,
                SiraNo = x.SiraNo,
                HedefKullaniciAdi = x.HedefKullaniciAdi
            })
            .ToList();

        return ServiceResponse<List<UygulamaListeDto>>.Success(
            uygulamaListesi,
            "Uygulamalar başarıyla getirildi."
        );
    }

    public async Task<ServiceResponse<UygulamaDetayDto>> GetByIdAsync(int id)
    {
        var uygulama = await _uygulamaRepository.GetByIdAsync(id);

        if (uygulama is null)
        {
            return ServiceResponse<UygulamaDetayDto>.Failure(
                "Uygulama bulunamadı."
            );
        }

        var uygulamaDto = MapToDetayDto(uygulama);

        return ServiceResponse<UygulamaDetayDto>.Success(
            uygulamaDto,
            "Uygulama başarıyla getirildi."
        );
    }

    public async Task<ServiceResponse<UygulamaDetayDto>> AddAsync(
        UygulamaEkleDto dto
    )
    {
        var uygulamaKodu = dto.UygulamaKodu
            .Trim()
            .ToUpperInvariant();

        var uygulamaKoduVarMi =
            await _uygulamaRepository.UygulamaKoduVarMiAsync(
                uygulamaKodu
            );

        if (uygulamaKoduVarMi)
        {
            return ServiceResponse<UygulamaDetayDto>.Failure(
                "Bu uygulama koduyla kayıtlı başka bir uygulama bulunmaktadır."
            );
        }

        var uygulama = new Uygulama
        {
            UygulamaAdi = dto.UygulamaAdi.Trim(),
            UygulamaKodu = uygulamaKodu,
            Port = dto.Port,
            GecisYolu = GecisYolunuDuzenle(dto.GecisYolu),
            SiraNo = dto.SiraNo,
            IstemciAnahtarHash = BC.HashPassword(dto.IstemciAnahtari),
            HedefKullaniciAdi = dto.HedefKullaniciAdi.Trim()
        };

        await _uygulamaRepository.AddAsync(uygulama);

        var uygulamaDto = MapToDetayDto(uygulama);

        return ServiceResponse<UygulamaDetayDto>.Success(
            uygulamaDto,
            "Uygulama başarıyla eklendi."
        );
    }

    public async Task<ServiceResponse<bool>> UpdateAsync(
        int id,
        UygulamaGuncelleDto dto
    )
    {
        var uygulama = await _uygulamaRepository.GetByIdAsync(id);

        if (uygulama is null)
        {
            return ServiceResponse<bool>.Failure(
                "Güncellenecek uygulama bulunamadı."
            );
        }

        var uygulamaKodu = dto.UygulamaKodu
            .Trim()
            .ToUpperInvariant();

        var uygulamaKoduVarMi =
            await _uygulamaRepository.UygulamaKoduVarMiAsync(
                uygulamaKodu,
                id
            );

        if (uygulamaKoduVarMi)
        {
            return ServiceResponse<bool>.Failure(
                "Bu uygulama koduyla kayıtlı başka bir uygulama bulunmaktadır."
            );
        }

        uygulama.UygulamaAdi = dto.UygulamaAdi.Trim();
        uygulama.UygulamaKodu = uygulamaKodu;
        uygulama.Port = dto.Port;
        uygulama.GecisYolu = GecisYolunuDuzenle(dto.GecisYolu);
        uygulama.SiraNo = dto.SiraNo;
        uygulama.HedefKullaniciAdi = dto.HedefKullaniciAdi.Trim();

        await _uygulamaRepository.UpdateAsync(uygulama);

        return ServiceResponse<bool>.Success(
            true,
            "Uygulama başarıyla güncellendi."
        );
    }

    public async Task<ServiceResponse<bool>> DeleteAsync(int id)
    {
        var uygulama = await _uygulamaRepository.GetByIdAsync(id);

        if (uygulama is null)
        {
            return ServiceResponse<bool>.Failure(
                "Silinecek uygulama bulunamadı."
            );
        }

        await _uygulamaRepository.DeleteAsync(uygulama);

        return ServiceResponse<bool>.Success(
            true,
            "Uygulama başarıyla silindi."
        );
    }

    public async Task<ServiceResponse<bool>> UygulamaAnahtariYenileAsync(
        int id,
        UygulamaAnahtariYenileDto dto
    )
    {
        var uygulama = await _uygulamaRepository.GetByIdAsync(id);

        if (uygulama is null)
        {
            return ServiceResponse<bool>.Failure(
                "İstemci anahtarı yenilenecek uygulama bulunamadı."
            );
        }

        uygulama.IstemciAnahtarHash =
            BC.HashPassword(dto.YeniIstemciAnahtari);

        await _uygulamaRepository.UpdateAsync(uygulama);

        return ServiceResponse<bool>.Success(
            true,
            "Uygulama istemci anahtarı başarıyla yenilendi."
        );
    }

    private static UygulamaDetayDto MapToDetayDto(Uygulama uygulama)
    {
        return new UygulamaDetayDto
        {
            Id = uygulama.Id,
            UygulamaAdi = uygulama.UygulamaAdi,
            UygulamaKodu = uygulama.UygulamaKodu,
            Port = uygulama.Port,
            GecisYolu = uygulama.GecisYolu,
            SiraNo = uygulama.SiraNo,
            HedefKullaniciAdi = uygulama.HedefKullaniciAdi
        };
    }

    private static string GecisYolunuDuzenle(string gecisYolu)
    {
        var duzenlenmisYol = gecisYolu.Trim();

        if (!duzenlenmisYol.StartsWith('/'))
        {
            duzenlenmisYol = $"/{duzenlenmisYol}";
        }

        return duzenlenmisYol;
    }
}