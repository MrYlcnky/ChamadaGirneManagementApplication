namespace ChamadaGirneManagement.Domain.Entities;

public class UygulamaGecisKodu
{
    public int Id { get; set; }

    public required string KodHash { get; set; }

    public int YonetimKullaniciId { get; set; }

    public int UygulamaId { get; set; }

    public DateTime SonKullanmaTarihi { get; set; }

    public bool KullanildiMi { get; set; }

    public required string YonetimDonusAdresi { get; set; }

    public  YonetimKullanicisi? YonetimKullanicisi { get; set; }

    public  Uygulama? Uygulama { get; set; }
}