# 5. PaintIn3D Özellikleri

## 🎯 Genel Bakış

**PaintIn3D**, Paint in 3D paketinin 3D boyama özelliklerini sağlayan ana modülüdür. Bu modül, 3D objeler üzerinde gerçek zamanlı boyama yapmak için gerekli tüm bileşenleri içerir.

### 📋 PaintIn3D'un Özellikleri
- **🎨 3D Boyama**: Mesh tabanlı 3D objelerde boyama
- **🖌️ Boyama Araçları**: Küre, dekal, çizgi boyama araçları
- **🎯 Hit Detection**: Çarpışma ve ekran tabanlı boyama
- **🔄 Çoklu Platform**: Mouse, touch, VR controller desteği
- **⚡ Performans**: GPU optimizasyonu ile hızlı boyama

---

## 🏗️ Sistem Mimarisi

### Bileşen Hiyerarşisi
```
PaintIn3D/
├── CwPaintableMesh        # Boyanabilir mesh
├── CwPaintSphere          # Küre boyama aracı
├── CwPaintDecal           # Dekal boyama aracı
├── CwPaintLine            # Çizgi boyama aracı
├── CwHitCollisions        # Çarpışma tabanlı boyama
├── CwHitScreen            # Ekran tabanlı boyama
└── CwHitRaycast           # Işın tabanlı boyama
```

### Boyama Akışı
```
Input → Hit Detection → Paint Tool → Command → PaintableMesh → Texture Update
```

---

## 🎨 CwPaintableMesh

### 📖 Genel Bilgiler
**Ne işe yarar?** Mesh tabanlı boyanabilir objeler  
**Nereye eklenir?** Boyanacak mesh'in GameObject'ine  
**Zorunlu bileşenler:** MeshFilter, MeshRenderer veya SkinnedMeshRenderer  
**Performans etkisi:** Orta, mesh işlemleri yapar

### 🔧 Temel Ayarları

#### Activation (Aktivasyon)
```csharp
// Aktivasyon zamanı
paintableMesh.Activation = CwModel.ActivationType.Manual;   // Manuel aktivasyon
paintableMesh.Activation = CwModel.ActivationType.OnStart;  // Start'ta otomatik
paintableMesh.Activation = CwModel.ActivationType.OnEnable; // Enable'da otomatik
```

#### Material Application (Material Uygulama)
```csharp
// Material uygulama yöntemi
paintableMesh.MaterialApplication = CwModel.MaterialApplicationType.Individual; // Her mesh için ayrı
paintableMesh.MaterialApplication = CwModel.MaterialApplicationType.Shared;     // Paylaşılan material
```

#### Other Renderers (Diğer Renderer'lar)
```csharp
// Ek renderer'lar ekle
paintableMesh.OtherRenderers = new Renderer[] { renderer1, renderer2 };
```

### 🛠️ Temel Fonksiyonları

#### Activate() / Deactivate()
```csharp
// Mesh'i aktifleştir/deaktifleştir
paintableMesh.Activate();
paintableMesh.Deactivate();

// Durumu kontrol et
if (paintableMesh.IsActive)
{
    Debug.Log("Mesh aktif!");
}
```

#### RemoveComponents()
```csharp
// Bileşenleri kaldır
paintableMesh.RemoveComponents();

// Sadece belirli bileşenleri kaldır
paintableMesh.RemoveComponents(typeof(CwPaintableTexture));
```

#### GetPaintableTextures()
```csharp
// Boyanabilir texture'ları al
var textures = paintableMesh.GetPaintableTextures();

// Belirli gruptaki texture'ları al
var bodyTextures = paintableMesh.GetPaintableTextures("Body");
```

### 📊 Gelişmiş Ayarları

#### UV Mapping
```csharp
// UV koordinat sistemi
paintableMesh.UVChannel = 0; // İlk UV seti
paintableMesh.UVChannel = 1; // İkinci UV seti
paintableMesh.UVChannel = 2; // Üçüncü UV seti
```

#### Texture Settings
```csharp
// Texture ayarları
paintableMesh.TextureSize = new Vector2Int(1024, 1024);
paintableMesh.GenerateMipMaps = true;
paintableMesh.Compression = TextureCompression.DXT1;
```

#### Performance Settings
```csharp
// Performans ayarları
paintableMesh.BatchSize = 100;
paintableMesh.MaxCommands = 1000;
paintableMesh.EnableCulling = true;
```

