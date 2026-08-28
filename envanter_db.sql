USE EnvanterDB;
GO


IF OBJECT_ID('dbo.Urunler', 'U') IS NOT NULL
    DROP TABLE dbo.Urunler;
GO


IF OBJECT_ID('dbo.Kategoriler', 'U') IS NOT NULL
    DROP TABLE dbo.Kategoriler;
GO


CREATE TABLE dbo.Kategoriler (
    KategoriID INT IDENTITY(1,1) NOT NULL,
    KategoriAdi NVARCHAR(100) NOT NULL,
    Aciklama NVARCHAR(250) NULL,
    CONSTRAINT PK_Kategoriler PRIMARY KEY CLUSTERED (KategoriID)
);
GO


CREATE TABLE dbo.Urunler (
    UrunID INT IDENTITY(101,1) NOT NULL,
    UrunAdi NVARCHAR(150) NOT NULL,
    KategoriID INT NOT NULL,
    StokAdedi INT NOT NULL DEFAULT 0,
    KritikStokSeviyesi INT NOT NULL DEFAULT 5,
    BirimFiyat DECIMAL(18,2) NULL DEFAULT 0.00,
    OlusturmaTarihi DATETIME NOT NULL DEFAULT GETDATE(),
    
    CONSTRAINT PK_Urunler PRIMARY KEY CLUSTERED (UrunID),
    CONSTRAINT FK_Urunler_Kategoriler FOREIGN KEY (KategoriID)
        REFERENCES dbo.Kategoriler (KategoriID)
        ON UPDATE CASCADE
        ON DELETE NO ACTION,
    CONSTRAINT CHK_Urunler_StokAdedi CHECK (StokAdedi >= 0),
    CONSTRAINT CHK_Urunler_KritikStok CHECK (KritikStokSeviyesi >= 0)
);
GO


INSERT INTO dbo.Kategoriler (KategoriAdi, Aciklama)
VALUES 
    (N'Dokunmatik Ekranlar', N'Endüstriyel POS ve Monitör Ekranları'),
    (N'Yazıcılar', N'Barkod, Termal Fiş ve Ağ Yazıcıları'),
    (N'Barkod Okuyucular', N'El Terminalleri, 1D/2D Karekod Okuyucular'),
    (N'POS Cihazları', N'Masaüstü ve Mobil Satış Noktası Cihazları'),
    (N'Depolama ve Bellek', N'SSD, HDD ve RAM Donanımları'),
    (N'Ağ ve Donanım', N'Switch, Router ve Altyapı Cihazları');
GO


SELECT * FROM dbo.Kategoriler;