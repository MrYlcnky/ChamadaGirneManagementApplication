# Chamada Girne Management Projesi

## Teknik Devir ve Devam Dokümanı

## 1. Projenin amacı

Bu projenin amacı, kurum içinde kullanılan bağımsız web uygulamalarını tek bir yönetim portalı üzerinden erişilebilir hale getirmektir.

Sisteme yalnızca IT yönetim kullanıcıları giriş yapacaktır. Kullanıcı, yönetim portalında bulunan uygulama kartlarından birine tıkladığında hedef uygulamanın normal giriş ekranını görmeden, o uygulamadaki mevcut tam yetkili kullanıcı hesabıyla oturum açacaktır.

Bu proje aşağıdaki uygulamalar için tasarlanmıştır:

* Chamada AV Control Application
* Chamada Raffle Application
* Chamada Printer Application
* `ChamadPythonProjeler` repository’sinde bulunan bağımsız Python uygulamaları

**Chamada Job Application bu projeye dahil değildir. Bu uygulama kapsam dışıdır ve entegrasyon yapılmamalıdır.**

---

# 2. Temel çalışma mantığı

Management portalı diğer uygulamaların kullanıcı tablolarını ortaklaştırmaz.

Her uygulama kendi:

* Kullanıcı tablosunu
* Rol sistemini
* JWT veya session yapısını
* Login mekanizmasını

korumaya devam eder.

Management portalı yalnızca güvenli ve tek kullanımlık bir geçiş bileti üretir.

Genel akış:

```text
IT kullanıcısı Management portalına giriş yapar
        ↓
Uygulama kartına tıklar
        ↓
Management WebAPI tek kullanımlık geçiş kodu üretir
        ↓
Kullanıcı hedef uygulamanın /management-login adresine yönlendirilir
        ↓
Hedef uygulamanın backend’i geçiş kodunu Management WebAPI’ye doğrulatır
        ↓
Management kodu, uygulama kimliğini ve istemci anahtarını doğrular
        ↓
Management hedef uygulamada kullanılacak kullanıcı adını döndürür
        ↓
Hedef uygulama kendi sistemindeki bu kullanıcıyı bulur
        ↓
Hedef uygulama kendi JWT, cookie veya session oturumunu oluşturur
```

Bu yapıda Management diğer uygulamalara JWT vermez. Her hedef uygulama kendi oturumunu kendisi oluşturur.

---

# 3. Rol ve yetki yaklaşımı

Management projesinde `Admin`, `SuperAdmin` veya `IT` gibi ortak bir hedef uygulama rol enum’u oluşturulmamıştır.

Bunun nedeni her uygulamanın en yüksek yetkili kullanıcı ve rol yapısının farklı olmasıdır.

Örnek:

```text
AV Control     → IT
Raffle         → SuperAdmin
Printer        → Admin
Python projesi → admin
```

Bu nedenle `Uygulamalar` tablosunda:

```csharp
HedefKullaniciAdi
```

alanı bulunmaktadır.

Management geçiş kodunu doğruladığında hedef uygulamaya bu kullanıcı adını döndürür.

Hedef uygulama:

1. `HedefKullaniciAdi` değerini kendi kullanıcı tablosunda bulur.
2. O kullanıcının mevcut rol ve izinlerini kullanır.
3. Kendi oturumunu oluşturur.

Management uygulaması hedef uygulamaların rol isimlerini veya yetki sistemlerini bilmez.

---

# 4. Solution yapısı

Solution şu projelerden oluşmaktadır:

```text
ChamadaGirneManagement
├── ChamadaGirneManagement.WebAPI
├── ChamadaGirneManagement.Service
├── ChamadaGirneManagement.Domain
└── ChamadaGirneManagement.Persistence
```

## Katmanların görevleri

### `ChamadaGirneManagement.Domain`

Sadece temel entity sınıflarını içerir.

```text
Domain
└── Entities
    ├── YonetimKullanicisi.cs
    ├── Uygulama.cs
    └── UygulamaGecisKodu.cs
```

Bu katmanda DTO, repository veya controller bulunmaz.

### `ChamadaGirneManagement.Service`

İş mantığı katmanıdır.

İçeriği:

```text
Service
├── Common
├── DTOs
├── Interfaces
│   ├── Repositories
│   └── Services
├── Services
└── DependencyInjection
```

Bu katmanda:

