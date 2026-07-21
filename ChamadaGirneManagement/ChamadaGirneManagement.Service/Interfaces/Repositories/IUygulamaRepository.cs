using ChamadaGirneManagement.Domain.Entities;

namespace ChamadaGirneManagement.Service.Interfaces.Repositories;

public interface IUygulamaRepository
{
    Task<List<Uygulama>> GetAllAsync();

    Task<Uygulama?> GetByIdAsync(int id);

    Task<Uygulama?> GetByUygulamaKoduAsync(string uygulamaKodu);

    Task<bool> UygulamaKoduVarMiAsync(
        string uygulamaKodu,
        int? haricTutulacakUygulamaId = null
    );

    Task AddAsync(Uygulama uygulama);

    Task UpdateAsync(Uygulama uygulama);

    Task DeleteAsync(Uygulama uygulama);
}