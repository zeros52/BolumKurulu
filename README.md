# Bölüm Kurul Karar Otomasyonu

Bölüm kurul toplantılarının gündem maddelerinin oluşturulmasını, üyelere iletilmesini ve toplantı takviminin duyurulmasını sağlar. Toplantı esnasında veya sonrasında alınan kararlar oylama sonuçlarıyla birlikte arşivlenir ve şablonlara uygun olarak tutanak altına alınır.

**Teknolojiler:** ASP.NET Core MVC (.NET 9), Entity Framework Core, SQL Server, ASP.NET Core Identity, Bootstrap

## Gereksinimler

- [.NET 9 SDK](https://dotnet.microsoft.com/download) (`dotnet --version` çıktısı `9.x` olmalı)
- SQL Server Express (veya LocalDB)
- Git
- EF Core aracı:

```powershell
dotnet tool install --global dotnet-ef
```

## Kurulum

**1. Projeyi klonla**

```powershell
git clone https://github.com/zeros52/BolumKurulu.git
cd BolumKurulu
```

**2. Veritabanı bağlantısını ayarla**

Bağlantı bilgisi repoya konmaz, herkes kendi bilgisayarında `user-secrets` ile tanımlar:

```powershell
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" 'Server=localhost\SQLEXPRESS;Database=BolumKurulu;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true'
```

`Server=` kısmını kendi SQL Server adına göre değiştir. Servis adını bulmak için:

```powershell
Get-Service *sql*
```

`MSSQL$SQLEXPRESS` görüyorsan sunucu adı `localhost\SQLEXPRESS` olur.

**3. Veritabanını oluştur**

```powershell
dotnet ef database update
```

**4. Çalıştır**

```powershell
dotnet run
```

Terminalde yazan adresi (örneğin `http://localhost:5242`) tarayıcıda aç.

## İlk giriş

Uygulama ilk açılışta rolleri ve bir yönetici hesabı oluşturur:

| E-posta | Şifre | Rol |
|---|---|---|
| `admin@ornek.com` | `Admin123!` | Admin |

> Bu hesap yalnızca geliştirme içindir. Gerçek kullanımdan önce şifreyi değiştirin.

Kendi kendine kayıt kapalıdır. Yeni kullanıcıları admin, menüdeki **Kullanıcılar** sayfasından oluşturur.

## Roller

| Rol | Yetki |
|---|---|
| **Admin** | Tüm işlemler: kullanıcı yönetimi, öğretim elemanı ekleme/düzenleme/silme, T.C. numarasının tamamını görme |
| **Goruntuleyici** | Yalnızca listeleme ve arama, T.C. numarası maskeli görünür |

## Proje yapısı

```
Controllers/     MVC controller'ları (OgretimElemani, Kullanici, ...)
Data/            DbContext, migration'lar, SeedData
Models/          Entity ve ViewModel sınıfları
Validation/      Özel doğrulama kuralları (T.C. kimlik no)
Views/           Razor görünümleri
```

## Geliştirme kuralları

- Herkes kendi branch'inde çalışır (`feature/a-...`, `feature/b-...`). `main`'e doğrudan commit atılmaz, Pull Request ile birleştirilir.
- Yeni bir branch açmadan önce `main`'i güncelle:

```powershell
git checkout main
git pull
git checkout -b feature/ozellik-adi
```

- **Migration çakışmasını önlemek için:** migration almadan önce `main`'i çek, aldıktan sonra hemen push et. İki kişi aynı anda migration alırsa `ApplicationDbContextModelSnapshot.cs` çakışır.

```powershell
dotnet ef migrations add MigrationAdi
dotnet ef database update
```

- `appsettings.json` içine bağlantı bilgisi veya şifre yazılmaz.

## Sık karşılaşılan sorunlar

| Hata | Çözüm |
|---|---|
| `Unable to locate a Local Database Runtime installation` | Bağlantı satırı LocalDB'yi gösteriyor ama kurulu değil. Adım 2'deki `user-secrets` komutuyla kendi sunucu adını ver. |
| `Connection string 'DefaultConnection' not found` | `user-secrets` komutunu proje klasöründe çalıştırdığından emin ol. |
| `dotnet ef` tanınmıyor | `dotnet tool install --global dotnet-ef` komutunu çalıştır ve yeni bir terminal aç. |
| Derleme sırasında SDK uyumsuzluğu | `dotnet --version` çıktısının `9.x` olduğunu kontrol et. |