* DTO’lar
* Service interface’leri
* Repository interface’leri
* İş kuralları
* JWT token üretimi
* BCrypt ve SHA-256 işlemleri

bulunmaktadır.

### `ChamadaGirneManagement.Persistence`

Veritabanı erişim katmanıdır.

İçeriği:

```text
Persistence
├── Context
├── Repositories
├── Migrations
└── DependencyInjection
```

Burada:

* `AppDbContext`
* Entity Framework Core
* MySQL bağlantısı
* Repository implementasyonları
* Migration dosyaları

bulunmaktadır.

### `ChamadaGirneManagement.WebAPI`

HTTP API katmanıdır.

İçeriği:

```text
WebAPI
├── Controllers
├── Program.cs
├── appsettings.json
└── Properties
```

Controller’lar doğrudan repository kullanmaz. Service interface’lerini çağırır.

---

# 5. Katman bağımlılıkları

Bağımlılık yönü şu şekildedir:

```text
Domain
   ↑
Service
   ↑
Persistence
   ↑
WebAPI
```

Gerçek çağrı akışı:

```text
Controller
    ↓
Service interface
    ↓
Service implementasyonu
    ↓
Repository interface
    ↓
Repository implementasyonu
    ↓
AppDbContext
    ↓
MySQL
```

---

# 6. Veritabanı

MySQL kullanılmaktadır.

Bağlantı portu mevcut ortamda:

```text
3307
```

Veritabanı adı:

```text
chamada_girne_management_db
```

Sistemde üç temel tablo vardır.

---

## 6.1. `YonetimKullanicilari`

Management portalına giriş yapabilen IT kullanıcılarını tutar.

Alanlar:

```text
Id
KullaniciAdi
SifreHash
AdSoyad
```

Entity:

```csharp
public class YonetimKullanicisi
{
    public int Id { get; set; }

    public required string KullaniciAdi { get; set; }

    public required string SifreHash { get; set; }

    public required string AdSoyad { get; set; }
}
```

Şifre düz metin olarak tutulmaz. BCrypt hash’i saklanır.

Seed olarak bir IT kullanıcısı tanımlanmıştır. Kaynak kodda mevcut BCrypt hash’i üzerinden giriş yapılmaktadır. Varsayılan şifrenin canlı kullanım öncesinde değiştirilmesi gerekir.

---

## 6.2. `Uygulamalar`

Management portalında gösterilecek hedef uygulamaları tutar.

Alanlar:

```text
Id
UygulamaAdi
UygulamaKodu
Port
GecisYolu
SiraNo
IstemciAnahtarHash
HedefKullaniciAdi
```

Entity:

```csharp
public class Uygulama
{
    public int Id { get; set; }

    public required string UygulamaAdi { get; set; }

    public required string UygulamaKodu { get; set; }

    public int Port { get; set; }

    public required string GecisYolu { get; set; }

    public int SiraNo { get; set; }

    public required string IstemciAnahtarHash { get; set; }

    public required string HedefKullaniciAdi { get; set; }
}
```

Alanların görevleri:

| Alan                 | Açıklama                                                  |
| -------------------- | --------------------------------------------------------- |
| `UygulamaAdi`        | Portal kartında gösterilecek uygulama adı                 |
| `UygulamaKodu`       | Uygulamayı tanımlayan benzersiz sabit kod                 |
| `Port`               | Hedef uygulamanın çalıştığı port                          |
| `GecisYolu`          | Genellikle `/management-login`                            |
| `SiraNo`             | Uygulama kartlarının sıralaması                           |
| `IstemciAnahtarHash` | Hedef uygulamanın Management API’ye kendisini doğrulaması |
| `HedefKullaniciAdi`  | Hedef uygulamada oturum açılacak mevcut kullanıcı         |

`HedefKullaniciAdi` veritabanında `varchar(100)` olarak yapılandırılmıştır.

---

## 6.3. `UygulamaGecisKodlari`

Tek kullanımlık geçiş kodlarını tutar.

Alanlar:

```text
Id
KodHash
YonetimKullaniciId
UygulamaId
SonKullanmaTarihi
KullanildiMi
YonetimDonusAdresi
```

Entity:

```csharp
public class UygulamaGecisKodu
{
    public int Id { get; set; }

    public required string KodHash { get; set; }

    public int YonetimKullaniciId { get; set; }

    public int UygulamaId { get; set; }

    public DateTime SonKullanmaTarihi { get; set; }

    public bool KullanildiMi { get; set; }

    public required string YonetimDonusAdresi { get; set; }

    public YonetimKullanicisi? YonetimKullanicisi { get; set; }

    public Uygulama? Uygulama { get; set; }
}
```

