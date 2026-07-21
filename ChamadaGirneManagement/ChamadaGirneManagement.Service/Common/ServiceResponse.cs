namespace ChamadaGirneManagement.Service.Common;

public class ServiceResponse<T>
{
    public bool Basarili { get; set; }

    public string Mesaj { get; set; } = string.Empty;

    public T? Veri { get; set; }

    public static ServiceResponse<T> Success(
        T veri,
        string mesaj = "İşlem başarıyla tamamlandı."
    )
    {
        return new ServiceResponse<T>
        {
            Basarili = true,
            Mesaj = mesaj,
            Veri = veri
        };
    }

    public static ServiceResponse<T> Failure(string mesaj)
    {
        return new ServiceResponse<T>
        {
            Basarili = false,
            Mesaj = mesaj,
            Veri = default
        };
    }
}