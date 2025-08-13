# Paint in 3D Paketi - Detaylı Dokümantasyon Planı

## 📋 Genel Bakış
Bu dokümantasyon, Paint in 3D paketinin tam kapsamlı kullanım kılavuzudur. Karavan boyama projesi için optimize edilmiş, sorun giderme ve performans optimizasyonu dahil her detayı içerir.

---

## 🗂️ Dokümantasyon Yapısı

### 1. BAŞLANGIÇ REHBERİ
#### 1.1 Paket Tanıtımı
- [ ] Paket nedir ve ne işe yarar?
- [ ] Hangi projeler için uygundur?
- [ ] Sistem gereksinimleri
- [ ] Unity versiyonu uyumluluğu
- [ ] Render Pipeline uyumluluğu (Built-in, URP, HDRP)

#### 1.2 Kurulum Rehberi
- [ ] Paket import etme
- [ ] Temel kurulum adımları
- [ ] İlk sahne kurulumu
- [ ] Gerekli bileşenlerin eklenmesi
- [ ] Test sahnesi oluşturma

#### 1.3 Hızlı Başlangıç
- [ ] 5 dakikada ilk boyama
- [ ] Temel bileşenlerin tanıtımı
- [ ] Basit boyama örneği
- [ ] Yaygın hatalar ve çözümleri

---

### 2. TEMEL BİLEŞENLER REHBERİ

#### 2.1 PaintCore (Temel Altyapı)

##### 2.1.1 CwPaintableManager
- [ ] **Ne işe yarar?** Tüm boyama işlemlerini yöneten ana bileşen
- [ ] **Nereye eklenir?** Sahneye bir kez eklenir (genellikle boş GameObject)
- [ ] **Zorunlu mu?** Evet, boyama sistemi çalışması için gerekli
- [ ] **Ayarları:**
  - ReadPixelsBudget: GPU performans ayarı
  - OnBeginPainting event
- [ ] **Fonksiyonları:**
  - SubmitAll(): Boyama komutlarını gönderir
  - GetOrCreateInstance(): Instance oluşturur
- [ ] **Performans etkisi:** Düşük, sadece yönetim yapar
- [ ] **Yaygın sorunlar:**
  - Birden fazla instance olması
  - ReadPixelsBudget çok düşük olması

##### 2.1.2 CwPaintableTexture
- [ ] **Ne işe yarar?** Boyanabilir texture'ları temsil eder
- [ ] **Nereye eklenir?** Boyanacak objenin child'ına
- [ ] **Zorunlu mu?** Evet, boyama için gerekli
- [ ] **Ayarları:**
  - Slot: Hangi material ve texture slot'u
  - Coord: UV koordinatları
  - Group: Boyama grubu
  - UndoRedo: Geri alma sistemi
  - SaveLoad: Kaydetme sistemi
- [ ] **Fonksiyonları:**
  - AddCommand(): Boyama komutu ekler
  - Save(): Kaydetme
  - Load(): Yükleme
  - Undo(): Geri alma
  - Redo(): Tekrarlama
- [ ] **Yaygın sorunlar:**
  - Yanlış slot seçimi
  - UV koordinatları uyumsuzluğu
  - Memory leak (çok fazla undo state)

##### 2.1.3 CwModel (Soyut Sınıf)
- [ ] **Ne işe yarar?** Boyanabilir 3D modelleri temsil eder
- [ ] **Nereye eklenir?** Doğrudan kullanılmaz, alt sınıflar kullanılır
- [ ] **Alt sınıfları:**
  - CwMeshModel
  - CwPaintableMesh
- [ ] **Fonksiyonları:**
  - FindPaintableTextures(): Boyanabilir texture'ları bulur
  - GetHash(): Model hash'i

