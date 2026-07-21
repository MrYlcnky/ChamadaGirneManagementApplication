using ChamadaGirneManagement.Domain.Entities;
using ChamadaGirneManagement.Persistence.Context;
using ChamadaGirneManagement.Service.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ChamadaGirneManagement.Persistence.Repositories;

public class UygulamaRepository : IUygulamaRepository
{
    private readonly AppDbContext _context;

    public UygulamaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Uygulama>> GetAllAsync()
    {
        return await _context.Uygulamalar
            .AsNoTracking()
            .OrderBy(x => x.SiraNo)
            .ToListAsync();
    }

    public async Task<Uygulama?> GetByIdAsync(int id)
    {
        return await _context.Uygulamalar
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Uygulama?> GetByUygulamaKoduAsync(
        string uygulamaKodu
    )
    {
        return await _context.Uygulamalar
            .FirstOrDefaultAsync(
                x => x.UygulamaKodu.ToLower() == uygulamaKodu.ToLower()
            );
    }

    public async Task<bool> UygulamaKoduVarMiAsync(
        string uygulamaKodu,
        int? haricTutulacakUygulamaId = null
    )
    {
        return await _context.Uygulamalar.AnyAsync(x =>
            x.UygulamaKodu.ToLower() == uygulamaKodu.ToLower()
            && (
                !haricTutulacakUygulamaId.HasValue
                || x.Id != haricTutulacakUygulamaId.Value
            )
        );
    }

    public async Task AddAsync(Uygulama uygulama)
    {
        await _context.Uygulamalar.AddAsync(uygulama);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Uygulama uygulama)
    {
        _context.Uygulamalar.Update(uygulama);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Uygulama uygulama)
    {
        _context.Uygulamalar.Remove(uygulama);
        await _context.SaveChangesAsync();
    }
}