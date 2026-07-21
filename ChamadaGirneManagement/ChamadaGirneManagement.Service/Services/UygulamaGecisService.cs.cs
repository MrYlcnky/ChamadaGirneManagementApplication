using System.Security.Cryptography;
using System.Text;
using ChamadaGirneManagement.Domain.Entities;
using ChamadaGirneManagement.Service.Common;
using ChamadaGirneManagement.Service.DTOs.UygulamaGecisleri;
using ChamadaGirneManagement.Service.Interfaces.Repositories;
using ChamadaGirneManagement.Service.Interfaces.Services;
using BC = BCrypt.Net.BCrypt;

namespace ChamadaGirneManagement.Service.Services;

public class UygulamaGecisService : IUygulamaGecisService
{
    private readonly IUygulamaRepository _uygulamaRepository;
    private readonly IYonetimKullanicisiRepository _kullaniciRepository;
    private readonly IUygulamaGecisKoduRepository _gecisKoduRepository;

    public UygulamaGecisService(
        IUygulamaRepository uygulamaRepository,
        IYonetimKullanicisiRepository kullaniciRepository,
        IUygulamaGecisKoduRepository gecisKoduRepository
    )
    {
        _uygulamaRepository = uygulamaRepository;
        _kullaniciRepository = kullaniciRepository;
        _gecisKoduRepository = gecisKoduRepository;
    }

    public async Task<ServiceResponse<GecisKoduOlusturmaSonucuDto>>
        GecisKoduOlusturAsync(
            int yonetimKullaniciId,
            string yonetimDonusAdresi,
            GecisKoduOlusturDto dto
        )
    {
        var kullanici =
            await _kullaniciRepository.GetByIdAsync(yonetimKullaniciId);

        if (kullanici is null)
        {
            return ServiceResponse<GecisKoduOlusturmaSonucuDto>.Failure(
                "Yönetim kullanıcısı bulunamadı."
            );
        }

        var uygulama =
            await _uygulamaRepository.GetByIdAsync(dto.UygulamaId);

        if (uygulama is null)
        {
            return ServiceResponse<GecisKoduOlusturmaSonucuDto>.Failure(
                "Geçiş yapılacak uygulama bulunamadı."
            );
        }

        if (!Uri.TryCreate(
                yonetimDonusAdresi,
                UriKind.Absolute,
                out var yonetimUri
            ))
        {
            return ServiceResponse<GecisKoduOlusturmaSonucuDto>.Failure(
                "Yönetim dönüş adresi geçerli değildir."
            );
        }

        if (yonetimUri.Scheme != Uri.UriSchemeHttp
            && yonetimUri.Scheme != Uri.UriSchemeHttps)
        {
            return ServiceResponse<GecisKoduOlusturmaSonucuDto>.Failure(
                "Yönetim dönüş adresi geçerli değildir."
            );
        }

        var utcSimdi = DateTime.UtcNow;

        // Eski kodların tabloda gereksiz yere birikmesini engeller.
        await _gecisKoduRepository.SuresiGecenKodlariSilAsync(utcSimdi);

        var duzKod = GuvenliKodOlustur();
        var kodHash = KodHashOlustur(duzKod);
        var sonKullanmaTarihi = utcSimdi.AddMinutes(1);

        var gecisKodu = new UygulamaGecisKodu
        {
            KodHash = kodHash,
            YonetimKullaniciId = kullanici.Id,
            UygulamaId = uygulama.Id,
            SonKullanmaTarihi = sonKullanmaTarihi,
            KullanildiMi = false,
            YonetimDonusAdresi = yonetimDonusAdresi
        };

        await _gecisKoduRepository.AddAsync(gecisKodu);

        var gecisAdresi = GecisAdresiOlustur(
            yonetimUri,
            uygulama,
            duzKod
        );

        var sonuc = new GecisKoduOlusturmaSonucuDto
        {
            GecisAdresi = gecisAdresi,
            SonKullanmaTarihi = sonKullanmaTarihi
        };

        return ServiceResponse<GecisKoduOlusturmaSonucuDto>.Success(
            sonuc,
            "Uygulama geçiş kodu başarıyla oluşturuldu."
        );
    }