##### 2.1.4 CwBlendMode
- [ ] **Ne işe yarar?** Farklı karışım modlarını destekler
- [ ] **Kullanım yerleri:** Boyama bileşenlerinde
- [ ] **Desteklenen modlar:**
  - AlphaBlend: Normal karışım
  - Additive: Toplama
  - Subtractive: Çıkarma
  - Multiply: Çarpma
  - Normal: Normal map
- [ ] **Fonksiyonları:**
  - AlphaBlend(): Alpha karışımı
  - Additive(): Toplama karışımı
  - Apply(): Material'a uygular

#### 2.2 PaintIn3D (3D Boyama Özellikleri)

##### 2.2.1 CwPaintableMesh
- [ ] **Ne işe yarar?** Mesh tabanlı boyanabilir objeler
- [ ] **Nereye eklenir?** Boyanacak mesh'in GameObject'ine
- [ ] **Zorunlu bileşenler:** MeshFilter, MeshRenderer veya SkinnedMeshRenderer
- [ ] **Ayarları:**
  - Activation: Aktivasyon zamanı
  - MaterialApplication: Material uygulama yöntemi
  - OtherRenderers: Diğer renderer'lar
- [ ] **Fonksiyonları:**
  - Activate(): Aktifleştirir
  - Deactivate(): Deaktifleştirir
  - RemoveComponents(): Bileşenleri kaldırır
- [ ] **Yaygın sorunlar:**
  - Mesh yok
  - Material yok
  - UV koordinatları eksik

##### 2.2.2 CwPaintSphere
- [ ] **Ne işe yarar?** Küre şeklinde boyama
- [ ] **Nereye eklenir?** Boyama aracının GameObject'ine
- [ ] **Ayarları:**
  - Color: Boyama rengi
  - Radius: Boyama yarıçapı
  - Opacity: Şeffaflık
  - Hardness: Sertlik
  - BlendMode: Karışım modu
  - Scale: Ölçek (elips için)
- [ ] **Fonksiyonları:**
  - HandleHitPoint(): Nokta boyama
  - HandleHitLine(): Çizgi boyama
  - HandleHitTriangle(): Üçgen boyama
  - HandleHitQuad(): Dörtgen boyama
- [ ] **Yaygın sorunlar:**
  - Radius çok büyük/küçük
  - Opacity 0
  - Yanlış layer mask

##### 2.2.3 CwHitCollisions
- [ ] **Ne işe yarar?** Çarpışma tabanlı boyama
- [ ] **Nereye eklenir?** Boyama aracının GameObject'ine
- [ ] **Zorunlu bileşenler:** Rigidbody
- [ ] **Ayarları:**
  - Emit: Çarpışma türü (PointsIn3D, PointsOnUV, TrianglesIn3D)
  - Layers: Hangi layer'lar
  - Threshold: Çarpışma eşiği
  - PressureMode: Basınç modu
- [ ] **Fonksiyonları:**
  - OnCollisionEnter(): Çarpışma tespiti
  - EmitHit(): Hit gönderir
- [ ] **Yaygın sorunlar:**
  - Rigidbody yok
  - Collider yok
  - Layer mask yanlış

##### 2.2.4 CwHitScreen
- [ ] **Ne işe yarar?** Ekran tabanlı boyama (mouse/touch)
- [ ] **Nereye eklenir?** Boyama aracının GameObject'ine
- [ ] **Ayarları:**
  - Camera: Hangi kamera
  - Layers: Hangi layer'lar
  - Distance: Maksimum mesafe
- [ ] **Fonksiyonları:**
  - HandleHit(): Hit işler
  - Raycast(): Işın gönderir
- [ ] **Yaygın sorunlar:**
  - Camera null
  - Mesh collider yok
  - Distance çok kısa

---

### 3. GELİŞMİŞ ÖZELLİKLER

#### 3.1 Boyama Türleri
- [ ] **Sphere Painting:** Küre boyama
- [ ] **Decal Painting:** Dekal boyama
- [ ] **Line Painting:** Çizgi boyama
- [ ] **Fill Painting:** Dolgu boyama
- [ ] **Triangle Painting:** Üçgen boyama
- [ ] **Quad Painting:** Dörtgen boyama

