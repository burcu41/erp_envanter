using ErpApi;

var builder = WebApplication.CreateBuilder(args);

// CORS Politikası
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

var app = builder.Build();
app.UseCors("AllowAll");

// Veritabanı Bağlantısı ve Repository Tanımlaması
string connectionString = "Server=localhost;Database=EnvanterDB;Trusted_Connection=True;TrustServerCertificate=True;";
var repo = new ProductRepository(connectionString);

// ==================== ENDPOINTS (YÖNLENDİRMELER) ====================

// 1. GET: Tüm Ürünleri Listele
app.MapGet("/api/products", () => Results.Ok(repo.GetProducts()));

// 2. GET: Kategorileri Listele
app.MapGet("/api/categories", () => Results.Ok(repo.GetCategories()));

// 3. GET: Dashboard İstatistikleri
app.MapGet("/api/dashboard", () => Results.Ok(repo.GetDashboardStats()));

// 4. POST: Yeni Ürün Ekle (Sunucu Doğrulamalı)
app.MapPost("/api/products", (UrunEkleDto dto) =>
{
    if (string.IsNullOrWhiteSpace(dto.UrunAdi) || dto.UrunAdi.Trim().Length < 2 || dto.KategoriID <= 0 || dto.BirimFiyat <= 0 || dto.StokAdedi < 0 || dto.KritikStokSeviyesi < 1)
        return Results.BadRequest(new { success = false, message = "Geçersiz veya eksik ürün bilgisi." });

    try
    {
        repo.AddProduct(dto);
        return Results.Ok(new { success = true, message = "Ürün başarıyla eklendi." });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { success = false, message = ex.Message });
    }
});

// 5. PUT: Ürün Güncelle
app.MapPut("/api/products", (UrunGuncelleDto dto) =>
{
    if (dto.UrunID <= 0 || string.IsNullOrWhiteSpace(dto.UrunAdi) || dto.UrunAdi.Trim().Length < 2 || dto.BirimFiyat <= 0 || dto.StokAdedi < 0 || dto.KritikStokSeviyesi < 1)
        return Results.BadRequest(new { success = false, message = "Geçersiz güncelleme verisi." });

    try
    {
        var basarili = repo.UpdateProduct(dto);
        return basarili 
            ? Results.Ok(new { success = true, message = "Ürün başarıyla güncellendi." }) 
            : Results.NotFound(new { success = false, message = "Güncellenecek ürün bulunamadı." });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { success = false, message = ex.Message });
    }
});

// 6. PATCH: Hızlı Stok Değiştir (+ / -)
app.MapPatch("/api/products/{id}/stock", (int id, int degisim) =>
{
    var basarili = repo.UpdateStockQuick(id, degisim);
    return basarili ? Results.Ok(new { success = true }) : Results.NotFound();
});

// 7. DELETE: Ürün Sil
app.MapDelete("/api/products/{id}", (int id) =>
{
    var basarili = repo.DeleteProduct(id);
    return basarili ? Results.Ok(new { success = true }) : Results.NotFound();
});

app.Run();

// ==================== DTO (DATA TRANSFER OBJECTS) ====================
public record UrunEkleDto(string UrunAdi, int KategoriID, int StokAdedi, int KritikStokSeviyesi, decimal BirimFiyat);
public record UrunGuncelleDto(int UrunID, string UrunAdi, decimal BirimFiyat, int StokAdedi, int KritikStokSeviyesi);