### 🚨 Yaygın Sorunlar

#### Sorun 1: Mesh Yok
**Belirtiler:** "No mesh found" hatası
**Çözüm:** MeshFilter bileşeninin var olduğunu kontrol edin

#### Sorun 2: Material Yok
**Belirtiler:** "No material found" hatası
**Çözüm:** MeshRenderer'da material atandığını kontrol edin

#### Sorun 3: UV Koordinatları Eksik
**Belirtiler:** Boyama görünmüyor
**Çözüm:** Mesh'in UV koordinatlarının var olduğunu kontrol edin

---

## 🖌️ CwPaintSphere

### 📖 Genel Bilgiler
**Ne işe yarar?** Küre şeklinde boyama aracı  
**Nereye eklenir?** Boyama aracının GameObject'ine  
**Zorunlu bileşenler:** Yok, bağımsız çalışır  
**Performans etkisi:** Düşük, sadece parametre tutar

### 🔧 Temel Ayarları

#### Color (Renk)
```csharp
// Boyama rengi
paintSphere.Color = Color.red;           // Kırmızı
paintSphere.Color = Color.blue;          // Mavi
paintSphere.Color = Color.green;         // Yeşil
paintSphere.Color = new Color(1, 0, 0, 1); // RGBA ile
```

#### Radius (Yarıçap)
```csharp
// Boyama yarıçapı
paintSphere.Radius = 0.1f;  // Küçük boyama
paintSphere.Radius = 0.5f;  // Orta boyama
paintSphere.Radius = 1.0f;  // Büyük boyama
```

#### Opacity (Şeffaflık)
```csharp
// Boyama şeffaflığı
paintSphere.Opacity = 0.0f; // Tamamen şeffaf
paintSphere.Opacity = 0.5f; // Yarı şeffaf
paintSphere.Opacity = 1.0f; // Tamamen opak
```

#### Hardness (Sertlik)
```csharp
// Boyama sertliği (kenar yumuşaklığı)
paintSphere.Hardness = 0.0f; // Çok yumuşak kenarlar
paintSphere.Hardness = 0.5f; // Orta sertlik
paintSphere.Hardness = 1.0f; // Çok sert kenarlar
```

#### BlendMode (Karışım Modu)
```csharp
// Karışım modu
paintSphere.BlendMode = CwBlendMode.AlphaBlend;  // Normal karışım
paintSphere.BlendMode = CwBlendMode.Additive;    // Toplama
paintSphere.BlendMode = CwBlendMode.Multiply;    // Çarpma
paintSphere.BlendMode = CwBlendMode.Normal;      // Normal map
```

#### Scale (Ölçek)
```csharp
// Boyama ölçeği (elips için)
paintSphere.Scale = Vector3.one;        // Daire
paintSphere.Scale = new Vector3(2, 1, 1); // Yatay elips
paintSphere.Scale = new Vector3(1, 2, 1); // Dikey elips
```

### 🛠️ Temel Fonksiyonları

#### HandleHitPoint()
```csharp
// Nokta boyama
paintSphere.HandleHitPoint(hitPoint, pressure);

// Örnek kullanım
void OnMouseDown()
{
    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    if (Physics.Raycast(ray, out RaycastHit hit))
    {
        paintSphere.HandleHitPoint(hit.point, 1.0f);
    }
}
```

#### HandleHitLine()
```csharp
// Çizgi boyama
paintSphere.HandleHitLine(startPoint, endPoint, pressure);

// Örnek kullanım
void OnMouseDrag()
{
    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    if (Physics.Raycast(ray, out RaycastHit hit))
    {
        paintSphere.HandleHitLine(lastHitPoint, hit.point, 1.0f);
        lastHitPoint = hit.point;
    }
}
```

#### HandleHitTriangle()
```csharp
// Üçgen boyama
paintSphere.HandleHitTriangle(point1, point2, point3, pressure);
```

#### HandleHitQuad()
```csharp
// Dörtgen boyama
paintSphere.HandleHitQuad(point1, point2, point3, point4, pressure);
```

### 📊 Gelişmiş Ayarları

