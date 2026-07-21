using System.Text;
using ChamadaGirneManagement.Persistence.DependencyInjection;
using ChamadaGirneManagement.Service.Common;
using ChamadaGirneManagement.Service.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

const string FrontendCorsPolicy = "FrontendCors";

// Persistence ve repository bağımlılıkları
builder.Services.AddPersistenceServices(builder.Configuration);

// Service bağımlılıkları
builder.Services.AddServiceServices();

// CORS ayarlarını appsettings.json dosyasından al
var izinVerilenOriginler = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>()
    ?? throw new InvalidOperationException(
        "CORS için izin verilen frontend adresleri tanımlanmamıştır."
    );

if (izinVerilenOriginler.Length == 0)
{
    throw new InvalidOperationException(
        "En az bir CORS origin adresi tanımlanmalıdır."
    );
}

builder.Services.AddCors(options =>
{
    options.AddPolicy(FrontendCorsPolicy, policy =>
    {
        policy
            .WithOrigins(izinVerilenOriginler)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// JWT ayarlarını appsettings.json dosyasından al
var jwtBolumu = builder.Configuration.GetSection(
    JwtAyarlari.SectionName
);

builder.Services.Configure<JwtAyarlari>(jwtBolumu);

var jwtAyarlari = jwtBolumu.Get<JwtAyarlari>()
    ?? throw new InvalidOperationException(
        "JWT ayarları bulunamadı."
    );

if (string.IsNullOrWhiteSpace(jwtAyarlari.Anahtar))
{
    throw new InvalidOperationException(
        "JWT güvenlik anahtarı tanımlanmamıştır."
    );
}

if (Encoding.UTF8.GetByteCount(jwtAyarlari.Anahtar) < 32)
{
    throw new InvalidOperationException(
        "JWT güvenlik anahtarı en az 32 byte olmalıdır."
    );
}

if (string.IsNullOrWhiteSpace(jwtAyarlari.Yayimci))
{
    throw new InvalidOperationException(
        "JWT yayıncı bilgisi tanımlanmamıştır."
    );
}

if (string.IsNullOrWhiteSpace(jwtAyarlari.HedefKitle))
{
    throw new InvalidOperationException(
        "JWT hedef kitle bilgisi tanımlanmamıştır."
    );
}

if (jwtAyarlari.GecerlilikSuresiDakika <= 0)
{
    throw new InvalidOperationException(
        "JWT geçerlilik süresi sıfırdan büyük olmalıdır."
    );
}

var jwtGuvenlikAnahtari = new SymmetricSecurityKey(
    Encoding.UTF8.GetBytes(jwtAyarlari.Anahtar)
);

// JWT kimlik doğrulama
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme =
            JwtBearerDefaults.AuthenticationScheme;

        options.DefaultChallengeScheme =
            JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = jwtGuvenlikAnahtari,

                ValidateIssuer = true,
                ValidIssuer = jwtAyarlari.Yayimci,

                ValidateAudience = true,
                ValidAudience = jwtAyarlari.HedefKitle,

                ValidateLifetime = true,

                // Token süresi dolduğu anda geçersiz olur.
                ClockSkew = TimeSpan.Zero
            };
    });

builder.Services.AddAuthorization();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc(
        "v1",
        new OpenApiInfo
        {
            Title = "Chamada Girne Management WebAPI",
            Version = "v1"
        }
    );

    options.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "JWT token bilgisini giriniz."
        }
    );

    options.AddSecurityRequirement(
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        }
    );
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

app.UseCors(FrontendCorsPolicy);

// Sıralama önemlidir.
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();