Geçiş kodunun düz hali veritabanında tutulmaz.

Kodun hash’i:

```text
SHA-256
```

ile oluşturulur.

İstemci anahtarı ve kullanıcı şifreleri ise:

```text
BCrypt
```

ile hash’lenir.

---

# 7. Hash yöntemlerinin ayrımı

## Kullanıcı şifresi

```text
BCrypt
```

kullanılır.

```csharp
BCrypt.Net.BCrypt.HashPassword(sifre);
BCrypt.Net.BCrypt.Verify(sifre, sifreHash);
```

## Uygulama istemci anahtarı

```text
BCrypt
```

kullanılır.

Management veritabanında sadece hash bulunur. Hedef uygulama düz istemci anahtarını kendi backend config dosyasında saklar.

## Geçiş kodu

```text
SHA-256
```

kullanılır.

Geçiş kodunu veritabanında hash üzerinden aramak gerektiği için deterministik bir hash gerekir. BCrypt her seferinde farklı salt ürettiğinden geçiş kodu aramasında kullanılmamıştır.

---

# 8. Geçiş kodunun güvenlik özellikleri

Geçiş kodu:

* Kriptografik olarak güvenli random veriyle üretilir.
* URL güvenli Base64 formatına çevrilir.
* Veritabanında yalnızca SHA-256 hash’i tutulur.
* Yaklaşık bir dakika geçerlidir.
* Yalnızca oluşturulduğu hedef uygulama için kullanılabilir.
* Tek kullanımlıktır.
* Süresi dolmuşsa reddedilir.
* Kullanılmışsa reddedilir.
* İstemci anahtarı yanlışsa reddedilir.
* Uygulama kodu yanlışsa reddedilir.

Kod tüketme işlemi atomik şekilde yapılmaktadır.

Repository’de bulunan işlem, veritabanında şu koşullarla tek sorguda güncelleme yapar:

```text
Id doğru
KullanildiMi = false
SonKullanmaTarihi > UTC şimdi
```

Aynı kod iki eş zamanlı istekle kullanılırsa yalnızca bir istek başarılı olur.

---

# 9. DTO yapısı

DTO’lar `ChamadaGirneManagement.Service/DTOs` klasöründedir.

```text
DTOs
├── YonetimKullanicilari
├── Uygulamalar
├── UygulamaGecisleri
└── KimlikDogrulama
```

---

## 9.1. Yönetim kullanıcı DTO’ları

```text
YonetimKullanicisiEkleDto
YonetimKullanicisiGuncelleDto
YonetimKullanicisiListeDto
YonetimKullanicisiDetayDto
YonetimKullanicisiSifreDegistirDto
```

`YonetimKullanicisiGuncelleDto` içinde opsiyonel şifre alanı bulunmaktadır.

Bu alanın amacı bir yönetim kullanıcısının başka bir yönetim kullanıcısının şifresini yönetici olarak sıfırlayabilmesidir.

İki ayrı şifre işlemi vardır:

```text
Update
→ Yönetici başka bir kullanıcının şifresini değiştirebilir.
→ Mevcut şifre sorulmaz.

SifreDegistir
→ Giriş yapan kullanıcı kendi şifresini değiştirir.
→ Mevcut şifre doğrulanır.
```

---

## 9.2. Uygulama DTO’ları

```text
UygulamaEkleDto
UygulamaGuncelleDto
UygulamaListeDto
UygulamaDetayDto
UygulamaAnahtariYenileDto
```

Liste ve detay DTO’larında:

```text
HedefKullaniciAdi
```

bulunmaktadır.

Hiçbir response DTO’sunda:

```text
IstemciAnahtarHash
```

dönmez.

---

## 9.3. Uygulama geçiş DTO’ları

```text
GecisKoduOlusturDto
GecisKoduOlusturmaSonucuDto
GecisKoduDogrulaDto
GecisKoduDogrulamaSonucuDto
```

Doğrulama sonucunda şu bilgiler döner:

```text
YonetimKullaniciId
YonetimKullaniciAdi
YonetimKullaniciAdSoyad
HedefKullaniciAdi
TamYetkiliYonetimGirisi
YonetimDonusAdresi
```

