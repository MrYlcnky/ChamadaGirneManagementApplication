using ChamadaGirneManagement.Domain.Entities;
using ChamadaGirneManagement.Persistence.Context;
using ChamadaGirneManagement.Service.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ChamadaGirneManagement.Persistence.Repositories;

public class YonetimKullanicisiRepository : IYonetimKullanicisiRepository
{
    private readonly AppDbContext _context;

    public YonetimKullanicisiRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<YonetimKullanicisi>> GetAllAsync()
    {
        return await _context.YonetimKullanicilari
            .AsNoTracking()
            .OrderBy(x => x.AdSoyad)
            .ToListAsync();
    }

    public async Task<YonetimKullanicisi?> GetByIdAsync(int id)
    {
        return await _context.YonetimKullanicilari
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<YonetimKullanicisi?> GetByKullaniciAdiAsync(
        string kullaniciAdi
    )
    {
        return await _context.YonetimKullanicilari
            .FirstOrDefaultAsync(
                x => x.KullaniciAdi.ToLower() == kullaniciAdi.ToLower()
            );
    }

    public async Task<bool> KullaniciAdiVarMiAsync(
        string kullaniciAdi,
        int? haricTutulacakKullaniciId = null
    )
    {
        return await _context.YonetimKullanicilari.AnyAsync(x =>
            x.KullaniciAdi.ToLower() == kullaniciAdi.ToLower()
            && (
                !haricTutulacakKullaniciId.HasValue
                || x.Id != haricTutulacakKullaniciId.Value
            )
        );
    }

    public async Task AddAsync(YonetimKullanicisi kullanici)
    {
        await _context.YonetimKullanicilari.AddAsync(kullanici);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(YonetimKullanicisi kullanici)
    {
        _context.YonetimKullanicilari.Update(kullanici);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(YonetimKullanicisi kullanici)
    {
        _context.YonetimKullanicilari.Remove(kullanici);
        await _context.SaveChangesAsync();
    }

    public async Task<int> CountAsync()
    {
        return await _context.YonetimKullanicilari.CountAsync();
    }
}