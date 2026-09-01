

USE EnvanterDB;
GO

-- 1. ADIM: KATEGORİLERİ PDF DOSYALARINA GÖRE GÜNCELLEME VE SENKRONİZE ETME
DELETE FROM dbo.Urunler;
DELETE FROM dbo.Kategoriler;
DBCC CHECKIDENT ('dbo.Kategoriler', RESEED, 0);
DBCC CHECKIDENT ('dbo.Urunler', RESEED, 100);
GO

INSERT INTO dbo.Kategoriler (KategoriAdi, Aciklama)
VALUES 
    (N'Dokunmatik Ekranlar', N'POS ve Kiosk Tipi Dokunmatik Ekranlar'),
    (N'Yazıcılar', N'Fiş, Barkod ve Mobil Termal Yazıcılar'),
    (N'Barkod Okuyucular ve El Terminalleri', N'Kablolu, Kablosuz Barkod Okuyucular ve El Terminalleri'),
    (N'POS, Ödeme ve Kasa Ekipmanları', N'POS Cihazları, Pinpad, Para Çekmeceleri ve Teraziler'),
    (N'Depolama ve Bellek (SSD / RAM)', N'SSD Diskler ve Bellek Donanımları'),
    (N'Ağ ve İletişim Cihazları', N'Switch, Router, CAT Kablolar ve Ağ Adaptörleri'),
    (N'Çevre Birimleri, Kablo ve Güç', N'Klavye, Mouse, Görüntü Kabloları ve Kesintisiz Güç Kaynakları (UPS)');
GO

-- 2. ADIM: PDF DOSYALARINDAKİ TÜM GERÇEK ENVANTER VERİLERİNİN EKLENMESİ (INSERT)
INSERT INTO dbo.Urunler (UrunAdi, KategoriID, StokAdedi, KritikStokSeviyesi, BirimFiyat)
VALUES 
    
    -- SAYIM DOSYASI 1: DOKUNMATİK EKRANLAR (KategoriID = 1)
    
    (N'Çift Ekran 18.5 inç Dokunmatik POS Ekranı', 1, 2, 3, 5800.00),
    (N'Tek Ekran 18.5 inç Dokunmatik POS Ekranı', 1, 13, 5, 4200.00),
    (N'Çift Ekran 15.6 inç Dokunmatik POS Ekranı', 1, 1, 2, 5100.00),

    
    -- SAYIM DOSYASI 1: YAZICILAR (KategoriID = 2)
    
    (N'Xprinter Termal Fiş Yazıcı', 2, 12, 5, 2400.00),
    (N'Thermal Standart Fiş Yazıcı', 2, 5, 4, 2100.00),
    (N'TSC Barkod Yazıcı', 2, 8, 3, 4900.00),
    (N'Mobile Taşınabilir Mobil Yazıcı', 2, 6, 3, 3600.00),


    -- SAYIM DOSYASI 1: BARKOD OKUYUCULAR & EL TERMİNALLERİ (KategoriID = 3)
    
    (N'Tetikli Kablosuz Barkod Okuyucu', 3, 10, 4, 1850.00),
    (N'Masaüstü Büyük Sabit Barkod Okuyucu', 3, 10, 3, 2900.00),
    (N'Masaüstü Küçük Barkod Okuyucu', 3, 4, 5, 2200.00),
    (N'Masaüstü Yuvarlak Barkod Okuyucu', 3, 4, 5, 2450.00),
    (N'Newland El Terminali', 3, 2, 3, 8500.00),
    (N'Ranger El Terminali', 3, 1, 2, 7900.00),

    
    -- SAYIM DOSYASI 1: POS, ÖDEME VE KASA EKİPMANLARI (KategoriID = 4)
    
    (N'Cash Drawer Para Çekmecesi', 4, 11, 4, 1250.00),
    (N'ER-JR Masaüstü Terazi', 4, 3, 2, 3800.00),
    (N'CL3000 Barkodlu Terazi', 4, 1, 2, 12500.00),
    (N'Pavo Masaüstü POS Cihazı', 4, 3, 2, 6500.00),
    (N'Inpos Dokunmatik POS Cihazı', 4, 3, 2, 6200.00),
    (N'Ingenico Mobil/Masaüstü POS Cihazı', 4, 2, 2, 7100.00),
    (N'Ingenico Harici Pinpad', 4, 3, 2, 2100.00),

    
    -- SAYIM DOSYASI 2: DEPOLAMA VE BELLEK (SSD / RAM) (KategoriID = 5)
    
    (N'SSD Disk 512 GB', 5, 2, 4, 1250.00),
    (N'SSD Disk 480 GB', 5, 1, 3, 1100.00),
    (N'SSD Disk 240 GB', 5, 6, 5, 650.00),
    (N'RAM 2 GB DDR3 (2. El)', 5, 1, 2, 200.00),
    (N'RAM 4 GB DDR3/DDR4', 5, 3, 4, 450.00),
    (N'RAM 8 GB DDR4', 5, 2, 4, 750.00),

    
    -- SAYIM DOSYASI 1 & 2: AĞ VE İLETİŞİM CİHAZLARI (KategoriID = 6)
    
    (N'16 Port Gigabit Switch', 6, 4, 2, 2800.00),
    (N'5 Port Standart Ağ Switch', 6, 1, 2, 450.00),
    (N'TP-Link Kurumsal Router', 6, 5, 3, 1600.00),
    (N'Everest Router', 6, 1, 2, 850.00),
    (N'HP CAT 7 Ağ Kablosu (3 Metre)', 6, 2, 5, 180.00),
    (N'CAT 7 Ağ Kablosu (10 Metre)', 6, 2, 5, 320.00),
    (N'TP-Link USB Ethernet Adaptörü', 6, 2, 3, 350.00),
    (N'USB Wi-Fi Kablosuz Ağ Adaptörü', 6, 3, 3, 280.00),


    -- SAYIM DOSYASI 1 & 2: ÇEVRE BİRİMLERİ, KABLO VE GÜÇ (KategoriID = 7)
    
    (N'Everest Klavye Mouse Seti', 7, 9, 4, 380.00),
    (N'Frisby Klavye Mouse Seti', 7, 1, 3, 320.00),
    (N'EasyUPS 800 Watt Kesintisiz Güç Kaynağı', 7, 10, 3, 2200.00),
    (N'Schneider 1 KW Kesintisiz Güç Kaynağı', 7, 2, 2, 4900.00),
    (N'USB Ses Adaptörü', 7, 3, 2, 120.00),
    (N'S-Link USB Ses Kartı / Adaptör', 7, 1, 2, 160.00),
    (N'Dayton USB Çoklayıcı ve Uzatma Kablosu', 7, 3, 2, 190.00),
    (N'VGA - HDMI Görüntü Dönüştürücü Kablo', 7, 2, 3, 140.00),
    (N'HDMI Standart Görüntü Kablosu', 7, 2, 3, 110.00),
    (N'Display Port Görüntü Kablosu', 7, 1, 2, 150.00),
    (N'S-Link RS232 / VGA-USB Dönüştürücü', 7, 1, 2, 220.00);
