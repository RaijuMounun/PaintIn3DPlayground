# 4. PaintCore Altyapısı

## 🎯 Genel Bakış

**PaintCore**, Paint in 3D paketinin temel altyapısını oluşturan çekirdek sistemdir. Bu sistem, tüm boyama işlemlerinin yönetimi, texture işlemleri, bellek yönetimi ve performans optimizasyonu gibi kritik görevleri üstlenir.

### 📋 PaintCore'un Görevleri
- **🎨 Boyama Yönetimi**: Tüm boyama komutlarını toplar ve işler
- **🖼️ Texture İşlemleri**: Boyanabilir texture'ları yönetir
- **💾 Bellek Yönetimi**: GPU ve CPU belleğini optimize eder
- **⚡ Performans**: Batch processing ve command pooling
- **🔄 Senkronizasyon**: CPU ve GPU arasında veri senkronizasyonu

---

## 🏗️ Sistem Mimarisi

### Hiyerarşik Yapı
```
PaintCore/
├── CwPaintableManager     # Ana yönetici
├── CwPaintableTexture     # Texture yönetimi
├── CwModel               # Model temsili
├── CwBlendMode           # Karışım modları
├── CwCommand             # Boyama komutları
└── CwSerialization       # Veri serileştirme
```

### Veri Akışı
```
Input → CwHitScreen → CwPaintSphere → CwCommand → CwPaintableManager → GPU → Texture
```

---

## 🎯 CwPaintableManager

### 📖 Genel Bilgiler
**Ne işe yarar?** Tüm boyama işlemlerini yöneten ana bileşen  
**Nereye eklenir?** Sahneye bir kez (genellikle boş GameObject)  
**Zorunlu mu?** Evet, boyama sistemi çalışması için gerekli  
**Performans etkisi:** Düşük, sadece yönetim yapar

### 🔧 Temel Ayarları

#### ReadPixelsBudget
```csharp
// GPU performans ayarı - her frame'de işlenecek maksimum pixel sayısı
manager.ReadPixelsBudget = 1000; // Düşük performanslı sistemler için
manager.ReadPixelsBudget = 5000; // Orta performanslı sistemler için
manager.ReadPixelsBudget = 10000; // Yüksek performanslı sistemler için
```

**Önerilen Değerler:**
- **Mobil**: 500-1000
- **PC (Orta)**: 2000-5000
- **PC (Yüksek)**: 5000-10000
- **VR**: 1000-2000

#### Events
```csharp
// Boyama başladığında tetiklenir
manager.OnBeginPainting += () => Debug.Log("Boyama başladı!");

// Boyama bittiğinde tetiklenir
manager.OnEndPainting += () => Debug.Log("Boyama bitti!");

// Her frame'de tetiklenir
manager.OnUpdate += () => Debug.Log("Frame güncellendi!");
```

### 🛠️ Temel Fonksiyonları

#### SubmitAll()
```csharp
// Tüm bekleyen boyama komutlarını GPU'ya gönderir
manager.SubmitAll();

// Manuel olarak çağrılabilir (genellikle otomatik)
void Update()
{
    if (Input.GetMouseButton(0)) // Mouse basılı tutulduğunda
    {
        manager.SubmitAll(); // Her frame'de gönder
    }
}
```

#### GetOrCreateInstance()
```csharp
// Singleton pattern - tek instance oluşturur
var manager = CwPaintableManager.GetOrCreateInstance();

// Eğer yoksa oluşturur, varsa mevcut olanı döner
if (manager == null)
{
    Debug.LogError("PaintableManager oluşturulamadı!");
}
```

#### ClearAll()
```csharp
// Tüm boyama verilerini temizler
manager.ClearAll();

// Sadece belirli grupları temizler
manager.ClearAll("Group1");
```

### 📊 Performans Optimizasyonu

#### Batch Processing
```csharp
// Toplu işleme ayarları
manager.BatchSize = 100; // Her batch'te işlenecek komut sayısı
manager.MaxBatches = 10; // Maksimum batch sayısı
```

