namespace ChamadaGirneManagement.Domain.Entities;

public class YonetimKullanicisi
{
    public int Id { get; set; }

    public required string KullaniciAdi { get; set; } 

    public required string SifreHash { get; set; } 

    public required string AdSoyad { get; set; } 
}