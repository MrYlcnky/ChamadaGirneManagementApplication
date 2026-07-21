using ChamadaGirneManagement.Service.Common;
using ChamadaGirneManagement.Service.DTOs.Uygulamalar;
using ChamadaGirneManagement.Service.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChamadaGirneManagement.WebAPI.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class UygulamalarController : ControllerBase
{
    private readonly IUygulamaService _uygulamaService;

    public UygulamalarController(IUygulamaService uygulamaService)
    {
        _uygulamaService = uygulamaService;
    }

    [HttpGet("GetAll")]
    public async Task<
        ActionResult<ServiceResponse<List<UygulamaListeDto>>>
    > GetAll()
    {
        var sonuc = await _uygulamaService.GetAllAsync();

        return Ok(sonuc);
    }

    [HttpGet("GetById/{id:int}")]
    public async Task<
        ActionResult<ServiceResponse<UygulamaDetayDto>>
    > GetById(int id)
    {
        var sonuc = await _uygulamaService.GetByIdAsync(id);

        if (!sonuc.Basarili)
        {
            return NotFound(sonuc);
        }

        return Ok(sonuc);
    }

    [HttpPost("Add")]
    public async Task<
        ActionResult<ServiceResponse<UygulamaDetayDto>>
    > Add([FromBody] UygulamaEkleDto dto)
    {
        var sonuc = await _uygulamaService.AddAsync(dto);

        if (!sonuc.Basarili)
        {
            return BadRequest(sonuc);
        }

        return Ok(sonuc);
    }

    [HttpPut("Update/{id:int}")]
    public async Task<ActionResult<ServiceResponse<bool>>> Update(
        int id,
        [FromBody] UygulamaGuncelleDto dto
    )
    {
        var sonuc = await _uygulamaService.UpdateAsync(id, dto);

        if (!sonuc.Basarili)
        {
            return BadRequest(sonuc);
        }

        return Ok(sonuc);
    }

    [HttpDelete("Delete/{id:int}")]
    public async Task<ActionResult<ServiceResponse<bool>>> Delete(int id)
    {
        var sonuc = await _uygulamaService.DeleteAsync(id);

        if (!sonuc.Basarili)
        {
            return BadRequest(sonuc);
        }

        return Ok(sonuc);
    }

    [HttpPut("UygulamaAnahtariYenile/{id:int}")]
    public async Task<ActionResult<ServiceResponse<bool>>>
        UygulamaAnahtariYenile(
            int id,
            [FromBody] UygulamaAnahtariYenileDto dto
        )
    {
        var sonuc =
            await _uygulamaService.UygulamaAnahtariYenileAsync(
                id,
                dto
            );

        if (!sonuc.Basarili)
        {
            return BadRequest(sonuc);
        }

        return Ok(sonuc);
    }
}