`TamYetkiliYonetimGirisi` yalnızca tüm doğrulamalar başarılı olduğunda `true` olur.

---

## 9.4. Kimlik doğrulama DTO’ları

```text
GirisDto
GirisSonucuDto
```

Başarılı giriş sonucunda:

```text
YonetimKullaniciId
KullaniciAdi
AdSoyad
Token
SonKullanmaTarihi
```

döner.

---

# 10. Ortak servis cevap modeli

Servis cevapları aşağıdaki modelle standartlaştırılmıştır:

```csharp
public class ServiceResponse<T>
{
    public bool Basarili { get; set; }

    public string Mesaj { get; set; } = string.Empty;

    public T? Veri { get; set; }
}
```

Örnek başarılı cevap:

```json
{
  "basarili": true,
  "mesaj": "İşlem başarıyla tamamlandı.",
  "veri": {}
}
```

Örnek başarısız cevap:

```json
{
  "basarili": false,
  "mesaj": "Uygulama bulunamadı.",
  "veri": null
}
```

---

# 11. Repository yapısı

Repository interface’leri Service katmanındadır:

```text
Service/Interfaces/Repositories
├── IYonetimKullanicisiRepository
├── IUygulamaRepository
└── IUygulamaGecisKoduRepository
```

Implementasyonları Persistence katmanındadır:

```text
Persistence/Repositories
├── YonetimKullanicisiRepository
├── UygulamaRepository
└── UygulamaGecisKoduRepository
```

Generic repository kullanılmamıştır.

Her entity için yalnızca ihtiyaç duyulan metotlar açık şekilde tanımlanmıştır.

---

# 12. Service yapısı

Servisler:

```text
Service/Services
├── YonetimKullanicisiService
├── UygulamaService
├── UygulamaGecisService
├── KimlikDogrulamaService
└── JwtTokenService
```

Interface’leri:

```text
Service/Interfaces/Services
├── IYonetimKullanicisiService
├── IUygulamaService
├── IUygulamaGecisService
├── IKimlikDogrulamaService
└── IJwtTokenService
```

---

# 13. JWT yapısı

Management portalına girişte JWT oluşturulur.

JWT claim’leri:

```text
sub
NameIdentifier
Name
adSoyad
jti
```

Management projesinde rol claim’i bulunmaz.

Management’a giriş yapabilen kullanıcıların tamamı aynı yetki seviyesinde kabul edilir.

JWT geçerlilik süresi mevcut ayarda:

```text
480 dakika
```

yani sekiz saattir.

Token doğrulamasında:

```text
Issuer
Audience
Signing Key
Lifetime
```

kontrol edilir.

`ClockSkew` sıfırdır. Token süresi dolduğu anda geçersiz olur.

---

# 14. Controller ve endpoint yapısı

Controller metotlarında açık route isimleri kullanılmıştır.

Örnek:

```csharp
[HttpGet("GetAll")]
[HttpGet("GetById/{id:int}")]
[HttpPost("Add")]
[HttpPut("Update/{id:int}")]
[HttpDelete("Delete/{id:int}")]
```

---

## 14.1. Kimlik doğrulama endpoint’i

```text
POST /api/KimlikDogrulama/Giris
```

Bu endpoint anonimdir.

Başarısız girişte kullanıcı adı veya şifrenin hangisinin yanlış olduğu açıklanmaz.

Cevap:

```text
Kullanıcı adı veya şifre hatalıdır.
```

---

## 14.2. Yönetim kullanıcıları endpoint’leri

```text
GET    /api/YonetimKullanicilari/GetAll
GET    /api/YonetimKullanicilari/GetById/{id}
POST   /api/YonetimKullanicilari/Add
PUT    /api/YonetimKullanicilari/Update/{id}
DELETE /api/YonetimKullanicilari/Delete/{id}
PUT    /api/YonetimKullanicilari/SifreDegistir
```

Bu controller `[Authorize]` ile korunmaktadır.

Sistemdeki son yönetim kullanıcısının silinmesi engellenmiştir.

---

## 14.3. Uygulama endpoint’leri

```text
GET    /api/Uygulamalar/GetAll
GET    /api/Uygulamalar/GetById/{id}
POST   /api/Uygulamalar/Add
PUT    /api/Uygulamalar/Update/{id}
DELETE /api/Uygulamalar/Delete/{id}
PUT    /api/Uygulamalar/UygulamaAnahtariYenile/{id}
```