#### Pressure Sensitivity
```csharp
// Basınç duyarlılığı
paintSphere.PressureMode = CwPressureMode.None;      // Basınç yok
paintSphere.PressureMode = CwPressureMode.Radius;    // Yarıçap değişir
paintSphere.PressureMode = CwPressureMode.Opacity;   // Şeffaflık değişir
paintSphere.PressureMode = CwPressureMode.Both;      // Her ikisi de değişir
```

#### Falloff
```csharp
// Düşüş eğrisi
paintSphere.Falloff = CwFalloff.Linear;     // Doğrusal
paintSphere.Falloff = CwFalloff.Smooth;     // Yumuşak
paintSphere.Falloff = CwFalloff.Sphere;     // Küresel
```

#### Tiling
```csharp
// Döşeme ayarları
paintSphere.Tiling = Vector2.one;           // Tek döşeme
paintSphere.Tiling = new Vector2(2, 2);     // 2x2 döşeme
```

### 🚨 Yaygın Sorunlar

#### Sorun 1: Radius Çok Büyük/Küçük
**Belirtiler:** Boyama çok büyük veya çok küçük
**Çözüm:** Radius değerini ayarlayın (0.01f - 10f arası)

#### Sorun 2: Opacity 0
**Belirtiler:** Boyama görünmüyor
**Çözüm:** Opacity değerini 0'dan büyük yapın

#### Sorun 3: Yanlış Layer Mask
**Belirtiler:** Boyama yanlış objelerde görünüyor
**Çözüm:** CwHitScreen'in Layers ayarını kontrol edin

---

## 🎯 CwHitCollisions

### 📖 Genel Bilgiler
**Ne işe yarar?** Çarpışma tabanlı boyama  
**Nereye eklenir?** Boyama aracının GameObject'ine  
**Zorunlu bileşenler:** Rigidbody  
**Performans etkisi:** Orta, fizik hesaplamaları yapar

### 🔧 Temel Ayarları

#### Emit (Çarpışma Türü)
```csharp
// Çarpışma türü
hitCollisions.Emit = CwHitCollisions.EmitType.PointsIn3D;    // 3D noktalar
hitCollisions.Emit = CwHitCollisions.EmitType.PointsOnUV;    // UV noktaları
hitCollisions.Emit = CwHitCollisions.EmitType.TrianglesIn3D; // 3D üçgenler
```

#### Layers (Layer'lar)
```csharp
// Hangi layer'lar ile çarpışacak
hitCollisions.Layers = LayerMask.GetMask("Default");
hitCollisions.Layers = LayerMask.GetMask("Paintable");
hitCollisions.Layers = LayerMask.GetMask("Default", "Paintable");
```

#### Threshold (Eşik)
```csharp
// Çarpışma eşiği
hitCollisions.Threshold = 0.1f; // Düşük hassasiyet
hitCollisions.Threshold = 0.5f; // Orta hassasiyet
hitCollisions.Threshold = 1.0f; // Yüksek hassasiyet
```

#### Pressure Mode (Basınç Modu)
```csharp
// Basınç modu
hitCollisions.PressureMode = CwPressureMode.None;      // Basınç yok
hitCollisions.PressureMode = CwPressureMode.Radius;    // Yarıçap değişir
hitCollisions.PressureMode = CwPressureMode.Opacity;   // Şeffaflık değişir
```

### 🛠️ Temel Fonksiyonları

#### OnCollisionEnter()
```csharp
// Çarpışma tespiti
void OnCollisionEnter(Collision collision)
{
    hitCollisions.OnCollisionEnter(collision);
}

// Özel çarpışma işleme
void OnCollisionEnter(Collision collision)
{
    if (collision.gameObject.layer == LayerMask.NameToLayer("Paintable"))
    {
        hitCollisions.OnCollisionEnter(collision);
    }
}
```

#### EmitHit()
```csharp
// Manuel hit gönder
hitCollisions.EmitHit(hitPoint, hitNormal, pressure);

// Örnek kullanım
void Update()
{
    if (Input.GetMouseButton(0))
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            hitCollisions.EmitHit(hit.point, hit.normal, 1.0f);
        }
    }
}
```

### 📊 Gelişmiş Ayarları

#### Collision Detection
```csharp
// Çarpışma tespiti ayarları
hitCollisions.UseCollision = true;        // OnCollisionEnter kullan
hitCollisions.UseTrigger = false;         // OnTriggerEnter kullanma
hitCollisions.UseRaycast = false;         // Raycast kullanma
```

