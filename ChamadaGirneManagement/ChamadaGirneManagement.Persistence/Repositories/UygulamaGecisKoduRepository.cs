using ChamadaGirneManagement.Domain.Entities;
using ChamadaGirneManagement.Persistence.Context;
using ChamadaGirneManagement.Service.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ChamadaGirneManagement.Persistence.Repositories;

public class UygulamaGecisKoduRepository : IUygulamaGecisKoduRepository
{
    private readonly AppDbContext _context;

    public UygulamaGecisKoduRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<UygulamaGecisKodu?> GetByKodHashAsync(
        string kodHash
    )
    {
        return await _context.UygulamaGecisKodlari
            .Include(x => x.YonetimKullanicisi)
            .Include(x => x.Uygulama)
            .FirstOrDefaultAsync(x => x.KodHash == kodHash);
    }

    public async Task AddAsync(UygulamaGecisKodu gecisKodu)
    {
        await _context.UygulamaGecisKodlari.AddAsync(gecisKodu);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(UygulamaGecisKodu gecisKodu)
    {
        _context.UygulamaGecisKodlari.Update(gecisKodu);
        await _context.SaveChangesAsync();
    }

    public async Task<int> SuresiGecenKodlariSilAsync(DateTime utcTarih)
    {
        var silinecekKodlar = await _context.UygulamaGecisKodlari
            .Where(x => x.SonKullanmaTarihi <= utcTarih)
            .ToListAsync();

        if (silinecekKodlar.Count == 0)
        {
            return 0;
        }

        _context.UygulamaGecisKodlari.RemoveRange(silinecekKodlar);
        await _context.SaveChangesAsync();

        return silinecekKodlar.Count;
    }

    public async Task<bool> KullanilmamisKoduTuketAsync(int gecisKoduId, DateTime utcTarih)
    {
        var etkilenenKayitSayisi = await _context.UygulamaGecisKodlari
            .Where(x =>
                x.Id == gecisKoduId
                && !x.KullanildiMi
                && x.SonKullanmaTarihi > utcTarih
            )
            .ExecuteUpdateAsync(setters =>
                setters.SetProperty(
                    x => x.KullanildiMi,
                    true
                )
            );

        return etkilenenKayitSayisi == 1;
    }
}