#### Memory Pooling
```csharp
// Bellek havuzlama ayarları
manager.PoolSize = 1000; // Havuz boyutu
manager.EnablePooling = true; // Havuzlamayı etkinleştir
```

### 🚨 Yaygın Sorunlar

#### Sorun 1: Birden Fazla Instance
**Belirtiler:**
```
Multiple PaintableManager instances found!
```

**Çözüm:**
```csharp
// Sahneyi kontrol et ve fazla olanları sil
var managers = FindObjectsOfType<CwPaintableManager>();
if (managers.Length > 1)
{
    for (int i = 1; i < managers.Length; i++)
    {
        DestroyImmediate(managers[i].gameObject);
    }
}
```

#### Sorun 2: ReadPixelsBudget Çok Düşük
**Belirtiler:** Boyama gecikmeli, FPS düşük
**Çözüm:** ReadPixelsBudget değerini artırın

#### Sorun 3: Memory Leak
**Belirtiler:** Bellek sürekli artıyor
**Çözüm:**
```csharp
void OnDestroy()
{
    // Temizlik yap
    if (manager != null)
    {
        manager.ClearAll();
    }
}
```

---

## 🖼️ CwPaintableTexture

### 📖 Genel Bilgiler
**Ne işe yarar?** Boyanabilir texture'ları temsil eder  
**Nereye eklenir?** Boyanacak objenin child'ına  
**Zorunlu mu?** Evet, boyama için gerekli  
**Performans etkisi:** Orta, texture işlemleri yapar

### 🔧 Temel Ayarları

#### Slot Ayarları
```csharp
// Hangi material ve texture slot'u kullanılacak
paintableTexture.Slot = new CwSlot(CwTextureType.Albedo, 0);

// Farklı texture türleri
paintableTexture.Slot = new CwSlot(CwTextureType.Albedo, 0);     // Ana renk
paintableTexture.Slot = new CwSlot(CwTextureType.Normal, 0);     // Normal map
paintableTexture.Slot = new CwSlot(CwTextureType.Metallic, 0);   // Metallic
paintableTexture.Slot = new CwSlot(CwTextureType.Smoothness, 0); // Smoothness
```

#### UV Koordinatları
```csharp
// UV koordinat sistemi
paintableTexture.Coord = CwCoord.First;  // İlk UV seti
paintableTexture.Coord = CwCoord.Second; // İkinci UV seti
paintableTexture.Coord = CwCoord.Third;  // Üçüncü UV seti
```

#### Grup Ayarları
```csharp
// Boyama grubu - aynı gruptaki texture'lar birlikte boyanır
paintableTexture.Group = "Body";     // Gövde grubu
paintableTexture.Group = "Wheels";    // Tekerlek grubu
paintableTexture.Group = "Windows";   // Pencere grubu
```

### 🛠️ Temel Fonksiyonları

#### AddCommand()
```csharp
// Boyama komutu ekler
paintableTexture.AddCommand(command);

// Manuel komut oluşturma
var command = new CwCommandSphere();
command.Color = Color.red;
command.Radius = 0.1f;
paintableTexture.AddCommand(command);
```

#### Save() / Load()
```csharp
// Boyama verilerini kaydet
paintableTexture.Save("MyPaintData");

// Boyama verilerini yükle
paintableTexture.Load("MyPaintData");

// Otomatik kaydetme
paintableTexture.AutoSave = true;
paintableTexture.AutoSaveInterval = 5f; // 5 saniyede bir
```

#### Undo() / Redo()
```csharp
// Son işlemi geri al
if (paintableTexture.CanUndo)
{
    paintableTexture.Undo();
}

// Son işlemi tekrarla
if (paintableTexture.CanRedo)
{
    paintableTexture.Redo();
}

// Undo/Redo durumunu kontrol et
Debug.Log($"Undo: {paintableTexture.CanUndo}, Redo: {paintableTexture.CanRedo}");
```

