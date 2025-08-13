# Paint in 3D Dokümantasyonu

Bu dokümantasyon, Paint in 3D Unity paketinin kapsamlı kullanım kılavuzudur. Özellikle first-person karavan boyama projeleri için özelleştirilmiştir.

## 📚 İçindekiler

### 🚀 Başlangıç Rehberi
- [Paket Tanıtımı](baslangic/paket-tanitimi.md) - Paket özellikleri ve avantajları
- [Kurulum Rehberi](baslangic/kurulum-rehberi.md) - Adım adım kurulum
- [Hızlı Başlangıç](baslangic/hizli-baslangic.md) - 5 dakikada ilk boyama

### 🔧 Temel Bileşenler
- [PaintCore Altyapısı](temel-bilesenler/paintcore.md) - Temel sistem bileşenleri
- [PaintIn3D Özellikleri](temel-bilesenler/paintin3d.md) - 3D boyama özellikleri
- [Boyama Araçları](temel-bilesenler/boyama-araclari.md) - Tüm boyama araçları

### ⚡ Gelişmiş Özellikler
- [Boyama Türleri](gelismis-ozellikler/boyama-turleri.md) - Farklı boyama teknikleri
- [Hit Detection](gelismis-ozellikler/hit-detection.md) - Vuruş algılama sistemleri
- [Blend Modes](gelismis-ozellikler/blend-modes.md) - Karışım modları
- [Performans Optimizasyonu](gelismis-ozellikler/performans-optimizasyonu.md) - Performans iyileştirmeleri
- [Gelişmiş Özellikler](gelismis-ozellikler/gelismis-ozellikler.md) - İleri seviye özellikler

### 🔍 Sorun Giderme
- [Genel Sorunlar ve Çözümleri](sorun-giderme/genel-sorunlar.md) - Yaygın sorunlar ve çözümleri

### 📖 API Referansı
- [API Referansı](api/api-referansi.md) - Kapsamlı API dokümantasyonu

### 💡 Örnekler
- [Temel Örnekler](ornekler/temel-ornekler.md) - Pratik kod örnekleri

### 🚐 Karavan Projesi Özel
- [Karavan Projesi Özel](karavan-projesi/karavan-projesi-ozel.md) - Proje spesifik dokümantasyon

### 📋 Sonuç ve Özet
- [Sonuç ve Özet](sonuc-ozet.md) - Genel değerlendirme ve öneriler

## 🎯 Hızlı Başlangıç

### 5 Adımda İlk Boyama
1. **📦 Paket Kurulumu**: [Kurulum Rehberi](baslangic/kurulum-rehberi.md) bölümünü takip edin
2. **🎨 İlk Boyama**: [Hızlı Başlangıç](baslangic/hizli-baslangic.md) ile ilk boyamanızı yapın
3. **🔧 Temel Bileşenler**: [Temel Bileşenler](temel-bilesenler/) bölümünü inceleyin
4. **💻 Örnekler**: [Örnekler](ornekler/) bölümündeki kodları deneyin
5. **🚐 Karavan Projesi**: [Karavan Projesi Özel](karavan-projesi/) bölümünü inceleyin

### Öğrenme Yol Haritası
- **Başlangıç (1-2 Hafta)**: Temel kurulum ve basit boyama
- **Orta Seviye (2-4 Hafta)**: Gelişmiş araçlar ve optimizasyon
- **İleri Seviye (1-2 Ay)**: Custom shader ve proje entegrasyonu

## 🎨 Paket Hakkında

Paint in 3D, Unity için geliştirilmiş güçlü bir 3D boyama sistemidir. Bu paket ile:

### ✨ Temel Özellikler
- **Gerçek Zamanlı 3D Boyama**: Anında görsel geri bildirim
- **Çoklu Boyama Araçları**: Sphere, Decal, Line, Fill, Box, Stamp
- **Gelişmiş Blend Modları**: AlphaBlend, Additive, Subtractive, Multiply, Normal
- **Yüksek Performans**: GPU optimize edilmiş sistem
- **Platform Bağımsız**: Unity Built-in, URP, HDRP desteği

### 🚐 Karavan Projesi İçin Özel Avantajlar
- **First-Person Boyama**: Crosshair tabanlı boyama sistemi
- **Kaydetme Sistemi**: Boyama sonuçlarını saklama
- **UI Entegrasyonu**: Boyama araçları arayüzü
- **Performans Optimizasyonu**: LOD ve culling sistemleri

## 📊 Dokümantasyon İstatistikleri

- **📄 Toplam Sayfa**: 16 ana bölüm
- **💻 Kod Örnekleri**: 100+ kod bloğu
- **📚 Kapsanan Konular**: 50+ ana konu
- **🔧 Troubleshooting**: 20+ yaygın sorun
- **📖 API Referansı**: 30+ sınıf ve metod

## 🎯 Önerilen Okuma Sırası

1. **[Başlangıç Rehberi](baslangic/)** - Temel kavramları öğrenin
2. **[Temel Bileşenler](temel-bilesenler/)** - Core sistemleri anlayın
3. **[Örnekler](ornekler/)** - Pratik yapın
4. **[Gelişmiş Özellikler](gelismis-ozellikler/)** - İleri seviye teknikleri öğrenin
5. **[Karavan Projesi Özel](karavan-projesi/)** - Projenize odaklanın
6. **[API Referansı](api/)** - Detaylı bilgi alın
7. **[Sorun Giderme](sorun-giderme/)** - Problemleri çözün

## 🔧 Hızlı Referans

### Temel Bileşenler
```csharp
// Manager
CwPaintableManager manager = CwPaintableManager.GetOrCreateInstance();

// Texture
CwPaintableTexture texture = GetComponent<CwPaintableTexture>();
texture.SetResolution(1024, 1024);

// Paint Tool
CwPaintSphere paintSphere = GetComponent<CwPaintSphere>();
paintSphere.Color = Color.red;
paintSphere.Radius = 0.1f;
```

### Karavan Projesi İçin
```csharp
// First-person boyama kontrolcüsü
CaravanPaintController controller = GetComponent<CaravanPaintController>();

// UI sistemi
CaravanPaintUI ui = GetComponent<CaravanPaintUI>();

// Kaydetme sistemi
CaravanPaintSaveSystem saveSystem = GetComponent<CaravanPaintSaveSystem>();
```

## 🆘 Destek

### Sorun Giderme
- **[Genel Sorunlar](sorun-giderme/genel-sorunlar.md)** - Yaygın sorunlar ve çözümleri
- **[API Referansı](api/api-referansi.md)** - Detaylı API dokümantasyonu
- **[Örnekler](ornekler/temel-ornekler.md)** - Pratik kod örnekleri

### Performans İpuçları
- Texture boyutlarını optimize edin (1024x1024 ideal)
- LOD sistemi kullanın
- Culling implementasyonu yapın
- Batch processing kullanın
- Memory management yapın

## 📈 Gelecek Geliştirmeler

### Kısa Vadeli (1-3 Ay)
- Mobile Optimization
- UI Improvements
- Tutorial System

### Orta Vadeli (3-6 Ay)
- Advanced Effects

---

*Bu dokümantasyon, Paint in 3D paketinin kapsamlı analizi sonucunda hazırlanmıştır. Karavan projesi için özelleştirilmiş örnekler ve çözümler içerir.*
