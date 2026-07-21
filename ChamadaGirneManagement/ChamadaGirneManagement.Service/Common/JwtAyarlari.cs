namespace ChamadaGirneManagement.Service.Common;

public class JwtAyarlari
{
    public const string SectionName = "Jwt";

    public string Anahtar { get; set; } = string.Empty;

    public string Yayimci { get; set; } = string.Empty;

    public string HedefKitle { get; set; } = string.Empty;

    public int GecerlilikSuresiDakika { get; set; }
}