#### 3.2 Karışım Modları
- [ ] **Alpha Blend:** Normal karışım
- [ ] **Additive:** Toplama
- [ ] **Subtractive:** Çıkarma
- [ ] **Multiply:** Çarpma
- [ ] **Normal Map:** Normal harita
- [ ] **Custom Blend:** Özel karışım

#### 3.3 Maskeleme Sistemi
- [ ] **CwMask:** Maskeleme bileşeni
- [ ] **Mask Texture:** Maske texture'ı
- [ ] **Mask Channel:** Maske kanalı
- [ ] **Mask Invert:** Maske tersleme

#### 3.4 Undo/Redo Sistemi
- [ ] **State Management:** Durum yönetimi
- [ ] **Memory Optimization:** Bellek optimizasyonu
- [ ] **Performance Impact:** Performans etkisi

#### 3.5 Save/Load Sistemi
- [ ] **Manual Save:** Manuel kaydetme
- [ ] **Automatic Save:** Otomatik kaydetme
- [ ] **File Format:** Dosya formatı
- [ ] **Compression:** Sıkıştırma

---

### 4. PERFORMANS OPTİMİZASYONU

#### 4.1 Texture Optimizasyonu
- [ ] **Texture Size:** Texture boyutu
- [ ] **Mip Maps:** Mip haritaları
- [ ] **Compression:** Sıkıştırma
- [ ] **Memory Management:** Bellek yönetimi

#### 4.2 Rendering Optimizasyonu
- [ ] **Batch Processing:** Toplu işleme
- [ ] **Command Pooling:** Komut havuzlama
- [ ] **LOD System:** Seviye detay sistemi
- [ ] **Culling:** Görünürlük kontrolü

#### 4.3 GPU Optimizasyonu
- [ ] **Shader Optimization:** Shader optimizasyonu
- [ ] **Draw Calls:** Çizim çağrıları
- [ ] **Memory Bandwidth:** Bellek bant genişliği
- [ ] **Async Operations:** Asenkron işlemler

---

### 5. SORUN GİDERME REHBERİ

#### 5.1 Yaygın Hatalar
- [ ] **"No PaintableManager found"** Hatası
  - **Sebep:** CwPaintableManager yok
  - **Çözüm:** Sahneye ekle
- [ ] **"No PaintableTexture found"** Hatası
  - **Sebep:** CwPaintableTexture yok
  - **Çözüm:** Objeye ekle
- [ ] **"No collision detected"** Hatası
  - **Sebep:** Collider yok
  - **Çözüm:** Collider ekle
- [ ] **"Texture not updating"** Hatası
  - **Sebep:** Yanlış slot
  - **Çözüm:** Slot kontrol et

#### 5.2 Performans Sorunları
- [ ] **FPS Drop:** FPS düşüşü
  - **Sebep:** Çok fazla boyama
  - **Çözüm:** Batch size artır
- [ ] **Memory Leak:** Bellek sızıntısı
  - **Sebep:** Texture'lar temizlenmiyor
  - **Çözüm:** Destroy'da temizle
- [ ] **GPU Overload:** GPU aşırı yükü
  - **Sebep:** Çok büyük texture'lar
  - **Çözüm:** Texture boyutunu küçült

#### 5.3 Render Pipeline Sorunları
- [ ] **URP Compatibility:** URP uyumluluğu
- [ ] **HDRP Compatibility:** HDRP uyumluluğu
- [ ] **Shader Errors:** Shader hataları
- [ ] **Material Issues:** Material sorunları

#### 5.4 Platform Sorunları
- [ ] **Mobile Performance:** Mobil performans
- [ ] **VR Compatibility:** VR uyumluluğu
- [ ] **Build Issues:** Build sorunları
- [ ] **Platform Specific:** Platform özel sorunlar

---

### 6. KARAVAN BOYAMA PROJESİ ÖZEL REHBERİ