Bu controller `[Authorize]` ile korunmaktadır.

---

## 14.4. Uygulama geçiş endpoint’leri

```text
POST   /api/UygulamaGecisleri/GecisKoduOlustur
POST   /api/UygulamaGecisleri/GecisKoduDogrula
DELETE /api/UygulamaGecisleri/SuresiGecenKodlariTemizle
```

### `GecisKoduOlustur`

JWT zorunludur.

Yönetim kullanıcı ID’si JWT içinden alınır.

Management dönüş adresi:

```csharp
$"{Request.Scheme}://{Request.Host}"
```

ile üretilir.

Portal `website` hostname’iyle açılmışsa dönüş adresi `website` olur. IP ile açılmışsa dönüş adresi IP olur.

### `GecisKoduDogrula`

Anonim endpoint’tir çünkü hedef uygulamanın backend’i çağırır.

Ancak şu bilgiler doğrulandığı için korumasız değildir:

```text
Kod
UygulamaKodu
IstemciAnahtari
```

### `SuresiGecenKodlariTemizle`

JWT zorunludur.

Süresi dolmuş kodları veritabanından temizler.

---

# 15. CORS

React frontend’in WebAPI’ye erişebilmesi için CORS yapılandırılmıştır.

İzin verilen origin’ler `appsettings.json` içindeki:

```text
Cors:AllowedOrigins
```

bölümünden okunmaktadır.

Mevcut geliştirme adresleri:

```text
http://localhost:5173
http://127.0.0.1:5173
http://website:5173
http://172.16.0.36:5173
```

Frontend portu değişirse bu liste güncellenmelidir.

JWT cookie yerine `Authorization` header ile gönderileceği için `AllowCredentials` kullanılmamıştır.

---

# 16. Çalışma ortamı

Sistem yerel ağda çalışacaktır.

Planlanan Management adresleri:

```text
http://website:7002
http://172.16.0.36:7002
```

Hedef uygulamalar farklı portlarda çalışacaktır.

Management, uygulamaya yönlendirme adresini şu şekilde oluşturur:

```text
Management’ın açıldığı scheme
+
Management’ın açıldığı host
+
Uygulamanın veritabanındaki portu
+
Uygulamanın GecisYolu alanı
```

Örnek:

```text
http://website:7003/management-login?code=...
```

veya:

```text
http://172.16.0.36:7003/management-login?code=...
```

---

# 17. Dependency Injection

Persistence kayıtları:

```text
AddPersistenceServices
```

metoduyla yapılmaktadır.

Service kayıtları:

```text
AddServiceServices
```

metoduyla yapılmaktadır.

`Program.cs` içinde:

```csharp
builder.Services.AddPersistenceServices(builder.Configuration);
builder.Services.AddServiceServices();
```

çağrıları bulunmaktadır.

---

# 18. Kullanılan önemli NuGet paketleri

Temel paketler:

```text
Microsoft.EntityFrameworkCore 8.0.28
Pomelo.EntityFrameworkCore.MySql 8.0.3
Microsoft.EntityFrameworkCore.Design 8.0.28
BCrypt.Net-Next 4.2.0
System.IdentityModel.Tokens.Jwt
Microsoft.Extensions.Options
Microsoft.AspNetCore.Authentication.JwtBearer 8.0.28
```

EF CLI aracı da `8.0.28` sürümüne yükseltilmiştir.

---

# 19. Migration durumu

İlk migration ile tablolar oluşturulmuştur.

Daha sonra `Uygulamalar` tablosuna:

```text
HedefKullaniciAdi
```

kolonu eklenmiştir.

Sonraki migration ile kolon:

```text
longtext
```

tipinden:

```text
varchar(100)
```

tipine dönüştürülmüştür.

Migration’lar veritabanına uygulanmıştır.

Yeni entity değişikliklerinde şu komut yapısı kullanılmalıdır:

```powershell
dotnet ef migrations add MigrationAdi \
  --project ChamadaGirneManagement.Persistence \
  --startup-project ChamadaGirneManagement.WebAPI \
  --output-dir Migrations
```

Ardından:

```powershell
dotnet ef database update \
  --project ChamadaGirneManagement.Persistence \
  --startup-project ChamadaGirneManagement.WebAPI
```

---

# 20. Güvenlik açısından önemli bilgiler

