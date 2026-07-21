using System.Security.Claims;
using ChamadaGirneManagement.Service.Common;
using ChamadaGirneManagement.Service.DTOs.UygulamaGecisleri;
using ChamadaGirneManagement.Service.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChamadaGirneManagement.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UygulamaGecisleriController : ControllerBase
{
    private readonly IUygulamaGecisService _uygulamaGecisService;

    public UygulamaGecisleriController(
        IUygulamaGecisService uygulamaGecisService
    )
    {
        _uygulamaGecisService = uygulamaGecisService;
    }

    [Authorize]
    [HttpPost("GecisKoduOlustur")]
    public async Task<
        ActionResult<ServiceResponse<GecisKoduOlusturmaSonucuDto>>
    > GecisKoduOlustur(
        [FromBody] GecisKoduOlusturDto dto
    )
    {
        var kullaniciIdDegeri = User.FindFirstValue(
            ClaimTypes.NameIdentifier
        );

        if (!int.TryParse(kullaniciIdDegeri, out var kullaniciId))
        {
            return Unauthorized(
                ServiceResponse<GecisKoduOlusturmaSonucuDto>.Failure(
                    "Oturum kullanıcı bilgisi alınamadı."
                )
            );
        }

        var yonetimDonusAdresi =
            $"{Request.Scheme}://{Request.Host}";

        var sonuc =
            await _uygulamaGecisService.GecisKoduOlusturAsync(
                kullaniciId,
                yonetimDonusAdresi,
                dto
            );

        if (!sonuc.Basarili)
        {
            return BadRequest(sonuc);
        }

        return Ok(sonuc);
    }

    [AllowAnonymous]
    [HttpPost("GecisKoduDogrula")]
    public async Task<
        ActionResult<ServiceResponse<GecisKoduDogrulamaSonucuDto>>
    > GecisKoduDogrula(
        [FromBody] GecisKoduDogrulaDto dto
    )
    {
        var sonuc =
            await _uygulamaGecisService.GecisKoduDogrulaAsync(dto);

        if (!sonuc.Basarili)
        {
            return Unauthorized(sonuc);
        }

        return Ok(sonuc);
    }

    [Authorize]
    [HttpDelete("SuresiGecenKodlariTemizle")]
    public async Task<ActionResult<ServiceResponse<int>>>
        SuresiGecenKodlariTemizle()
    {
        var sonuc =
            await _uygulamaGecisService
                .SuresiGecenKodlariTemizleAsync();

        return Ok(sonuc);
    }
}