#### 6.1 Proje Kurulumu
- [ ] **Karavan Modeli Hazırlama:** UV mapping
- [ ] **Material Setup:** Material kurulumu
- [ ] **Paintable Components:** Boyanabilir bileşenler
- [ ] **Paint Tools:** Boyama araçları

#### 6.2 First-Person Boyama
- [ ] **Camera Setup:** Kamera kurulumu
- [ ] **Input System:** Giriş sistemi
- [ ] **Paint Controls:** Boyama kontrolleri
- [ ] **UI Integration:** UI entegrasyonu

#### 6.3 Boyama Araçları
- [ ] **Brush System:** Fırça sistemi
- [ ] **Color Picker:** Renk seçici
- [ ] **Tool Selection:** Araç seçimi
- [ ] **Settings Panel:** Ayarlar paneli

#### 6.4 Save System
- [ ] **Progress Save:** İlerleme kaydetme
- [ ] **Multiple Saves:** Çoklu kayıt
- [ ] **Export System:** Dışa aktarma
- [ ] **Cloud Save:** Bulut kaydetme

---

### 7. API REFERANSI

#### 7.1 Core Classes
- [ ] **CwPaintableManager API**
- [ ] **CwPaintableTexture API**
- [ ] **CwModel API**
- [ ] **CwBlendMode API**

#### 7.2 Paint Classes
- [ ] **CwPaintableMesh API**
- [ ] **CwPaintSphere API**
- [ ] **CwHitCollisions API**
- [ ] **CwHitScreen API**

#### 7.3 Command Classes
- [ ] **CwCommand API**
- [ ] **CwCommandSphere API**
- [ ] **CwCommandDecal API**

#### 7.4 Utility Classes
- [ ] **CwCommon API**
- [ ] **CwHelper API**
- [ ] **CwSerialization API**

---

### 8. ÖRNEKLER VE TUTORİALLER

#### 8.1 Temel Örnekler
- [ ] **Basic Painting:** Temel boyama
- [ ] **Color Changing:** Renk değiştirme
- [ ] **Brush Size:** Fırça boyutu
- [ ] **Blend Modes:** Karışım modları

#### 8.2 Gelişmiş Örnekler
- [ ] **Multi-Texture Painting:** Çoklu texture boyama
- [ ] **Normal Map Painting:** Normal harita boyama
- [ ] **Decal System:** Dekal sistemi
- [ ] **Masking System:** Maskeleme sistemi

#### 8.3 Proje Örnekleri
- [ ] **Car Painting:** Araba boyama
- [ ] **Wall Painting:** Duvar boyama
- [ ] **Object Customization:** Obje özelleştirme
- [ ] **Interactive Art:** Etkileşimli sanat

---

### 9. BEST PRACTICES

#### 9.1 Kod Yazma
- [ ] **Component Organization:** Bileşen organizasyonu
- [ ] **Memory Management:** Bellek yönetimi
- [ ] **Performance Optimization:** Performans optimizasyonu
- [ ] **Error Handling:** Hata yönetimi

#### 9.2 Asset Management
- [ ] **Texture Organization:** Texture organizasyonu
- [ ] **Material Setup:** Material kurulumu
- [ ] **Prefab Creation:** Prefab oluşturma
- [ ] **Scene Organization:** Sahne organizasyonu

#### 9.3 User Experience
- [ ] **Intuitive Controls:** Sezgisel kontroller
- [ ] **Visual Feedback:** Görsel geri bildirim
- [ ] **Performance Indicators:** Performans göstergeleri
- [ ] **Error Messages:** Hata mesajları

---

### 10. TROUBLESHOOTING MATRIX

#### 10.1 Hata Matrisi
- [ ] **Hata Türü → Olası Sebepler → Çözümler**
- [ ] **Performans Sorunları → Çözümler**
- [ ] **Render Sorunları → Çözümler**
- [ ] **Platform Sorunları → Çözümler**

