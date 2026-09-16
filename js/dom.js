// js/dom.js - Arayüz ve DOM Manipülasyon Servisi
export const domService = {
  // Envanter Tablosunu Ekrana Basma
  tabloyuEkranaBas(liste, onStokDegistir, onDuzenle, onSil) {
    const tabloGovdesi = document.querySelector('#envanterTabloGovdesi');
    const uyariBari = document.querySelector('#kritikStokUyariBari');
    const uyariSayi = document.querySelector('#kritikSayiVurgu');

    if (!tabloGovdesi) return;
    tabloGovdesi.innerHTML = '';

    if (liste.length === 0) {
      tabloGovdesi.innerHTML = `<tr><td colspan="9" align="center" style="color: #6c757d; padding: 20px;">Aramanıza uygun ürün bulunamadı.</td></tr>`;
      if (uyariBari) uyariBari.style.display = 'none';
      return;
    }

    let kritikUrunAdedi = 0;

    liste.forEach(urun => {
      const satir = document.createElement('tr');
      const isKritik = urun.stokAdedi <= urun.kritikStokSeviyesi;

      if (isKritik) {
        kritikUrunAdedi++;
        satir.classList.add('satir-kritik');
      }

      const durumRozeti = isKritik
        ? `<span style="color: #dc3545; font-weight: bold; background: #ffe6e6; padding: 3px 8px; border-radius: 4px; border: 1px solid #f5c2c7;">⚠️ Kritik</span>`
        : `<span style="color: #198754; background: #e8f5e9; padding: 3px 8px; border-radius: 4px;">Yeterli</span>`;

      satir.innerHTML = `
        <td>${urun.urunID}</td>
        <td><strong>${urun.urunAdi}</strong></td>
        <td>${urun.kategoriAdi}</td>
        <td>
          <button class="stok-btn azalat-btn">➖</button>
          <strong style="margin: 0 8px; font-size: 15px; color: ${isKritik ? '#dc3545' : '#212529'};">${urun.stokAdedi}</strong>
          <button class="stok-btn arttir-btn">➕</button>
        </td>
        <td>${durumRozeti}</td>
        <td>${urun.kritikStokSeviyesi}</td>
        <td>${Number(urun.birimFiyat).toLocaleString('tr-TR', { style: 'currency', currency: 'TRY' })}</td>
        <td>${Number(urun.toplamDeger).toLocaleString('tr-TR', { style: 'currency', currency: 'TRY' })}</td>
        <td>
          <button class="btn-duzenle" style="background-color: #ffc107; color: black; border: none; padding: 5px 10px; border-radius: 4px; cursor: pointer; font-weight: bold; margin-right: 5px;">Düzenle</button>
          <button class="btn-sil" style="background-color: #dc3545; color: white; border: none; padding: 5px 10px; border-radius: 4px; cursor: pointer; font-weight: bold;">Sil</button>
        </td>
      `;

      // Olayları güvenle dinleyicilere bağlama
      satir.querySelector('.azalat-btn').onclick = () => onStokDegistir(urun.urunID, -1);
      satir.querySelector('.arttir-btn').onclick = () => onStokDegistir(urun.urunID, 1);
      satir.querySelector('.btn-duzenle').onclick = () => onDuzenle(urun);
      satir.querySelector('.btn-sil').onclick = () => onSil(urun.urunID);

      tabloGovdesi.appendChild(satir);
    });

    // 14. Gün Uyarı Barı Kontrolü
    if (uyariBari && uyariSayi) {
      if (kritikUrunAdedi > 0) {
        uyariSayi.textContent = kritikUrunAdedi;
        uyariBari.style.display = 'flex';
      } else {
        uyariBari.style.display = 'none';
      }
    }
  },

  // Dashboard Sayaçlarını Doldurma
  dashboardGuncelle(data) {
    const toplamCesit = data.toplamUrunCesidi ?? data.ToplamUrunCesidi ?? 0;
    const toplamStok = data.toplamStokAdedi ?? data.ToplamStokAdedi ?? 0;
    const toplamDeger = data.toplamEnvanterDegeri ?? data.ToplamEnvanterDegeri ?? 0;
    const kritikStok = data.kritikUrunSayisi ?? data.KritikUrunSayisi ?? 0;

    document.querySelector('#kartToplamCesit').textContent = toplamCesit;
    document.querySelector('#kartToplamStok').textContent = Number(toplamStok).toLocaleString('tr-TR');
    document.querySelector('#kartToplamDeger').textContent = Number(toplamDeger).toLocaleString('tr-TR', { style: 'currency', currency: 'TRY' });
    document.querySelector('#kartKritikStok').textContent = kritikStok;
  },

  // Kategori Seçim Kutularını Doldurma
  kategorileriDoldur(kategoriler) {
    const formSelect = document.querySelector('#kategoriId');
    const filtreSelect = document.querySelector('#filtreKategori');

    if (formSelect) formSelect.innerHTML = '<option value="">Kategori Seçiniz</option>';
    if (filtreSelect) filtreSelect.innerHTML = '<option value="">Tüm Kategoriler</option>';

    kategoriler.forEach(k => {
      if (formSelect) {
        const opt = document.createElement('option');
        opt.value = k.kategoriID;
        opt.textContent = k.kategoriAdi;
        formSelect.appendChild(opt);
      }
      if (filtreSelect) {
        const opt = document.createElement('option');
        opt.value = k.kategoriAdi;
        opt.textContent = k.kategoriAdi;
        filtreSelect.appendChild(opt);
      }
    });
  },

  // Modal Aç / Kapat
  modalAc(urun) {
    document.querySelector('#editUrunId').value = urun.urunID;
    document.querySelector('#editUrunAdi').value = urun.urunAdi;
    document.querySelector('#editBirimFiyat').value = urun.birimFiyat;
    document.querySelector('#editStokAdedi').value = urun.stokAdedi;
    document.querySelector('#editKritikStok').value = urun.kritikStokSeviyesi;
    document.querySelector('#duzenleModal').style.display = 'block';
  },

  modalKapat() {
    document.querySelector('#duzenleModal').style.display = 'none';
  },

  // 15. Gün: CSV Dosyası İndirme
  csvIndir(urunler) {
    if (!urunler || urunler.length === 0) {
      alert('İndirilecek envanter verisi bulunamadı!');
      return;
    }

    const basliklar = ['Ürün ID', 'Ürün Adı', 'Kategori', 'Stok Adedi', 'Kritik Eşik', 'Birim Fiyat (TL)', 'Toplam Değer (TL)', 'Stok Durumu'];
    const satirlar = urunler.map(u => [
      u.urunID,
      `"${(u.urunAdi || '').replace(/"/g, '""')}"`,
      `"${u.kategoriAdi || ''}"`,
      u.stokAdedi,
      u.kritikStokSeviyesi,
      u.birimFiyat,
      u.toplamDeger,
      `"${u.stokDurumu || ''}"`
    ]);

    const csvIcerik = [basliklar.join(';'), ...satirlar.map(s => s.join(';'))].join('\r\n');
    const blob = new Blob(['\uFEFF' + csvIcerik], { type: 'text/csv;charset=utf-8;' });
    const link = document.createElement('a');
    const url = URL.createObjectURL(blob);
    const bugun = new Date().toISOString().slice(0, 10);

    link.setAttribute('href', url);
    link.setAttribute('download', `Envanter_Raporu_${bugun}.csv`);
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    URL.revokeObjectURL(url);
  }
};