using ChamadaGirneManagement.Domain.Entities;

namespace ChamadaGirneManagement.Service.Interfaces.Repositories;

public interface IUygulamaGecisKoduRepository
{
    Task<UygulamaGecisKodu?> GetByKodHashAsync(string kodHash);

    Task AddAsync(UygulamaGecisKodu gecisKodu);

    Task UpdateAsync(UygulamaGecisKodu gecisKodu);

    Task<int> SuresiGecenKodlariSilAsync(DateTime utcTarih);

    Task<bool> KullanilmamisKoduTuketAsync(int gecisKoduId,DateTime utcTarih);
}