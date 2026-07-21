using ChamadaGirneManagement.Service.Common;
using ChamadaGirneManagement.Service.DTOs.KimlikDogrulama;
using ChamadaGirneManagement.Service.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChamadaGirneManagement.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class KimlikDogrulamaController : ControllerBase
{
    private readonly IKimlikDogrulamaService _kimlikDogrulamaService;

    public KimlikDogrulamaController(
        IKimlikDogrulamaService kimlikDogrulamaService
    )
    {
        _kimlikDogrulamaService = kimlikDogrulamaService;
    }

    [AllowAnonymous]
    [HttpPost("Giris")]
    public async Task<ActionResult<ServiceResponse<GirisSonucuDto>>> Giris(
        [FromBody] GirisDto dto
    )
    {
        var sonuc = await _kimlikDogrulamaService.GirisAsync(dto);

        if (!sonuc.Basarili)
        {
            return Unauthorized(sonuc);
        }

        return Ok(sonuc);
    }
}