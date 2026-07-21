using System.Security.Claims;
using ChamadaGirneManagement.Service.Common;
using ChamadaGirneManagement.Service.DTOs.YonetimKullanicilari;
using ChamadaGirneManagement.Service.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChamadaGirneManagement.WebAPI.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class YonetimKullanicilariController : ControllerBase
{
    private readonly IYonetimKullanicisiService _yonetimKullanicisiService;

    public YonetimKullanicilariController(
        IYonetimKullanicisiService yonetimKullanicisiService
    )
    {
        _yonetimKullanicisiService = yonetimKullanicisiService;
    }

    [HttpGet("GetAll")]
    public async Task<
        ActionResult<ServiceResponse<List<YonetimKullanicisiListeDto>>>
    > GetAll()
    {
        var sonuc = await _yonetimKullanicisiService.GetAllAsync();

        return Ok(sonuc);
    }

    [HttpGet("GetById/{id:int}")]
    public async Task<
        ActionResult<ServiceResponse<YonetimKullanicisiDetayDto>>
    > GetById(int id)
    {
        var sonuc = await _yonetimKullanicisiService.GetByIdAsync(id);

        if (!sonuc.Basarili)
        {
            return NotFound(sonuc);
        }

        return Ok(sonuc);
    }

    [HttpPost("Add")]
    public async Task<
        ActionResult<ServiceResponse<YonetimKullanicisiDetayDto>>
    > Add(
        [FromBody] YonetimKullanicisiEkleDto dto
    )
    {
        var sonuc = await _yonetimKullanicisiService.AddAsync(dto);

        if (!sonuc.Basarili)
        {
            return BadRequest(sonuc);
        }

        return Ok(sonuc);
    }

    [HttpPut("Update/{id:int}")]
    public async Task<ActionResult<ServiceResponse<bool>>> Update(
        int id,
        [FromBody] YonetimKullanicisiGuncelleDto dto
    )
    {
        var sonuc = await _yonetimKullanicisiService.UpdateAsync(id, dto);

        if (!sonuc.Basarili)
        {
            return BadRequest(sonuc);
        }

        return Ok(sonuc);
    }

    [HttpDelete("Delete/{id:int}")]
    public async Task<ActionResult<ServiceResponse<bool>>> Delete(int id)
    {
        var sonuc = await _yonetimKullanicisiService.DeleteAsync(id);

        if (!sonuc.Basarili)
        {
            return BadRequest(sonuc);
        }

        return Ok(sonuc);
    }

    [HttpPut("SifreDegistir")]
    public async Task<ActionResult<ServiceResponse<bool>>> SifreDegistir(
        [FromBody] YonetimKullanicisiSifreDegistirDto dto
    )
    {
        var kullaniciIdDegeri = User.FindFirstValue(
            ClaimTypes.NameIdentifier
        );

        if (!int.TryParse(kullaniciIdDegeri, out var kullaniciId))
        {
            return Unauthorized(
                ServiceResponse<bool>.Failure(
                    "Oturum kullanıcı bilgisi alınamadı."
                )
            );
        }

        var sonuc =
            await _yonetimKullanicisiService.SifreDegistirAsync(
                kullaniciId,
                dto
            );

        if (!sonuc.Basarili)
        {
            return BadRequest(sonuc);
        }

        return Ok(sonuc);
    }
}