#### Performance Settings
```csharp
// Performans ayarları
hitCollisions.MaxHitsPerFrame = 100;      // Frame başına maksimum hit
hitCollisions.MinDistance = 0.01f;        // Minimum mesafe
hitCollisions.MaxDistance = 10f;          // Maksimum mesafe
```

### 🚨 Yaygın Sorunlar

#### Sorun 1: Rigidbody Yok
**Belirtiler:** Çarpışma tespit edilmiyor
**Çözüm:** GameObject'e Rigidbody ekleyin

#### Sorun 2: Collider Yok
**Belirtiler:** Çarpışma tespit edilmiyor
**Çözüm:** Hedef objelerde Collider olduğunu kontrol edin

#### Sorun 3: Layer Mask Yanlış
**Belirtiler:** Yanlış objelerde boyama oluyor
**Çözüm:** Layers ayarını kontrol edin

---

## 🖱️ CwHitScreen

### 📖 Genel Bilgiler
**Ne işe yarar?** Ekran tabanlı boyama (mouse/touch)  
**Nereye eklenir?** Boyama aracının GameObject'ine  
**Zorunlu bileşenler:** Yok, bağımsız çalışır  
**Performans etkisi:** Düşük, sadece raycast yapar

### 🔧 Temel Ayarları

#### Camera (Kamera)
```csharp
// Hangi kamera kullanılacak
hitScreen.Camera = Camera.main;           // Ana kamera
hitScreen.Camera = Camera.current;        // Mevcut kamera
hitScreen.Camera = FindObjectOfType<Camera>(); // İlk bulunan kamera
```

#### Layers (Layer'lar)
```csharp
// Hangi layer'lar ile çarpışacak
hitScreen.Layers = LayerMask.GetMask("Default");
hitScreen.Layers = LayerMask.GetMask("Paintable");
hitScreen.Layers = LayerMask.GetMask("Default", "Paintable");
```

#### Distance (Mesafe)
```csharp
// Maksimum raycast mesafesi
hitScreen.Distance = 10f;   // Kısa mesafe
hitScreen.Distance = 100f;  // Orta mesafe
hitScreen.Distance = 1000f; // Uzun mesafe
```

### 🛠️ Temel Fonksiyonları

#### HandleHit()
```csharp
// Hit işle
hitScreen.HandleHit();

// Manuel hit işleme
void Update()
{
    if (Input.GetMouseButton(0))
    {
        hitScreen.HandleHit();
    }
}
```

#### Raycast()
```csharp
// Manuel raycast
RaycastHit hit;
if (hitScreen.Raycast(Input.mousePosition, out hit))
{
    // Hit noktasında boyama yap
    paintSphere.HandleHitPoint(hit.point, 1.0f);
}
```

#### GetHitPoint()
```csharp
// Hit noktasını al
Vector3 hitPoint = hitScreen.GetHitPoint(Input.mousePosition);
if (hitPoint != Vector3.zero)
{
    // Hit noktasında boyama yap
    paintSphere.HandleHitPoint(hitPoint, 1.0f);
}
```

### 📊 Gelişmiş Ayarları

#### Input Settings
```csharp
// Giriş ayarları
hitScreen.UseMouse = true;        // Mouse kullan
hitScreen.UseTouch = true;        // Touch kullan
hitScreen.UseVR = false;          // VR kullanma
```

#### Raycast Settings
```csharp
// Raycast ayarları
hitScreen.UsePhysicsRaycast = true;   // Physics.Raycast kullan
hitScreen.UseGraphicsRaycast = false; // Graphics.Raycast kullanma
hitScreen.SortByDistance = true;      // Mesafeye göre sırala
```

### 🚨 Yaygın Sorunlar

#### Sorun 1: Camera Null
**Belirtiler:** "Camera is null" hatası
**Çözüm:** Camera ayarını kontrol edin

#### Sorun 2: Mesh Collider Yok
**Belirtiler:** Raycast çalışmıyor
**Çözüm:** Hedef objelerde Mesh Collider olduğunu kontrol edin

#### Sorun 3: Distance Çok Kısa
**Belirtiler:** Uzak objelerde boyama olmuyor
**Çözüm:** Distance değerini artırın

---

## 🎮 İnteraktif Örnekler