Aşağıdaki değerler frontend’e gönderilmemelidir:

```text
SifreHash
IstemciAnahtarHash
JWT Anahtarı
MySQL şifresi
Hedef uygulamaların düz istemci anahtarları
```

Hedef uygulamaların istemci anahtarları:

* Management’a uygulama eklenirken düz olarak gönderilir.
* Management veritabanında BCrypt hash’i saklanır.
* Hedef uygulamanın backend config dosyasında düz hali bulunur.
* Frontend JavaScript kodunda bulunmaz.

Her hedef uygulama için farklı bir istemci anahtarı kullanılmalıdır.

---

# 21. Backendin mevcut durumu

Backend, frontend geliştirmesine başlanabilecek durumdadır.

Tamamlanan başlıklar:

* Entity yapısı
* MySQL bağlantısı
* Migration’lar
* Kullanıcı CRUD
* Uygulama CRUD
* Şifre değiştirme ve yönetici şifre sıfırlama
* JWT giriş sistemi
* Swagger JWT desteği
* Repository yapısı
* Service yapısı
* Dependency Injection
* Uygulama istemci anahtarı
* Tek kullanımlık geçiş kodu
* Geçiş kodunun atomik tüketilmesi
* Uygulama bazlı hedef kullanıcı
* CORS

---

# 22. Henüz yapılmamış veya ertelenmiş backend işleri

Aşağıdaki işler frontend geliştirmesini engellemez ancak sistem tamamlanmadan değerlendirilebilir:

## Global exception middleware

Beklenmeyen hataların standart cevap formatında dönmesi için middleware eklenebilir.

Beklenen cevap:

```json
{
  "basarili": false,
  "mesaj": "Beklenmeyen bir hata oluştu.",
  "veri": null
}
```

## Kendi hesabını silme kontrolü

Sistemdeki son kullanıcı silinememektedir.

Ancak giriş yapan kullanıcının, başka kullanıcılar varken kendi hesabını silmesi ayrıca engellenebilir.

Bunun için controller’dan JWT kullanıcı ID’si Service’e iletilebilir.

## Rate limiting

Özellikle şu endpoint’lere istek sınırı eklenebilir:

```text
POST /api/KimlikDogrulama/Giris
POST /api/UygulamaGecisleri/GecisKoduDogrula
```

Yerel ağ kullanımı nedeniyle şimdilik zorunlu görülmemiştir.

## Forwarded headers

Sistem IIS veya reverse proxy arkasında çalışırsa gerçek scheme ve host bilgisini almak için forwarded headers yapılandırması eklenmelidir.

Doğrudan Kestrel ve yerel ağ kullanımında şimdilik gerekli değildir.

## Secret yönetimi

Mevcut bağlantı bilgileri ve JWT anahtarı `appsettings.json` içinde bulunmaktadır.

Kaynak kod bir repository’ye gönderilecekse gerçek değerler:

* Environment variable
* IIS environment settings
* `appsettings.Production.json`
* .NET User Secrets

üzerinden verilmelidir.

---

# 23. Frontend’de yapılması gerekenler

Frontend için React ve Vite kullanılabilir.

Önerilen yapı:

```text
src
├── api
├── components
├── context
├── hooks
├── layouts
├── pages
├── routes
├── services
└── utils
```

---

## 23.1. Giriş sayfası

Kullanıcıdan:

```text
KullaniciAdi
Sifre
```

alınır.

İstek:

```text
POST /api/KimlikDogrulama/Giris
```

Başarılı olduğunda:

* Token saklanır.
* Kullanıcı bilgileri saklanır.
* Uygulama kartlarının bulunduğu ana sayfaya yönlendirilir.

JWT bütün korumalı isteklerde:

```http
Authorization: Bearer TOKEN
```

başlığıyla gönderilir.

---

## 23.2. Ana uygulama ekranı

Uygulamalar şu endpoint’ten alınır:

```text
GET /api/Uygulamalar/GetAll
```

Kartlar:

```text
SiraNo
```

değerine göre sıralı gelecektir.

Her kartta en az:

* Uygulama adı
* Uygulama kodu
* Uygulamaya git butonu

bulunabilir.

---

## 23.3. Uygulamaya geçiş

Karta tıklandığında:

```text
POST /api/UygulamaGecisleri/GecisKoduOlustur
```

çağrılır.

İstek:

```json
{
  "uygulamaId": 1
}
```