#### 10.2 Debug Araçları
- [ ] **Debug Logging:** Debug loglama
- [ ] **Performance Profiling:** Performans profilleme
- [ ] **Memory Profiling:** Bellek profilleme
- [ ] **Visual Debugging:** Görsel debug

---

### 11. SIK SORULAN SORULAR (SSS)

#### 11.1 Genel Sorular
- [ ] **Paket ücretsiz mi?**
- [ ] **Hangi Unity versiyonları desteklenir?**
- [ ] **Mobil platformlarda çalışır mı?**
- [ ] **VR/AR desteği var mı?**

#### 11.2 Teknik Sorular
- [ ] **Maksimum texture boyutu nedir?**
- [ ] **Kaç obje aynı anda boyanabilir?**
- [ ] **Undo/Redo limiti nedir?**
- [ ] **Save dosya boyutu ne kadar?**

#### 11.3 Performans Soruları
- [ ] **Minimum sistem gereksinimleri?**
- [ ] **Mobil cihazlarda performans?**
- [ ] **VR'da performans?**
- [ ] **Büyük projelerde ölçeklenebilirlik?**

---

### 12. GÜNCELLEME NOTLARI

#### 12.1 Versiyon Geçmişi
- [ ] **v1.0.0:** İlk sürüm
- [ ] **v1.1.0:** Yeni özellikler
- [ ] **v1.2.0:** Performans iyileştirmeleri
- [ ] **v1.3.0:** Bug fixes

#### 12.2 Breaking Changes
- [ ] **API Değişiklikleri**
- [ ] **Deprecated Features**
- [ ] **Migration Guide**

---

### 13. KAYNAKLAR VE REFERANSLAR

#### 13.1 Dış Kaynaklar
- [ ] **Unity Documentation**
- [ ] **Shader Documentation**
- [ ] **Performance Guides**
- [ ] **Community Forums**

#### 13.2 Video Tutorials
- [ ] **Setup Tutorials**
- [ ] **Advanced Features**
- [ ] **Troubleshooting**
- [ ] **Project Examples**

---

### 14. İLETİŞİM VE DESTEK

#### 14.1 Destek Kanalları
- [ ] **Email Support**
- [ ] **Community Forums**
- [ ] **Discord Server**
- [ ] **GitHub Issues**

#### 14.2 Katkıda Bulunma
- [ ] **Bug Reports**
- [ ] **Feature Requests**
- [ ] **Code Contributions**
- [ ] **Documentation Updates**

---

## 📝 Dokümantasyon Yazım Kuralları

### Format Standartları
- **Markdown** formatı kullanılacak
- **Kod blokları** syntax highlighting ile
- **Görseller** ekran görüntüleri ile
- **Video** tutorial'lar için linkler

### İçerik Standartları
- **Pratik örnekler** her konu için
- **Kod örnekleri** tam çalışır durumda
- **Sorun giderme** adım adım
- **Performans ipuçları** her bölümde

### Güncelleme Planı
- **Haftalık** içerik güncellemesi
- **Aylık** büyük revizyon
- **Versiyon** güncellemeleri ile senkronize

---

## 🎯 Öncelik Sırası

### Faz 1 (Hafta 1-2)
1. Başlangıç Rehberi
2. Temel Bileşenler (Core)
3. Basit Örnekler

### Faz 2 (Hafta 3-4)
1. Gelişmiş Özellikler
2. Performans Optimizasyonu
3. Sorun Giderme

### Faz 3 (Hafta 5-6)
1. Karavan Projesi Özel Rehberi
2. API Referansı
3. Best Practices

### Faz 4 (Hafta 7-8)
1. Troubleshooting Matrix
2. SSS
3. Final Review ve Düzenleme

---

Bu plan, Paint in 3D paketinin tam kapsamlı dokümantasyonu için hazırlanmıştır. Her bölüm detaylı olarak yazılacak ve sürekli güncellenecektir.