### Basit Boyama Sistemi
```csharp
using UnityEngine;
using PaintIn3D;

public class SimplePaintSystem : MonoBehaviour
{
    [Header("Paint Settings")]
    public Color paintColor = Color.red;
    public float paintRadius = 0.1f;
    public float paintOpacity = 1.0f;
    
    [Header("References")]
    public CwPaintSphere paintSphere;
    public CwHitScreen hitScreen;
    
    void Start()
    {
        SetupPaintSystem();
    }
    
    void SetupPaintSystem()
    {
        // PaintSphere ayarları
        if (paintSphere != null)
        {
            paintSphere.Color = paintColor;
            paintSphere.Radius = paintRadius;
            paintSphere.Opacity = paintOpacity;
            paintSphere.BlendMode = CwBlendMode.AlphaBlend;
        }
        
        // HitScreen ayarları
        if (hitScreen != null)
        {
            hitScreen.Camera = Camera.main;
            hitScreen.Layers = LayerMask.GetMask("Default");
            hitScreen.Distance = 100f;
        }
    }
    
    void Update()
    {
        HandleInput();
    }
    
    void HandleInput()
    {
        // Mouse ile boyama
        if (Input.GetMouseButton(0))
        {
            hitScreen.HandleHit();
        }
        
        // R tuşu ile renk değiştir
        if (Input.GetKeyDown(KeyCode.R))
        {
            paintColor = Random.ColorHSV();
            paintSphere.Color = paintColor;
        }
        
        // Mouse wheel ile boyama boyutu değiştir
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            paintRadius = Mathf.Clamp(paintRadius + scroll, 0.01f, 1f);
            paintSphere.Radius = paintRadius;
        }
    }
}
```

### Gelişmiş Boyama Sistemi
```csharp
using UnityEngine;
using PaintIn3D;

public class AdvancedPaintSystem : MonoBehaviour
{
    [Header("Paint Tools")]
    public CwPaintSphere paintSphere;
    public CwPaintDecal paintDecal;
    public CwPaintLine paintLine;
    
    [Header("Hit Detection")]
    public CwHitScreen hitScreen;
    public CwHitCollisions hitCollisions;
    
    [Header("Settings")]
    public PaintTool currentTool = PaintTool.Sphere;
    public Color[] paintColors = { Color.red, Color.blue, Color.green, Color.yellow };
    public float[] brushSizes = { 0.05f, 0.1f, 0.2f, 0.5f };
    
    private int currentColorIndex = 0;
    private int currentSizeIndex = 1;
    
    public enum PaintTool
    {
        Sphere,
        Decal,
        Line
    }
    
    void Start()
    {
        SetupPaintSystem();
    }
    
    void SetupPaintSystem()
    {
        // Temel ayarları yap
        UpdatePaintTool();
        UpdatePaintColor();
        UpdatePaintSize();
        
        // Hit detection ayarları
        if (hitScreen != null)
        {
            hitScreen.Camera = Camera.main;
            hitScreen.Layers = LayerMask.GetMask("Default");
        }
        
        if (hitCollisions != null)
        {
            hitCollisions.Layers = LayerMask.GetMask("Default");
            hitCollisions.Threshold = 0.1f;
        }
    }
    
    void Update()
    {
        HandleInput();
    }
    
    void HandleInput()
    {
        // Boyama araçları arasında geçiş
        if (Input.GetKeyDown(KeyCode.Alpha1)) currentTool = PaintTool.Sphere;
        if (Input.GetKeyDown(KeyCode.Alpha2)) currentTool = PaintTool.Decal;
        if (Input.GetKeyDown(KeyCode.Alpha3)) currentTool = PaintTool.Line;
        
        // Renk değiştir
        if (Input.GetKeyDown(KeyCode.R))
        {
            currentColorIndex = (currentColorIndex + 1) % paintColors.Length;
            UpdatePaintColor();
        }
        
        // Boyama boyutu değiştir
        if (Input.GetKeyDown(KeyCode.T))
        {
            currentSizeIndex = (currentSizeIndex + 1) % brushSizes.Length;
            UpdatePaintSize();
        }
        
        // Boyama yap
        if (Input.GetMouseButton(0))
        {
            PerformPaint();
        }
    }
    
    void UpdatePaintTool()
    {
        // Aktif aracı güncelle
        paintSphere.enabled = (currentTool == PaintTool.Sphere);
        paintDecal.enabled = (currentTool == PaintTool.Decal);
        paintLine.enabled = (currentTool == PaintTool.Line);
    }
    
    void UpdatePaintColor()
    {
        Color color = paintColors[currentColorIndex];
        
        if (paintSphere != null) paintSphere.Color = color;
        if (paintDecal != null) paintDecal.Color = color;
        if (paintLine != null) paintLine.Color = color;
    }
    
    void UpdatePaintSize()
    {
        float size = brushSizes[currentSizeIndex];
        
        if (paintSphere != null) paintSphere.Radius = size;
        if (paintDecal != null) paintDecal.Size = size;
        if (paintLine != null) paintLine.Size = size;
    }
    
    void PerformPaint()
    {
        switch (currentTool)
        {
            case PaintTool.Sphere:
                hitScreen.HandleHit();
                break;
            case PaintTool.Decal:
                hitScreen.HandleHit();
                break;
            case PaintTool.Line:
                hitScreen.HandleHit();
                break;
        }
    }
}
```

