using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ChamadaGirneManagement.Domain.Entities;
using ChamadaGirneManagement.Service.Common;
using ChamadaGirneManagement.Service.Interfaces.Services;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ChamadaGirneManagement.Service.Services;

public class JwtTokenService : IJwtTokenService
{
    private readonly JwtAyarlari _jwtAyarlari;

    public JwtTokenService(IOptions<JwtAyarlari> jwtAyarlari)
    {
        _jwtAyarlari = jwtAyarlari.Value;
    }

    public TokenSonucu TokenOlustur(YonetimKullanicisi kullanici)
    {
        var utcSimdi = DateTime.UtcNow;

        var sonKullanmaTarihi = utcSimdi.AddMinutes(
            _jwtAyarlari.GecerlilikSuresiDakika
        );

        var claims = new List<Claim>
        {
            new(
                JwtRegisteredClaimNames.Sub,
                kullanici.Id.ToString()
            ),
            new(
                ClaimTypes.NameIdentifier,
                kullanici.Id.ToString()
            ),
            new(
                ClaimTypes.Name,
                kullanici.KullaniciAdi
            ),
            new(
                "adSoyad",
                kullanici.AdSoyad
            ),
            new(
                JwtRegisteredClaimNames.Jti,
                Guid.NewGuid().ToString()
            )
        };

        var guvenlikAnahtari = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_jwtAyarlari.Anahtar)
        );

        var imzaBilgileri = new SigningCredentials(
            guvenlikAnahtari,
            SecurityAlgorithms.HmacSha256
        );

        var jwtToken = new JwtSecurityToken(
            issuer: _jwtAyarlari.Yayimci,
            audience: _jwtAyarlari.HedefKitle,
            claims: claims,
            notBefore: utcSimdi,
            expires: sonKullanmaTarihi,
            signingCredentials: imzaBilgileri
        );

        var token = new JwtSecurityTokenHandler()
            .WriteToken(jwtToken);

        return new TokenSonucu
        {
            Token = token,
            SonKullanmaTarihi = sonKullanmaTarihi
        };
    }
}