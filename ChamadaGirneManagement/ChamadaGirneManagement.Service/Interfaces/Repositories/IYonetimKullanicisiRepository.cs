using ChamadaGirneManagement.Domain.Entities;

namespace ChamadaGirneManagement.Service.Interfaces.Repositories;

public interface IYonetimKullanicisiRepository
{
    Task<List<YonetimKullanicisi>> GetAllAsync();

    Task<YonetimKullanicisi?> GetByIdAsync(int id);

    Task<YonetimKullanicisi?> GetByKullaniciAdiAsync(string kullaniciAdi);

    Task<bool> KullaniciAdiVarMiAsync(
        string kullaniciAdi,
        int? haricTutulacakKullaniciId = null
    );

    Task AddAsync(YonetimKullanicisi kullanici);

    Task UpdateAsync(YonetimKullanicisi kullanici);

    Task DeleteAsync(YonetimKullanicisi kullanici);

    Task<int> CountAsync();
}