---

## 📊 Performans İpuçları

### Hit Detection Optimizasyonu
```csharp
// Layer mask'i sınırla
hitScreen.Layers = LayerMask.GetMask("Paintable"); // Sadece boyanabilir objeler

// Mesafeyi sınırla
hitScreen.Distance = 10f; // Sadece yakın objeler

// Frame başına hit sayısını sınırla
hitCollisions.MaxHitsPerFrame = 50;
```

### Boyama Aracı Optimizasyonu
```csharp
// Boyama boyutunu sınırla
paintSphere.Radius = Mathf.Clamp(paintSphere.Radius, 0.01f, 1f);

// Şeffaflığı optimize et
paintSphere.Opacity = Mathf.Clamp(paintSphere.Opacity, 0f, 1f);
```

### Mesh Optimizasyonu
```csharp
// Texture boyutunu optimize et
paintableMesh.TextureSize = new Vector2Int(1024, 1024); // 2048 yerine 1024

// Batch size'ı artır
paintableMesh.BatchSize = 200;
```

---

## 🎯 Karavan Projesi İçin Özel Ayarlar

### First-Person Boyama
```csharp
// First-person için optimize edilmiş ayarlar
hitScreen.Camera = Camera.main;           // First-person kamera
hitScreen.Distance = 5f;                  // Kısa mesafe (karavan boyutu)
hitScreen.Layers = LayerMask.GetMask("Karavan"); // Sadece karavan

paintSphere.Radius = 0.05f;               // Küçük boyama (hassas)
paintSphere.Opacity = 0.8f;               // Hafif şeffaf
```

### Büyük Obje Optimizasyonu
```csharp
// Karavan boyutunda objeler için
paintableMesh.TextureSize = new Vector2Int(2048, 2048); // Yüksek çözünürlük
paintableMesh.BatchSize = 300;                           // Büyük batch'ler
hitScreen.Distance = 10f;                                // Uzun mesafe
```

### Çoklu Bölge Boyama
```csharp
// Karavan için farklı bölgeler
var bodyMesh = karavanBody.AddComponent<CwPaintableMesh>();
var wheelMesh = karavanWheels.AddComponent<CwPaintableMesh>();
var windowMesh = karavanWindows.AddComponent<CwPaintableMesh>();

// Farklı boyama araçları
var bodyPaint = bodyTool.AddComponent<CwPaintSphere>();
var wheelPaint = wheelTool.AddComponent<CwPaintSphere>();
var windowPaint = windowTool.AddComponent<CwPaintSphere>();
```

---

## ✅ Kontrol Listesi

### Temel Kurulum
- [ ] CwPaintableMesh doğru objeye eklendi
- [ ] CwPaintSphere boyama aracına eklendi
- [ ] CwHitScreen veya CwHitCollisions eklendi
- [ ] Temel ayarlar yapıldı

### Boyama Testi
- [ ] Mouse ile boyama çalışıyor
- [ ] Renk değiştirme çalışıyor
- [ ] Boyama boyutu değiştirme çalışıyor
- [ ] Performans kabul edilebilir

### Gelişmiş Özellikler
- [ ] Çoklu boyama araçları çalışıyor
- [ ] Hit detection doğru çalışıyor
- [ ] Layer mask doğru ayarlandı
- [ ] Hata mesajı yok

---

*Önceki: [4. PaintCore Altyapısı](paintcore.md) | Sonraki: [6. Boyama Araçları](boyama-araclari.md)*