Başarılı cevapta:

```text
GecisAdresi
```

döner.

Frontend:

```javascript
window.location.href = sonuc.veri.gecisAdresi;
```

ile hedef uygulamaya yönlendirme yapar.

---

## 23.4. Yönetim kullanıcıları ekranı

Kullanılacak endpoint’ler:

```text
GET    GetAll
GET    GetById
POST   Add
PUT    Update
DELETE Delete
PUT    SifreDegistir
```

Kullanıcı yönetim ekranında:

* Listeleme
* Yeni kullanıcı ekleme
* Kullanıcı bilgisi güncelleme
* Yönetici olarak şifre sıfırlama
* Kullanıcı silme

işlemleri yapılabilir.

Giriş yapan kullanıcının kendi şifre değişikliği ayrı ekran veya profil menüsünde bulunmalıdır.

---

## 23.5. Uygulama yönetim ekranı

Kullanılacak endpoint’ler:

```text
GET    GetAll
GET    GetById
POST   Add
PUT    Update
DELETE Delete
PUT    UygulamaAnahtariYenile
```

Uygulama formunda:

```text
UygulamaAdi
UygulamaKodu
Port
GecisYolu
SiraNo
HedefKullaniciAdi
IstemciAnahtari
```

alanları bulunmalıdır.

Güncelleme ekranında istemci anahtarı gösterilmez.

İstemci anahtarı için ayrı yenileme işlemi kullanılmalıdır.

---

# 24. Hedef uygulamalarda yapılması gereken entegrasyon

Her hedef uygulamaya aşağıdaki endpoint veya route eklenmelidir:

```text
/management-login
```

Bu route URL’den:

```text
code
```

query parametresini alır.

Örnek:

```text
http://website:7003/management-login?code=...
```

---

## Hedef uygulama entegrasyon akışı

### 1. Kodu al

```text
code
```

değeri URL’den alınır.

### 2. Management WebAPI’ye doğrulama isteği gönder

```text
POST /api/UygulamaGecisleri/GecisKoduDogrula
```

Body:

```json
{
  "kod": "URLDEN_GELEN_KOD",
  "uygulamaKodu": "RAFFLE",
  "istemciAnahtari": "HEDEF_UYGULAMA_BACKEND_CONFIG_DEGERI"
}
```

Bu istek hedef uygulamanın frontend’i tarafından yapılmamalıdır.

Hedef uygulamanın backend’i çağırmalıdır.

### 3. Doğrulama sonucunu kontrol et

Başarılı cevapta:

```text
TamYetkiliYonetimGirisi = true
```

olmalıdır.

### 4. Kullanıcıyı bul

Cevaptaki:

```text
HedefKullaniciAdi
```

kendi veritabanında aranır.

### 5. Yerel oturumu oluştur

Hedef uygulama kendi mevcut yöntemini kullanır:

* JWT
* Cookie
* Flask session
* Django session
* ASP.NET session

### 6. Kodu URL’den temizle

Kullanıcı uygulamanın ana sayfasına yönlendirilmelidir.

Geçiş kodu browser history içinde açık şekilde bırakılmamalıdır.

### 7. Management dönüş adresini sakla

Cevaptaki:

```text
YonetimDonusAdresi
```

session veya başka uygun yerde saklanabilir.

Hedef uygulamaya bir:

```text
Management Portalına Dön
```

butonu eklenebilir.

---

# 25. Uygulamalar tek tek incelenirken kontrol edilecekler

Her hedef uygulama için şu sorular cevaplanmalıdır:

1. Backend teknolojisi nedir?
2. Kullanıcı tablosu hangisidir?
3. Login sistemi nasıl çalışır?
4. JWT mi, cookie mi, session mı kullanılır?
5. En yetkili mevcut kullanıcı hangisidir?
6. Bu kullanıcı kullanıcı adıyla bulunabilir mi?
7. Rol kontrolü hangi alan üzerinden yapılır?
8. Oturum oluşturmak için hangi servis veya fonksiyon çağrılmalıdır?
9. Management dönüş adresi nerede saklanabilir?
10. İstemci anahtarı hangi config dosyasında tutulacaktır?

Her uygulamada mevcut login akışı korunmalıdır.

Management entegrasyonu mevcut login endpoint’inin yerine geçmemelidir. Ek bir giriş yolu olarak çalışmalıdır.

---

# 26. Devam edecek kişiden beklenen çalışma yöntemi

