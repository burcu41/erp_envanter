// js/api.js - Sunucu API İstekleri Servisi
const API_BASE = 'http://localhost:5134/api';

export const apiService = {
  // Tüm ürünleri çekme
  async envanteriGetir() {
    const res = await fetch(`${API_BASE}/products`);
    return await res.json();
  },

  // Kategorileri çekme
  async kategorileriGetir() {
    const res = await fetch(`${API_BASE}/categories`);
    return await res.json();
  },

  // Dashboard istatistiklerini çekme
  async dashboardVerileriniGetir() {
    const res = await fetch(`${API_BASE}/dashboard`);
    return await res.json();
  },

  // Yeni ürün ekleme
  async urunEkle(urun) {
    const res = await fetch(`${API_BASE}/products`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(urun)
    });
    return await res.json();
  },

  // Ürün güncelleme
  async urunGuncelle(urun) {
    const res = await fetch(`${API_BASE}/products`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(urun)
    });
    return await res.json();
  },

  // Hızlı stok (+ / -) güncelleme
  async stokDegistir(id, degisim) {
    const res = await fetch(`${API_BASE}/products/${id}/stock?degisim=${degisim}`, {
      method: 'PATCH'
    });
    return await res.json();
  },

  // Ürün silme
  async urunSil(id) {
    const res = await fetch(`${API_BASE}/products/${id}`, {
      method: 'DELETE'
    });
    return await res.json();
  }
};