    public async Task<ServiceResponse<GecisKoduDogrulamaSonucuDto>>
        GecisKoduDogrulaAsync(GecisKoduDogrulaDto dto)
    {
        var uygulamaKodu = dto.UygulamaKodu
            .Trim()
            .ToUpperInvariant();

        var uygulama =
            await _uygulamaRepository.GetByUygulamaKoduAsync(
                uygulamaKodu
            );

        if (uygulama is null)
        {
            return GecersizGecisSonucu();
        }

        var istemciAnahtariDogruMu = BC.Verify(
            dto.IstemciAnahtari,
            uygulama.IstemciAnahtarHash
        );

        if (!istemciAnahtariDogruMu)
        {
            return GecersizGecisSonucu();
        }

        var kodHash = KodHashOlustur(dto.Kod);

        var gecisKodu =
            await _gecisKoduRepository.GetByKodHashAsync(kodHash);

        if (gecisKodu is null)
        {
            return GecersizGecisSonucu();
        }

        if (gecisKodu.UygulamaId != uygulama.Id)
        {
            return GecersizGecisSonucu();
        }

        if (gecisKodu.KullanildiMi)
        {
            return GecersizGecisSonucu();
        }

        var utcSimdi = DateTime.UtcNow;

        if (gecisKodu.SonKullanmaTarihi <= utcSimdi)
        {
            return GecersizGecisSonucu();
        }

        if (gecisKodu.YonetimKullanicisi is null
            || gecisKodu.Uygulama is null)
        {
            return GecersizGecisSonucu();
        }

        var kodTuketildiMi =
            await _gecisKoduRepository.KullanilmamisKoduTuketAsync(
                gecisKodu.Id,
                utcSimdi
            );

        if (!kodTuketildiMi)
        {
            return GecersizGecisSonucu();
        }

        var sonuc = new GecisKoduDogrulamaSonucuDto
        {
            YonetimKullaniciId =
                gecisKodu.YonetimKullanicisi.Id,

            YonetimKullaniciAdi =
                gecisKodu.YonetimKullanicisi.KullaniciAdi,

            YonetimKullaniciAdSoyad =
                gecisKodu.YonetimKullanicisi.AdSoyad,

            HedefKullaniciAdi =
                gecisKodu.Uygulama.HedefKullaniciAdi,

            TamYetkiliYonetimGirisi = true,

            YonetimDonusAdresi =
                gecisKodu.YonetimDonusAdresi
        };

        return ServiceResponse<GecisKoduDogrulamaSonucuDto>.Success(
            sonuc,
            "Uygulama geçiş kodu başarıyla doğrulandı."
        );
    }

    public async Task<ServiceResponse<int>>
        SuresiGecenKodlariTemizleAsync()
    {
        var silinenKodSayisi =
            await _gecisKoduRepository.SuresiGecenKodlariSilAsync(
                DateTime.UtcNow
            );

        return ServiceResponse<int>.Success(
            silinenKodSayisi,
            $"{silinenKodSayisi} adet süresi geçmiş geçiş kodu silindi."
        );
    }

    private static string GuvenliKodOlustur()
    {
        var rastgeleVeri = RandomNumberGenerator.GetBytes(32);

        return Convert.ToBase64String(rastgeleVeri)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }

    private static string KodHashOlustur(string kod)
    {
        var kodVerisi = Encoding.UTF8.GetBytes(kod);
        var hashVerisi = SHA256.HashData(kodVerisi);

        return Convert.ToHexString(hashVerisi);
    }

    private static string GecisAdresiOlustur(
        Uri yonetimUri,
        Uygulama uygulama,
        string kod
    )
    {
        var adresOlusturucu = new UriBuilder
        {
            Scheme = yonetimUri.Scheme,
            Host = yonetimUri.Host,
            Port = uygulama.Port,
            Path = uygulama.GecisYolu,
            Query = $"code={Uri.EscapeDataString(kod)}"
        };

        return adresOlusturucu.Uri.ToString();
    }

    private static ServiceResponse<GecisKoduDogrulamaSonucuDto>
        GecersizGecisSonucu()
    {
        return ServiceResponse<GecisKoduDogrulamaSonucuDto>.Failure(
            "Geçiş kodu veya uygulama doğrulama bilgileri geçersizdir."
        );
    }
}