Projede değişiklikler adım adım yapılmalıdır.

Her adımda:

1. Önce mevcut dosyalar incelenmeli.
2. Değiştirilecek dosyanın tam yolu belirtilmeli.
3. Değişiklik tek bir konuya odaklanmalı.
4. Kod yazıldıktan sonra `dotnet build` çalıştırılmalı.
5. Build hatası varsa sonraki adıma geçilmemeli.
6. Entity değiştiyse migration gerekip gerekmediği kontrol edilmeli.
7. Hassas veriler response DTO’larına eklenmemeli.
8. Hedef uygulamaların rol sistemleri Management’a taşınmamalı.
9. Chamada Job Application bu projeye eklenmemeli.
10. Mevcut mimari gereksiz yere büyütülmemeli.

---

# 27. Önerilen devam sırası

## Aşama 1 — React frontend

1. React/Vite projesini oluştur.
2. Axios API istemcisini oluştur.
3. JWT interceptor ekle.
4. Auth context oluştur.
5. Login sayfasını yap.
6. Protected route yapısını oluştur.
7. Uygulama kartlarının olduğu dashboard’u yap.
8. Yönetim kullanıcıları CRUD ekranını yap.
9. Uygulamalar CRUD ekranını yap.
10. Uygulama geçiş butonunu çalıştır.

## Aşama 2 — Management frontend/backend birlikte test

1. Login testi
2. JWT ile korumalı endpoint testi
3. Kullanıcı CRUD testi
4. Uygulama CRUD testi
5. İstemci anahtarı yenileme testi
6. Geçiş kodu üretme testi
7. Süresi dolan kod testi
8. Kullanılmış kod testi
9. Yanlış uygulama kodu testi
10. Yanlış istemci anahtarı testi

## Aşama 3 — Hedef uygulama entegrasyonları

Önerilen sıra:

```text
1. Raffle
2. AV Control
3. Printer
4. Python uygulamaları
```

İlk uygulamada entegrasyon modeli kesinleştirildikten sonra diğer uygulamalara benzer yapı uygulanabilir.

## Aşama 4 — Ağ ortamına alma

1. WebAPI’yi `7002` portunda çalıştır.
2. Gerekli firewall kuralını ekle.
3. `website` DNS alias çözümlemesini kontrol et.
4. IP üzerinden erişimi kontrol et.
5. CORS origin listesini gerçek frontend adresleriyle güncelle.
6. Hedef uygulamaların portlarını `Uygulamalar` tablosuna ekle.
7. Her uygulama için ayrı istemci anahtarı üret.
8. Anahtarların düz halini yalnızca hedef backend’lere ekle.

---

# 28. Değiştirilmemesi gereken temel kararlar

Aşağıdaki kararlar bilinçli alınmıştır:

* Ortak kullanıcı tablosu yapılmayacak.
* Hedef uygulamaların mevcut login sistemi kaldırılmayacak.
* Hedef uygulamaların rol yapıları Management’a taşınmayacak.
* Management hedef uygulama adına JWT üretmeyecek.
* Tarayıcı doğrudan geçiş kodu doğrulaması yapmayacak.
* İstemci anahtarı frontend’de tutulmayacak.
* Geçiş kodu düz metin olarak veritabanında saklanmayacak.
* Her hedef uygulama kendi yerel oturumunu oluşturacak.
* Job Application entegrasyona dahil edilmeyecek.
* Sistem sade tutulacak ve gereksiz tablo eklenmeyecek.

---

# 29. Kısa proje özeti

Bu proje, IT kullanıcılarının tek bir portal üzerinden kurum içindeki bağımsız uygulamalara tam yetkili şekilde geçebilmesini sağlar.

Management uygulaması:

* Kendi kullanıcılarını yönetir.
* Uygulamaları ve portlarını yönetir.
* Uygulama bazlı gizli istemci anahtarı saklar.
* Tek kullanımlık geçiş kodu üretir.
* Hedef uygulamayı doğrular.
* Hedef uygulamada kullanılacak mevcut kullanıcı adını döndürür.

Hedef uygulama:

* Geçiş kodunu backend üzerinden doğrulatır.
* Kendi veritabanındaki tam yetkili kullanıcıyı bulur.
* Kendi oturumunu oluşturur.
* Mevcut login sistemini korur.

Backend şu anda frontend geliştirmesine başlanabilecek seviyededir.
