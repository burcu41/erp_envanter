USE EnvanterDB;
GO

-- 1. GÖREV: ENVANTER LİSTESİ SQL VIEW YAPISI

-- Frontend arayüzünde tek bir tablo gibi doğrudan çağrılacak birleştirilmiş görünüm.

IF OBJECT_ID('dbo.vw_EnvanterListesi', 'V') IS NOT NULL
    DROP VIEW dbo.vw_EnvanterListesi;
GO

CREATE VIEW dbo.vw_EnvanterListesi
AS
SELECT 
    u.UrunID,
    u.UrunAdi,
    k.KategoriAdi,
    u.StokAdedi,
    u.KritikStokSeviyesi,
    u.BirimFiyat,
    (u.StokAdedi * u.BirimFiyat) AS ToplamDeger,
    CASE 
        WHEN u.StokAdedi <= u.KritikStokSeviyesi THEN N'Kritik Stok Uyarısı'
        ELSE N'Yeterli'
    END AS StokDurumu,
    u.OlusturmaTarihi
FROM dbo.Urunler u
INNER JOIN dbo.Kategoriler k ON u.KategoriID = k.KategoriID;
GO


-- 2. GÖREV: YENİ ÜRÜN EKLEME STORED PROCEDURE (sp_YeniUrunEkle)

-- Parametreleri alarak veri tabanına kontrollü şekilde yeni donanım kaydeder.

IF OBJECT_ID('dbo.sp_YeniUrunEkle', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_YeniUrunEkle;
GO

CREATE PROCEDURE dbo.sp_YeniUrunEkle
    @UrunAdi NVARCHAR(150),
    @KategoriID INT,
    @StokAdedi INT = 0,
    @KritikStokSeviyesi INT = 5,
    @BirimFiyat DECIMAL(18,2) = 0.00
AS
BEGIN
    SET NOCOUNT ON;

    -- Validasyon: Kategori kontrolü
    IF NOT EXISTS (SELECT 1 FROM dbo.Kategoriler WHERE KategoriID = @KategoriID)
    BEGIN
        RAISERROR(N'Hata: Belirtilen KategoriID mevcut değil.', 16, 1);
        RETURN;
    END

    -- Validasyon: Negatif stok veya fiyat engeli
    IF @StokAdedi < 0 OR @BirimFiyat < 0
    BEGIN
        RAISERROR(N'Hata: Stok adedi veya birim fiyat negatif olamaz.', 16, 1);
        RETURN;
    END

    -- Kayıt Ekleme
    INSERT INTO dbo.Urunler (UrunAdi, KategoriID, StokAdedi, KritikStokSeviyesi, BirimFiyat)
    VALUES (@UrunAdi, @KategoriID, @StokAdedi, @KritikStokSeviyesi, @BirimFiyat);

    SELECT N'Yeni ürün başarıyla envantere eklendi.' AS Mesaj, SCOPE_IDENTITY() AS YeniUrunID;
END;
GO


-- 3. GÖREV: STOK GÜNCELLEME STORED PROCEDURE (sp_StokGuncelle)

-- Verilen Ürün ID'sine göre stok miktarını günceller.

IF OBJECT_ID('dbo.sp_StokGuncelle', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_StokGuncelle;
GO

CREATE PROCEDURE dbo.sp_StokGuncelle
    @UrunID INT,
    @YeniStokAdedi INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Validasyon: Ürün var mı kontrolü
    IF NOT EXISTS (SELECT 1 FROM dbo.Urunler WHERE UrunID = @UrunID)
    BEGIN
        RAISERROR(N'Hata: Güncellenecek ÜrünID bulunamadı.', 16, 1);
        RETURN;
    END

    -- Validasyon: Stok negatif olamaz
    IF @YeniStokAdedi < 0
    BEGIN
        RAISERROR(N'Hata: Stok miktarı 0 dan küçük olamaz.', 16, 1);
        RETURN;
    END

    -- Stok Güncelleme
    UPDATE dbo.Urunler
    SET StokAdedi = @YeniStokAdedi
    WHERE UrunID = @UrunID;

    SELECT N'Stok miktarı başarıyla güncellendi.' AS Mesaj, @UrunID AS GuncellenenUrunID, @YeniStokAdedi AS GuncelStok;
END;
GO

-- 4. ADIM: TEST VE DOĞRULAMA ÇALIŞTIRMALARI

-- Test 1: View üzerinden envanter listesini çekme
SELECT TOP 10 * FROM dbo.vw_EnvanterListesi ORDER BY UrunID DESC;

-- Test 2: Stored Procedure ile Yeni Ürün Ekleme Testi
EXEC dbo.sp_YeniUrunEkle 
    @UrunAdi = N'Test Barkod Okuyucu V2', 
    @KategoriID = 3, 
    @StokAdedi = 15, 
    @KritikStokSeviyesi = 4, 
    @BirimFiyat = 2450.00;

-- Test 3: Stored Procedure ile Stok Güncelleme Testi
EXEC dbo.sp_StokGuncelle 
    @UrunID = 101, 
    @YeniStokAdedi = 25;
GO