#### Clear()
```csharp
// Tüm boyamaları temizle
paintableTexture.Clear();

// Belirli rengi temizle
paintableTexture.Clear(Color.red);

// Belirli alanı temizle
paintableTexture.Clear(area);
```

### 📊 Gelişmiş Ayarları

#### Texture Boyutu
```csharp
// Texture boyutunu ayarla
paintableTexture.Size = new Vector2Int(1024, 1024); // 1024x1024
paintableTexture.Size = new Vector2Int(2048, 2048); // 2048x2048
paintableTexture.Size = new Vector2Int(4096, 4096); // 4096x4096
```

#### Mip Maps
```csharp
// Mip map ayarları
paintableTexture.GenerateMipMaps = true;  // Mip map oluştur
paintableTexture.MipMapCount = 4;         // Mip map sayısı
```

#### Compression
```csharp
// Sıkıştırma ayarları
paintableTexture.Compression = TextureCompression.DXT1; // DXT1 sıkıştırma
paintableTexture.Compression = TextureCompression.DXT5; // DXT5 sıkıştırma
paintableTexture.Compression = TextureCompression.None; // Sıkıştırma yok
```

### 🚨 Yaygın Sorunlar

#### Sorun 1: Yanlış Slot Seçimi
**Belirtiler:** Boyama görünmüyor
**Çözüm:** Material'da doğru texture slot'unu kontrol edin

#### Sorun 2: UV Koordinatları Uyumsuzluğu
**Belirtiler:** Boyama yanlış yerde görünüyor
**Çözüm:** UV mapping'i kontrol edin

#### Sorun 3: Memory Leak (Çok Fazla Undo State)
**Belirtiler:** Bellek sürekli artıyor
**Çözüm:**
```csharp
// Undo state sayısını sınırla
paintableTexture.MaxUndoStates = 50; // Maksimum 50 undo state
```

---

## 🎨 CwModel (Soyut Sınıf)

### 📖 Genel Bilgiler
**Ne işe yarar?** Boyanabilir 3D modelleri temsil eder  
**Nereye eklenir?** Doğrudan kullanılmaz, alt sınıflar kullanılır  
**Alt sınıfları:** CwMeshModel, CwPaintableMesh  
**Performans etkisi:** Düşük, sadece referans tutar

### 🔧 Alt Sınıfları

#### CwMeshModel
```csharp
// Mesh tabanlı model
public class CwMeshModel : CwModel
{
    public Mesh Mesh;           // Mesh referansı
    public Material Material;   // Material referansı
    public Transform Transform; // Transform referansı
}
```

#### CwPaintableMesh
```csharp
// Boyanabilir mesh
public class CwPaintableMesh : CwModel
{
    public CwPaintableTexture[] PaintableTextures; // Boyanabilir texture'lar
    public CwModel.ActivationType Activation;      // Aktivasyon türü
}
```

### 🛠️ Temel Fonksiyonları

#### FindPaintableTextures()
```csharp
// Boyanabilir texture'ları bulur
var textures = model.FindPaintableTextures();

// Belirli gruptaki texture'ları bulur
var bodyTextures = model.FindPaintableTextures("Body");
```

#### GetHash()
```csharp
// Model hash'i - performans için kullanılır
int hash = model.GetHash();
```

#### Activate() / Deactivate()
```csharp
// Modeli aktifleştir/deaktifleştir
model.Activate();
model.Deactivate();

// Aktivasyon türü
model.Activation = CwModel.ActivationType.Manual;  // Manuel
model.Activation = CwModel.ActivationType.OnStart; // Start'ta otomatik
```

---

## 🎨 CwBlendMode

### 📖 Genel Bilgiler
**Ne işe yarar?** Farklı karışım modlarını destekler  
**Kullanım yerleri:** Boyama bileşenlerinde  
**Performans etkisi:** Düşük, shader seviyesinde işlenir

### 🔧 Desteklenen Modlar

#### AlphaBlend
```csharp
// Normal karışım - en yaygın kullanılan
paintSphere.BlendMode = CwBlendMode.AlphaBlend;
```
**Kullanım:** Normal boyama, renk karışımı