GO


-- 3. ADIM: İSTENEN İLERİ DÜZEY T-SQL ANALİZ VE RAPORLAMA SORGULARI


 --SORGU 1: Hangi kategoride kaç adet ürün çeşidi ve toplam kaç adet stok var?
SELECT 
    k.KategoriAdi AS [Kategori Adı],
    COUNT(u.UrunID) AS [Farklı Ürün Çeşidi],
    SUM(u.StokAdedi) AS [Kategorideki Toplam Adet]
FROM dbo.Kategoriler k
INNER JOIN dbo.Urunler u ON k.KategoriID = u.KategoriID
GROUP BY k.KategoriAdi
ORDER BY [Kategorideki Toplam Adet] DESC;
GO

-- SORGU 2: Kategori Bazlı ve Toplam Envanter Değeri (StokAdedi * BirimFiyat)
SELECT 
    k.KategoriAdi AS [Kategori Adı],
    SUM(u.StokAdedi) AS [Toplam Donanım Sayısı],
    FORMAT(SUM(u.StokAdedi * u.BirimFiyat), 'C', 'tr-TR') AS [Kategori Toplam Parasal Değeri]
FROM dbo.Kategoriler k
INNER JOIN dbo.Urunler u ON k.KategoriID = u.KategoriID
GROUP BY k.KategoriAdi
ORDER BY SUM(u.StokAdedi * u.BirimFiyat) DESC;
GO

-- SORGU 3: Genel Depo Özeti (Toplam Ürün Sayısı ve Genel Envanter Maliyet Değeri)
SELECT 
    COUNT(UrunID) AS [Toplam Kalem Sayısı],
    SUM(StokAdedi) AS [Depodaki Toplam Parça Sayısı],
    FORMAT(SUM(StokAdedi * BirimFiyat), 'C', 'tr-TR') AS [Genel Toplam Envanter Değeri]
FROM dbo.Urunler;
GO
-- SORGU 4: Stok Miktarı Kritik Seviyenin Altına Düşen veya Kritik Eşikteki Ürünler(WHERE + INNER JOIN)
SELECT 
    u.UrunID AS [Ürün Kodu],
    u.UrunAdi AS [Donanım Adı],
    k.KategoriAdi AS [Kategori],
    u.StokAdedi AS [Mevcut Stok],
    u.KritikStokSeviyesi AS [Kritik Eşik],
    (u.KritikStokSeviyesi - u.StokAdedi) AS [Acil Sipariş / Eksik Adet],
    FORMAT(u.BirimFiyat, 'C', 'tr-TR') AS [Birim Fiyat]
FROM dbo.Urunler u
INNER JOIN dbo.Kategoriler k ON u.KategoriID = k.KategoriID
WHERE u.StokAdedi <= u.KritikStokSeviyesi
ORDER BY u.StokAdedi ASC;
GO