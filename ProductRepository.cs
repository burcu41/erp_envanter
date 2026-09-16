using System.Data;
using Microsoft.Data.SqlClient;

namespace ErpApi;

public class ProductRepository
{
    private readonly string _connectionString;

    public ProductRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    // 1. Tüm Envanteri Getir (View Üzerinden)
    public List<dynamic> GetProducts()
    {
        var list = new List<dynamic>();
        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        var query = "SELECT * FROM dbo.vw_EnvanterListesi ORDER BY UrunID ASC";
        using var command = new SqlCommand(query, connection);
        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            list.Add(new
            {
                UrunID = reader["UrunID"],
                UrunAdi = reader["UrunAdi"],
                KategoriAdi = reader["KategoriAdi"],
                BirimFiyat = reader["BirimFiyat"],
                StokAdedi = reader["StokAdedi"],
                KritikStokSeviyesi = reader["KritikStokSeviyesi"],
                ToplamDeger = reader["ToplamDeger"],
                StokDurumu = reader["StokDurumu"]
            });
        }
        return list;
    }

    // 2. Kategorileri Getir
    public List<dynamic> GetCategories()
    {
        var list = new List<dynamic>();
        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        using var command = new SqlCommand("SELECT KategoriID, KategoriAdi FROM dbo.Kategoriler ORDER BY KategoriAdi ASC", connection);
        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            list.Add(new
            {
                KategoriID = reader["KategoriID"],
                KategoriAdi = reader["KategoriAdi"]
            });
        }
        return list;
    }

    // 3. Dashboard İstatistiklerini Getir (SQL Aggregation)
    public dynamic GetDashboardStats()
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        var query = @"SELECT 
                        COUNT(*) AS ToplamUrunCesidi,
                        ISNULL(SUM(StokAdedi), 0) AS ToplamStokAdedi,
                        ISNULL(SUM(ToplamDeger), 0) AS ToplamEnvanterDegeri,
                        ISNULL(SUM(CASE WHEN StokAdedi <= KritikStokSeviyesi THEN 1 ELSE 0 END), 0) AS KritikUrunSayisi
                      FROM dbo.vw_EnvanterListesi";

        using var command = new SqlCommand(query, connection);
        using var reader = command.ExecuteReader();

        if (reader.Read())
        {
            return new
            {
                ToplamUrunCesidi = reader["ToplamUrunCesidi"],
                ToplamStokAdedi = reader["ToplamStokAdedi"],
                ToplamEnvanterDegeri = reader["ToplamEnvanterDegeri"],
                KritikUrunSayisi = reader["KritikUrunSayisi"]
            };
        }
        return new { ToplamUrunCesidi = 0, ToplamStokAdedi = 0, ToplamEnvanterDegeri = 0, KritikUrunSayisi = 0 };
    }

    // 4. Stored Procedure ile Yeni Ürün Ekle
    public void AddProduct(UrunEkleDto dto)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        using var command = new SqlCommand("dbo.sp_YeniUrunEkle", connection);
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.AddWithValue("@UrunAdi", dto.UrunAdi.Trim());
        command.Parameters.AddWithValue("@KategoriID", dto.KategoriID);
        command.Parameters.AddWithValue("@StokAdedi", dto.StokAdedi);
        command.Parameters.AddWithValue("@KritikStokSeviyesi", dto.KritikStokSeviyesi);
        command.Parameters.AddWithValue("@BirimFiyat", dto.BirimFiyat);
        command.ExecuteNonQuery();
    }

    // 5. Ürün Bilgilerini Güncelle (Parametreli Sorgu)
    public bool UpdateProduct(UrunGuncelleDto dto)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        var query = @"UPDATE dbo.Urunler 
                      SET UrunAdi = @UrunAdi, BirimFiyat = @BirimFiyat, 
                          StokAdedi = @StokAdedi, KritikStokSeviyesi = @KritikStok 
                      WHERE UrunID = @UrunID";
        using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@UrunID", dto.UrunID);
        command.Parameters.AddWithValue("@UrunAdi", dto.UrunAdi.Trim());
        command.Parameters.AddWithValue("@BirimFiyat", dto.BirimFiyat);
        command.Parameters.AddWithValue("@StokAdedi", dto.StokAdedi);
        command.Parameters.AddWithValue("@KritikStok", dto.KritikStokSeviyesi);
        return command.ExecuteNonQuery() > 0;
    }

    // 6. Hızlı Stok Değiştir (+ / -)
    public bool UpdateStockQuick(int id, int degisim)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        var query = @"UPDATE dbo.Urunler 
                      SET StokAdedi = CASE WHEN StokAdedi + @degisim < 0 THEN 0 ELSE StokAdedi + @degisim END 
                      WHERE UrunID = @id";
        using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@id", id);
        command.Parameters.AddWithValue("@degisim", degisim);
        return command.ExecuteNonQuery() > 0;
    }

    // 7. Ürün Sil
    public bool DeleteProduct(int id)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        using var command = new SqlCommand("DELETE FROM dbo.Urunler WHERE UrunID = @id", connection);
        command.Parameters.AddWithValue("@id", id);
        return command.ExecuteNonQuery() > 0;
    }
}