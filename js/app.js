// js/app.js - Uygulama Giriş Noktası ve Akış Yönetimi
import { apiService } from './api.js';
import { domService } from './dom.js';

let tumUrunler = [];

// Tüm ekranı ve sayaçları güncelleyen yardımcı fonksiyon
async function sayfayiYenile() {
  try {
    tumUrunler = await apiService.envanteriGetir();
    domService.tabloyuEkranaBas(tumUrunler, hizliStokHandler, duzenleHandler, silHandler);
    
    const dashboardData = await apiService.dashboardVerileriniGetir();
    domService.dashboardGuncelle(dashboardData);
  } catch (err) {
    console.error('Veri yenileme hatası:', err);
  }
}

// Filtreleme mantığı
function filtreUygula() {
  const aramaMetni = document.querySelector('#aramaKutusu').value.toLowerCase().trim();
  const secilenKategori = document.querySelector('#filtreKategori').value;

  const filtrelenmis = tumUrunler.filter(urun => {
    const isimUyuyor = urun.urunAdi.toLowerCase().includes(aramaMetni);
    const kategoriUyuyor = secilenKategori === '' || urun.kategoriAdi === secilenKategori;
    return isimUyuyor && kategoriUyuyor;
  });

  domService.tabloyuEkranaBas(filtrelenmis, hizliStokHandler, duzenleHandler, silHandler);
}

// Olay Fonksiyonları (Handlers)
async function hizliStokHandler(id, degisim) {
  await apiService.stokDegistir(id, degisim);
  await sayfayiYenile();
  filtreUygula();
}

function duzenleHandler(urun) {
  domService.modalAc(urun);
}

async function silHandler(id) {
  if (!confirm(`${id} ID'li ürünü silmek istediğinize emin misiniz?`)) return;
  await apiService.urunSil(id);
  await sayfayiYenile();
  filtreUygula();
}

// Sayfa Yüklendiğinde Olay Dinleyicilerini Bağlama
document.addEventListener('DOMContentLoaded', async () => {
  await sayfayiYenile();
  
  const kategoriler = await apiService.kategorileriGetir();
  domService.kategorileriDoldur(kategoriler);

  // Arama ve Kategori Filtresi
  document.querySelector('#aramaKutusu').addEventListener('input', filtreUygula);
  document.querySelector('#filtreKategori').addEventListener('change', filtreUygula);
  
  // Filtreleri Temizle Butonu
  const btnTemizle = document.querySelector('#btnFiltreTemizle');
  if (btnTemizle) {
    btnTemizle.addEventListener('click', () => {
      document.querySelector('#aramaKutusu').value = '';
      document.querySelector('#filtreKategori').value = '';
      domService.tabloyuEkranaBas(tumUrunler, hizliStokHandler, duzenleHandler, silHandler);
    });
  }

  // CSV İndir Butonu
  const btnCsv = document.querySelector('#btnCsvIndir');
  if (btnCsv) {
    btnCsv.addEventListener('click', () => {
      domService.csvIndir(tumUrunler);
    });
  }

  // Modal İptal Butonu
  const btnIptal = document.querySelector('#btnModalIptal');
  if (btnIptal) {
    btnIptal.addEventListener('click', domService.modalKapat);
  }

  // Modal Güncelleme Formu (Validasyonlu)
  document.querySelector('#modalDuzenleFormu').addEventListener('submit', async (e) => {
    e.preventDefault();
    const dto = {
      urunID: parseInt(document.querySelector('#editUrunId').value),
      urunAdi: document.querySelector('#editUrunAdi').value.trim(),
      birimFiyat: parseFloat(document.querySelector('#editBirimFiyat').value),
      stokAdedi: parseInt(document.querySelector('#editStokAdedi').value),
      kritikStokSeviyesi: parseInt(document.querySelector('#editKritikStok').value)
    };

    if (dto.urunAdi.length < 2 || dto.birimFiyat <= 0 || dto.stokAdedi < 0 || dto.kritikStokSeviyesi < 1) {
      alert('Lütfen geçerli değerler giriniz!');
      return;
    }

    const res = await apiService.urunGuncelle(dto);
    if (res.success) {
      alert('Ürün başarıyla güncellendi!');
      domService.modalKapat();
      await sayfayiYenile();
      filtreUygula();
    } else {
      alert('Hata: ' + res.message);
    }
  });

  // Yeni Ürün Ekleme Formu (Validasyonlu)
  document.querySelector('#yeniUrunFormu').addEventListener('submit', async (e) => {
    e.preventDefault();
    const dto = {
      urunAdi: document.querySelector('#urunAdi').value.trim(),
      kategoriID: parseInt(document.querySelector('#kategoriId').value),
      birimFiyat: parseFloat(document.querySelector('#birimFiyat').value),
      stokAdedi: parseInt(document.querySelector('#stokAdedi').value),
      kritikStokSeviyesi: parseInt(document.querySelector('#kritikStok').value)
    };

    if (dto.urunAdi.length < 2 || dto.kategoriID <= 0 || dto.birimFiyat <= 0 || dto.stokAdedi < 0 || dto.kritikStokSeviyesi < 1) {
      alert('Lütfen tüm alanları kurallara uygun doldurunuz!');
      return;
    }

    const res = await apiService.urunEkle(dto);
    if (res.success) {
      alert('Ürün başarıyla eklendi!');
      document.querySelector('#yeniUrunFormu').reset();
      await sayfayiYenile();
      filtreUygula();
    } else {
      alert('Hata: ' + res.message);
    }
  });
});