#### Additive
```csharp
// Toplama karışımı - parlak efektler için
paintSphere.BlendMode = CwBlendMode.Additive;
```
**Kullanım:** Parlak efektler, ışık efektleri

#### Subtractive
```csharp
// Çıkarma karışımı - karanlık efektler için
paintSphere.BlendMode = CwBlendMode.Subtractive;
```
**Kullanım:** Karanlık efektler, gölge efektleri

#### Multiply
```csharp
// Çarpma karışımı - koyu efektler için
paintSphere.BlendMode = CwBlendMode.Multiply;
```
**Kullanım:** Koyu efektler, doku karışımı

#### Normal
```csharp
// Normal map karışımı - yüzey detayları için
paintSphere.BlendMode = CwBlendMode.Normal;
```
**Kullanım:** Normal map boyama, yüzey detayları

### 🛠️ Temel Fonksiyonları

#### Apply()
```csharp
// Material'a karışım modunu uygula
blendMode.Apply(material);

// Shader'a karışım modunu uygula
blendMode.Apply(shader);
```

#### GetBlendFunction()
```csharp
// Karışım fonksiyonunu al
var blendFunc = blendMode.GetBlendFunction();
```

---

## 📊 Performans İpuçları

### GPU Optimizasyonu
```csharp
// ReadPixelsBudget'ı sistem performansına göre ayarla
manager.ReadPixelsBudget = SystemInfo.graphicsMemorySize > 4000 ? 5000 : 1000;

// Batch size'ı artır
manager.BatchSize = 200; // Daha fazla komut bir arada işlensin
```

### Bellek Yönetimi
```csharp
// Texture boyutunu küçült
paintableTexture.Size = new Vector2Int(1024, 1024); // 2048 yerine 1024

// Undo state sayısını sınırla
paintableTexture.MaxUndoStates = 30; // Bellek tasarrufu için
```

### Senkronizasyon
```csharp
// Manuel senkronizasyon
void LateUpdate()
{
    if (isPainting)
    {
        manager.SubmitAll(); // Her frame'de gönder
    }
}
```

---

## 🎯 Karavan Projesi İçin Özel Ayarlar

### Büyük Objeler İçin Optimizasyon
```csharp
// Karavan boyutunda objeler için
manager.ReadPixelsBudget = 8000;        // Yüksek performans
manager.BatchSize = 300;                // Büyük batch'ler
paintableTexture.Size = new Vector2Int(2048, 2048); // Yüksek çözünürlük
```

### Çoklu Texture Desteği
```csharp
// Karavan için farklı bölgeler
var bodyTexture = paintableMesh.AddPaintableTexture("Body");
var wheelTexture = paintableMesh.AddPaintableTexture("Wheels");
var windowTexture = paintableMesh.AddPaintableTexture("Windows");
```

### First-Person Optimizasyonu
```csharp
// First-person için performans ayarları
manager.ReadPixelsBudget = 5000;        // Orta performans
paintableTexture.Size = new Vector2Int(1024, 1024); // Orta çözünürlük
paintableTexture.MaxUndoStates = 20;    // Sınırlı undo
```

---

## ✅ Kontrol Listesi

### Temel Kurulum
- [ ] CwPaintableManager sahneye eklendi
- [ ] ReadPixelsBudget ayarlandı
- [ ] Events bağlandı
- [ ] Performans test edildi

### Texture Ayarları
- [ ] CwPaintableTexture doğru slot'a bağlandı
- [ ] UV koordinatları kontrol edildi
- [ ] Grup ayarları yapıldı
- [ ] Texture boyutu optimize edildi

### Performans
- [ ] FPS kabul edilebilir seviyede
- [ ] Bellek kullanımı normal
- [ ] GPU kullanımı optimize edildi
- [ ] Batch processing çalışıyor

---

*Önceki: [3. Hızlı Başlangıç](baslangic/hizli-baslangic.md) | Sonraki: [5. PaintIn3D Özellikleri](paintin3d.md)*
