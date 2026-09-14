using Microsoft.Data.SqlClient;
using System.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

var app = builder.Build();
app.UseCors("AllowAll");

var connectionString = app.Configuration.GetConnectionString("DefaultConnection");


app.MapGet("/api/products", () =>
{
    var urunler = new List<object>();
    using var connection = new SqlConnection(connectionString);
    connection.Open();
    var query = @"SELECT UrunID, UrunAdi, KategoriAdi, StokAdedi, 
                         KritikStokSeviyesi, BirimFiyat, ToplamDeger, StokDurumu 
                  FROM dbo.vw_EnvanterListesi ORDER BY UrunID ASC";

    using var command = new SqlCommand(query, connection);
    using var reader = command.ExecuteReader();
    while (reader.Read())
    {
        urunler.Add(new
        {
            urunID = reader.GetInt32(0),
            urunAdi = reader.GetString(1),
            kategoriAdi = reader.GetString(2),
            stokAdedi = reader.GetInt32(3),
            kritikStokSeviyesi = reader.GetInt32(4),
            birimFiyat = reader.GetDecimal(5),
            toplamDeger = reader.GetDecimal(6),
            stokDurumu = reader.GetString(7)
        });
    }
    return Results.Ok(urunler);
});


app.MapGet("/api/categories", () =>
{
    var kategoriler = new List<object>();
    using var connection = new SqlConnection(connectionString);
    connection.Open();
    var query = "SELECT KategoriID, KategoriAdi FROM dbo.Kategoriler ORDER BY KategoriAdi ASC";

    using var command = new SqlCommand(query, connection);
    using var reader = command.ExecuteReader();
    while (reader.Read())
    {
        kategoriler.Add(new
        {
            kategoriID = reader.GetInt32(0),
            kategoriAdi = reader.GetString(1)
        });
    }
    return Results.Ok(kategoriler);
});


app.MapPost("/api/products", (UrunEkleDto dto) =>
{
    try
    {
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        using var command = new SqlCommand("dbo.sp_YeniUrunEkle", connection);
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.AddWithValue("@UrunAdi", dto.UrunAdi);
        command.Parameters.AddWithValue("@KategoriID", dto.KategoriID);
        command.Parameters.AddWithValue("@StokAdedi", dto.StokAdedi);
        command.Parameters.AddWithValue("@KritikStokSeviyesi", dto.KritikStokSeviyesi);
        command.Parameters.AddWithValue("@BirimFiyat", dto.BirimFiyat);
        command.ExecuteNonQuery();
        return Results.Ok(new { success = true, message = "Ürün başarıyla eklendi." });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { success = false, message = ex.Message });
    }
});


app.MapDelete("/api/products/{id:int}", (int id) =>
{
    try
    {
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        var query = "DELETE FROM dbo.Urunler WHERE UrunID = @UrunID";
        using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@UrunID", id);
        var rows = command.ExecuteNonQuery();
        return rows > 0 ? Results.Ok(new { success = true }) : Results.NotFound();
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { success = false, message = ex.Message });
    }
});


app.MapPut("/api/products", (UrunGuncelleDto dto) =>
{
    try
    {
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        var query = @"UPDATE dbo.Urunler 
                      SET UrunAdi = @UrunAdi, 
                          BirimFiyat = @BirimFiyat, 
                          StokAdedi = @StokAdedi, 
                          KritikStokSeviyesi = @KritikStok 
                      WHERE UrunID = @UrunID";
        using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@UrunID", dto.UrunID);
        command.Parameters.AddWithValue("@UrunAdi", dto.UrunAdi);
        command.Parameters.AddWithValue("@BirimFiyat", dto.BirimFiyat);
        command.Parameters.AddWithValue("@StokAdedi", dto.StokAdedi);
        command.Parameters.AddWithValue("@KritikStok", dto.KritikStokSeviyesi);
        command.ExecuteNonQuery();
        return Results.Ok(new { success = true, message = "Ürün güncellendi." });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { success = false, message = ex.Message });
    }
});


app.MapPatch("/api/products/{id:int}/stock", (int id, int degisim) =>
{
    try
    {
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        var query = @"UPDATE dbo.Urunler 
                      SET StokAdedi = CASE 
                                        WHEN StokAdedi + @degisim < 0 THEN 0 
                                        ELSE StokAdedi + @degisim 
                                      END 
                      WHERE UrunID = @UrunID";
        using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@UrunID", id);
        command.Parameters.AddWithValue("@degisim", degisim);
        command.ExecuteNonQuery();
        return Results.Ok(new { success = true });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { success = false, message = ex.Message });
    }
});


app.MapGet("/api/dashboard", () =>
{
    using var connection = new SqlConnection(connectionString);
    connection.Open();

    var query = @"
        SELECT 
            COUNT(UrunID) AS ToplamUrunCesidi,
            ISNULL(SUM(StokAdedi), 0) AS ToplamStokAdedi,
            ISNULL(SUM(StokAdedi * BirimFiyat), 0) AS ToplamEnvanterDegeri,
            COUNT(CASE WHEN StokAdedi <= KritikStokSeviyesi THEN 1 END) AS KritikUrunSayisi
        FROM dbo.Urunler";

    using var command = new SqlCommand(query, connection);
    using var reader = command.ExecuteReader();

    if (reader.Read())
    {
        return Results.Ok(new
        {
            toplamUrunCesidi = Convert.ToInt32(reader["ToplamUrunCesidi"]),
            toplamStokAdedi = Convert.ToInt32(reader["ToplamStokAdedi"]),
            toplamEnvanterDegeri = Convert.ToDecimal(reader["ToplamEnvanterDegeri"]),
            kritikUrunSayisi = Convert.ToInt32(reader["KritikUrunSayisi"])
        });
    }

    return Results.BadRequest(new { message = "İstatistikler hesaplanamadı." });
});

app.MapGet("/", () => "ERP Envanter API Calisiyor!");

app.Run();

public record UrunEkleDto(string UrunAdi, int KategoriID, int StokAdedi, int KritikStokSeviyesi, decimal BirimFiyat);
public record UrunGuncelleDto(int UrunID, string UrunAdi, decimal BirimFiyat, int StokAdedi, int KritikStokSeviyesi);