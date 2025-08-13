# API Reference (API Referansı)

Paint in 3D paketinin tüm API'lerini detaylı olarak açıklayan kapsamlı referans dokümantasyonu.

## 1. Genel Bakış

Bu bölüm, Paint in 3D paketinin tüm public API'lerini, sınıflarını, metodlarını ve özelliklerini detaylandırır. Her API için kullanım örnekleri ve açıklamalar bulunmaktadır.

### API Kategorileri
- **Core Classes**: Temel sınıflar
- **Paint Tools**: Boyama araçları
- **Hit Detection**: Vuruş algılama
- **Blend Modes**: Karışım modları
- **Utilities**: Yardımcı sınıflar

## 2. Core Classes (Temel Sınıflar)

### CwPaintableManager

#### Genel Bakış
Tüm boyama işlemlerini yöneten ana sınıf. Sahneye eklenmesi zorunludur.

#### Public Properties
```csharp
public static CwPaintableManager Instance { get; }
// Singleton instance

public List<CwPaintableTexture> PaintableTextures { get; }
// Sahnedeki tüm paintable texture'lar

public bool IsPainting { get; }
// Şu anda boyama yapılıyor mu?

public int PaintCallCount { get; }
// Toplam boyama çağrı sayısı
```

#### Public Methods
```csharp
public static CwPaintableManager GetOrCreateInstance()
// Instance'ı al veya oluştur

public void RegisterPaintableTexture(CwPaintableTexture texture)
// Yeni paintable texture kaydet

public void UnregisterPaintableTexture(CwPaintableTexture texture)
// Paintable texture kaydını kaldır

public void ClearAllTextures()
// Tüm texture'ları temizle

public void SaveAllTextures()
// Tüm texture'ları kaydet
```

#### Kullanım Örneği
```csharp
// Manager'ı al veya oluştur
var manager = CwPaintableManager.GetOrCreateInstance();

// Texture kaydet
var texture = GetComponent<CwPaintableTexture>();
manager.RegisterPaintableTexture(texture);

// Boyama durumunu kontrol et
if (manager.IsPainting)
{
    Debug.Log("Boyama devam ediyor...");
}

// Tüm texture'ları kaydet
manager.SaveAllTextures();
```

### CwPaintableTexture

#### Genel Bakış
Boyanabilir texture'ları temsil eden sınıf.

#### Public Properties
```csharp
public int Width { get; set; }
// Texture genişliği

public int Height { get; set; }
// Texture yüksekliği

public TextureFormat Format { get; set; }
// Texture formatı

public FilterMode FilterMode { get; set; }
// Filtreleme modu

public bool GenerateMipmaps { get; set; }
// Mipmap oluştur

public Texture2D MainTexture { get; }
// Ana texture

public bool IsDirty { get; }
// Değişiklik var mı?
```

#### Public Methods
```csharp
public void SetResolution(int width, int height)
// Çözünürlük ayarla

public void SetPixel(int x, int y, Color color)
// Piksel ayarla

public Color GetPixel(int x, int y)
// Piksel al

public void SetPixels(Color[] colors)
// Piksel dizisi ayarla

public Color[] GetPixels()
// Piksel dizisi al

public void Apply()
// Değişiklikleri uygula

public void Clear()
// Texture'ı temizle

public void SaveToFile(string path)
// Dosyaya kaydet

public void LoadFromFile(string path)
// Dosyadan yükle
```

#### Kullanım Örneği
```csharp
// Texture oluştur
var paintableTexture = gameObject.AddComponent<CwPaintableTexture>();
paintableTexture.SetResolution(1024, 1024);
paintableTexture.Format = TextureFormat.RGBA32;
paintableTexture.FilterMode = FilterMode.Bilinear;
paintableTexture.GenerateMipmaps = true;

// Piksel ayarla
paintableTexture.SetPixel(100, 100, Color.red);

// Piksel al
Color pixelColor = paintableTexture.GetPixel(100, 100);

// Değişiklikleri uygula
paintableTexture.Apply();

// Texture'ı temizle
paintableTexture.Clear();

// Dosyaya kaydet
paintableTexture.SaveToFile("Assets/Textures/paint.png");
```

### CwModel

#### Genel Bakış
Boyanabilir 3D modellerin temel sınıfı.

#### Public Properties
```csharp
public Mesh Mesh { get; set; }
// Model mesh'i

public Material Material { get; set; }
// Model materyali

public Transform Transform { get; }
// Model transform'u

public Bounds Bounds { get; }
// Model sınırları

public bool IsVisible { get; set; }
// Görünür mü?
```

#### Public Methods
```csharp
public virtual void OnPaint(CwHit hit)
// Boyama event'i

public virtual bool CanPaint(CwHit hit)
// Boyanabilir mi?

public virtual Vector2 GetUV(CwHit hit)
// UV koordinatı al

public virtual Vector3 GetNormal(CwHit hit)
// Normal vektör al
```

#### Kullanım Örneği
```csharp
// Model oluştur
public class CustomModel : CwModel
{
    public override void OnPaint(CwHit hit)
    {
        // Özel boyama mantığı
        Debug.Log($"Model boyandı: {hit.Position}");
    }
    
    public override bool CanPaint(CwHit hit)
    {
        // Boyama kontrolü
        return hit.Distance < 10f;
    }
    
    public override Vector2 GetUV(CwHit hit)
    {
        // Özel UV hesaplama
        return new Vector2(hit.Position.x, hit.Position.z);
    }
}
```

## 3. Paint Tools (Boyama Araçları)

### CwPaintSphere

#### Genel Bakış
Küre şeklinde boyama aracı.

#### Public Properties
```csharp
public float Radius { get; set; }
// Boyama yarıçapı

public float Hardness { get; set; }
// Boyama sertliği (0-1)

public float Opacity { get; set; }
// Boyama opaklığı (0-1)

public Color Color { get; set; }
// Boyama rengi

public CwBlendMode BlendMode { get; set; }
// Karışım modu

public CwPaintableTexture Texture { get; set; }
// Hedef texture

public LayerMask LayerMask { get; set; }
// Katman filtresi
```

#### Public Methods
```csharp
public void Paint(CwHit hit)
// Boyama yap

public void Paint(Vector3 position, Vector2 uv)
// Pozisyon ve UV ile boyama

public void SetColor(Color color)
// Renk ayarla

public void SetRadius(float radius)
// Yarıçap ayarla

public void SetOpacity(float opacity)
// Opaklık ayarla
```

#### Kullanım Örneği
```csharp
// Paint sphere oluştur
var paintSphere = gameObject.AddComponent<CwPaintSphere>();
paintSphere.Radius = 0.1f;
paintSphere.Hardness = 0.8f;
paintSphere.Opacity = 1.0f;
paintSphere.Color = Color.red;
paintSphere.BlendMode = CwBlendMode.AlphaBlend;
paintSphere.LayerMask = LayerMask.GetMask("Paintable");

// Texture ata
var paintableTexture = FindObjectOfType<CwPaintableTexture>();
paintSphere.Texture = paintableTexture;

// Boyama yap
var hit = new CwHit
{
    Position = Vector3.zero,
    UV = Vector2.one * 0.5f,
    Valid = true
};
paintSphere.Paint(hit);
```

### CwPaintDecal

#### Genel Bakış
Dekal/sticker boyama aracı.

#### Public Properties
```csharp
public Texture2D DecalTexture { get; set; }
// Dekal texture'ı

public float Scale { get; set; }
// Ölçek faktörü

public float Rotation { get; set; }
// Rotasyon (derece)

public Vector2 Offset { get; set; }
// Pozisyon offseti

public bool Tiling { get; set; }
// Tekrarlama

public Vector2 TilingScale { get; set; }
// Tekrarlama ölçeği
```

#### Public Methods
```csharp
public void ApplyDecal(CwHit hit)
// Dekal uygula

public void SetDecalTexture(Texture2D texture)
// Dekal texture ayarla

public void SetScale(float scale)
// Ölçek ayarla

public void SetRotation(float rotation)
// Rotasyon ayarla
```

#### Kullanım Örneği
```csharp
// Decal painter oluştur
var paintDecal = gameObject.AddComponent<CwPaintDecal>();
paintDecal.DecalTexture = decalTexture;
paintDecal.Scale = 2.0f;
paintDecal.Rotation = 45f;
paintDecal.Offset = Vector2.zero;
paintDecal.Tiling = false;

// Dekal uygula
var hit = new CwHit
{
    Position = Vector3.zero,
    UV = Vector2.one * 0.5f,
    Valid = true
};
paintDecal.ApplyDecal(hit);
```

### CwPaintLine

#### Genel Bakış
Çizgi boyama aracı.

#### Public Properties
```csharp
public float Radius { get; set; }
// Çizgi kalınlığı

public float Spacing { get; set; }
// Nokta arası mesafe

public int Smoothing { get; set; }
// Yumuşatma seviyesi

public bool Connect { get; set; }
// Noktaları birleştir

public float PressureScale { get; set; }
// Basınç ölçeği

public AnimationCurve PressureCurve { get; set; }
// Basınç eğrisi
```

#### Public Methods
```csharp
public void StartLine(Vector3 position, Vector2 uv)
// Çizgi başlat

public void ContinueLine(Vector3 position, Vector2 uv)
// Çizgiyi devam ettir

public void EndLine()
// Çizgiyi bitir

public void SetPressure(float pressure)
// Basınç ayarla
```

#### Kullanım Örneği
```csharp
// Line painter oluştur
var paintLine = gameObject.AddComponent<CwPaintLine>();
paintLine.Radius = 0.05f;
paintLine.Spacing = 0.02f;
paintLine.Smoothing = 2;
paintLine.Connect = true;
paintLine.PressureScale = 1.0f;

// Çizgi çiz
paintLine.StartLine(Vector3.zero, Vector2.zero);
paintLine.ContinueLine(Vector3.right, Vector2.right);
paintLine.ContinueLine(Vector3.right + Vector3.up, Vector2.one);
paintLine.EndLine();
```

### CwPaintFill

#### Genel Bakış
Doldurma boyama aracı.

#### Public Properties
```csharp
public Color TargetColor { get; set; }
// Hedef renk

public float Tolerance { get; set; }
// Renk toleransı

public bool FillConnected { get; set; }
// Sadece bağlı alanları doldur

public int MaxPixels { get; set; }
// Maksimum piksel sayısı

public bool UseMask { get; set; }
// Maske kullan

public Texture2D MaskTexture { get; set; }
// Maske texture'ı
```

#### Public Methods
```csharp
public void Fill(CwHit hit)
// Doldurma yap

public void Fill(Vector2 uv)
// UV ile doldurma

public void SetTargetColor(Color color)
// Hedef renk ayarla

public void SetTolerance(float tolerance)
// Tolerans ayarla
```

#### Kullanım Örneği
```csharp
// Fill painter oluştur
var paintFill = gameObject.AddComponent<CwPaintFill>();
paintFill.TargetColor = Color.red;
paintFill.Tolerance = 0.1f;
paintFill.FillConnected = true;
paintFill.MaxPixels = 10000;
paintFill.UseMask = false;

// Doldurma yap
var hit = new CwHit
{
    Position = Vector3.zero,
    UV = Vector2.one * 0.5f,
    Valid = true
};
paintFill.Fill(hit);
```

## 4. Hit Detection (Vuruş Algılama)

### CwHit

#### Genel Bakış
Vuruş bilgilerini içeren temel sınıf.

#### Public Properties
```csharp
public Vector3 Position { get; set; }
// Vuruş pozisyonu (dünya koordinatları)

public Vector3 Normal { get; set; }
// Yüzey normali

public Vector2 UV { get; set; }
// UV koordinatları

public CwPaintableTexture Texture { get; set; }
// Hedef texture

public CwModel Model { get; set; }
// Hedef model

public float Distance { get; set; }
// Mesafe

public bool Valid { get; set; }
// Geçerli vuruş mu?
```

#### Public Methods
```csharp
public void Reset()
// Vuruş bilgilerini sıfırla

public CwHit Clone()
// Vuruş kopyala

public bool IsValid()
// Geçerli mi kontrol et
```

#### Kullanım Örneği
```csharp
// Hit oluştur
var hit = new CwHit
{
    Position = Vector3.zero,
    Normal = Vector3.up,
    UV = Vector2.one * 0.5f,
    Distance = 1.0f,
    Valid = true
};

// Hit kontrolü
if (hit.IsValid())
{
    Debug.Log($"Hit pozisyonu: {hit.Position}");
    Debug.Log($"UV koordinatları: {hit.UV}");
}

// Hit kopyala
var hitCopy = hit.Clone();

// Hit sıfırla
hit.Reset();
```

### CwHitCollisions

#### Genel Bakış
Fizik tabanlı vuruş algılama.

#### Public Properties
```csharp
public float Radius { get; set; }
// Vuruş yarıçapı

public LayerMask LayerMask { get; set; }
// Katman filtresi

public bool UseColliders { get; set; }
// Collider kullan

public bool UseTriggers { get; set; }
// Trigger kullan

public bool UseRigidbody { get; set; }
// Rigidbody kullan

public float Force { get; set; }
// Uygulanan kuvvet
```

#### Public Methods
```csharp
public CwHit GetHit()
// Vuruş al

public bool HasHit()
// Vuruş var mı kontrol et

public void SetRadius(float radius)
// Yarıçap ayarla

public void SetLayerMask(LayerMask layerMask)
// Layer mask ayarla
```

#### Kullanım Örneği
```csharp
// Hit collisions oluştur
var hitCollisions = gameObject.AddComponent<CwHitCollisions>();
hitCollisions.Radius = 0.1f;
hitCollisions.LayerMask = LayerMask.GetMask("Paintable");
hitCollisions.UseColliders = true;
hitCollisions.UseTriggers = false;
hitCollisions.UseRigidbody = false;

// Vuruş kontrolü
if (hitCollisions.HasHit())
{
    var hit = hitCollisions.GetHit();
    if (hit.Valid)
    {
        Debug.Log($"Collision hit: {hit.Position}");
    }
}
```

### CwHitScreen

#### Genel Bakış
Ekran tabanlı vuruş algılama.

#### Public Properties
```csharp
public Camera Camera { get; set; }
// Hedef kamera

public float Distance { get; set; }
// Raycast mesafesi

public LayerMask LayerMask { get; set; }
// Katman filtresi

public bool UseMouse { get; set; }
// Mouse kullan

public bool UseTouch { get; set; }
// Touch kullan

public bool UseMultiTouch { get; set; }
// Çoklu dokunma
```

#### Public Methods
```csharp
public CwHit GetHit()
// Vuruş al

public CwHit GetHit(Vector2 screenPosition)
// Ekran pozisyonu ile vuruş al

public bool HasHit()
// Vuruş var mı kontrol et

public void SetCamera(Camera camera)
// Kamera ayarla
```

#### Kullanım Örneği
```csharp
// Hit screen oluştur
var hitScreen = gameObject.AddComponent<CwHitScreen>();
hitScreen.Camera = Camera.main;
hitScreen.Distance = 100f;
hitScreen.LayerMask = LayerMask.GetMask("Paintable");
hitScreen.UseMouse = true;
hitScreen.UseTouch = true;
hitScreen.UseMultiTouch = false;

// Mouse pozisyonu ile vuruş al
Vector2 mousePos = Input.mousePosition;
var hit = hitScreen.GetHit(mousePos);

if (hit.Valid)
{
    Debug.Log($"Screen hit: {hit.Position}");
}
```

### CwHitRaycast

#### Genel Bakış
Raycast tabanlı vuruş algılama.

#### Public Properties
```csharp
public Vector3 Origin { get; set; }
// Raycast başlangıç noktası

public Vector3 Direction { get; set; }
// Raycast yönü

public float Distance { get; set; }
// Raycast mesafesi

public LayerMask LayerMask { get; set; }
// Katman filtresi

public bool UseSphereCast { get; set; }
// SphereCast kullan

public float SphereRadius { get; set; }
// Sphere yarıçapı
```

#### Public Methods
```csharp
public CwHit GetHit()
// Vuruş al

public CwHit GetHit(Vector3 origin, Vector3 direction)
// Başlangıç ve yön ile vuruş al

public bool HasHit()
// Vuruş var mı kontrol et

public void SetOrigin(Vector3 origin)
// Başlangıç noktası ayarla

public void SetDirection(Vector3 direction)
// Yön ayarla
```

#### Kullanım Örneği
```csharp
// Hit raycast oluştur
var hitRaycast = gameObject.AddComponent<CwHitRaycast>();
hitRaycast.Origin = transform.position;
hitRaycast.Direction = transform.forward;
hitRaycast.Distance = 50f;
hitRaycast.LayerMask = LayerMask.GetMask("Paintable");
hitRaycast.UseSphereCast = false;

// Raycast ile vuruş al
var hit = hitRaycast.GetHit();

if (hit.Valid)
{
    Debug.Log($"Raycast hit: {hit.Position}");
}

// Özel başlangıç ve yön ile vuruş al
var customHit = hitRaycast.GetHit(Vector3.zero, Vector3.up);
```

## 5. Blend Modes (Karışım Modları)

### CwBlendMode

#### Genel Bakış
Karışım modları enum'u.

#### Enum Değerleri
```csharp
public enum CwBlendMode
{
    AlphaBlend,     // Alfa karışımı
    Additive,       // Toplama
    Subtractive,    // Çıkarma
    Multiply,       // Çarpma
    Normal,         // Normal (üzerine yazma)
    Custom          // Özel karışım
}
```

#### Kullanım Örneği
```csharp
// Blend mode ayarlama
var paintSphere = GetComponent<CwPaintSphere>();

// Farklı blend modları
paintSphere.BlendMode = CwBlendMode.AlphaBlend;  // Yumuşak geçiş
paintSphere.BlendMode = CwBlendMode.Additive;    // Parlaklık artışı
paintSphere.BlendMode = CwBlendMode.Subtractive; // Karanlık efekt
paintSphere.BlendMode = CwBlendMode.Multiply;    // Renk filtresi
paintSphere.BlendMode = CwBlendMode.Normal;      // Direkt değişim

// Blend mode kontrolü
switch (paintSphere.BlendMode)
{
    case CwBlendMode.AlphaBlend:
        Debug.Log("Alpha blend kullanılıyor");
        break;
    case CwBlendMode.Additive:
        Debug.Log("Additive blend kullanılıyor");
        break;
    case CwBlendMode.Subtractive:
        Debug.Log("Subtractive blend kullanılıyor");
        break;
    case CwBlendMode.Multiply:
        Debug.Log("Multiply blend kullanılıyor");
        break;
    case CwBlendMode.Normal:
        Debug.Log("Normal blend kullanılıyor");
        break;
    case CwBlendMode.Custom:
        Debug.Log("Custom blend kullanılıyor");
        break;
}
```

## 6. Utilities (Yardımcı Sınıflar)

### CwPaintableMesh

#### Genel Bakış
Mesh tabanlı boyanabilir nesneler.

#### Public Properties
```csharp
public Mesh Mesh { get; set; }
// Mesh

public Material Material { get; set; }
// Materyal

public CwPaintableTexture PaintableTexture { get; set; }
// Boyanabilir texture

public bool AutoGenerateTexture { get; set; }
// Otomatik texture oluştur

public int TextureResolution { get; set; }
// Texture çözünürlüğü
```

#### Public Methods
```csharp
public void GenerateTexture()
// Texture oluştur

public void ClearTexture()
// Texture temizle

public void SaveTexture(string path)
// Texture kaydet

public void LoadTexture(string path)
// Texture yükle
```

#### Kullanım Örneği
```csharp
// Paintable mesh oluştur
var paintableMesh = gameObject.AddComponent<CwPaintableMesh>();
paintableMesh.Mesh = GetComponent<MeshFilter>().sharedMesh;
paintableMesh.Material = GetComponent<Renderer>().material;
paintableMesh.AutoGenerateTexture = true;
paintableMesh.TextureResolution = 1024;

// Texture oluştur
paintableMesh.GenerateTexture();

// Texture temizle
paintableMesh.ClearTexture();

// Texture kaydet
paintableMesh.SaveTexture("Assets/Textures/mesh_paint.png");
```

### CwPaintableRenderer

#### Genel Bakış
Renderer tabanlı boyanabilir nesneler.

#### Public Properties
```csharp
public Renderer Renderer { get; set; }
// Renderer

public Material Material { get; set; }
// Materyal

public CwPaintableTexture PaintableTexture { get; set; }
// Boyanabilir texture

public string TextureProperty { get; set; }
// Texture property adı
```

#### Public Methods
```csharp
public void SetTexture(Texture2D texture)
// Texture ayarla

public Texture2D GetTexture()
// Texture al

public void UpdateMaterial()
// Materyal güncelle
```

#### Kullanım Örneği
```csharp
// Paintable renderer oluştur
var paintableRenderer = gameObject.AddComponent<CwPaintableRenderer>();
paintableRenderer.Renderer = GetComponent<Renderer>();
paintableRenderer.Material = GetComponent<Renderer>().material;
paintableRenderer.TextureProperty = "_MainTex";

// Texture ayarla
var texture = new Texture2D(1024, 1024);
paintableRenderer.SetTexture(texture);

// Materyal güncelle
paintableRenderer.UpdateMaterial();
```

## 7. Events (Olaylar)

### Paint Events

#### CwPaintEvent
```csharp
public class CwPaintEvent : UnityEvent<CwHit, Color, float>
{
    // Boyama event'i
    // Parametreler: hit, color, radius
}
```

#### Kullanım Örneği
```csharp
// Paint event sistemi
public class PaintEventHandler : MonoBehaviour
{
    public CwPaintEvent OnPaint = new CwPaintEvent();
    
    private void Start()
    {
        // Event listener ekle
        OnPaint.AddListener(HandlePaint);
    }
    
    private void HandlePaint(CwHit hit, Color color, float radius)
    {
        Debug.Log($"Boyama eventi: {hit.Position}, {color}, {radius}");
    }
    
    private void OnDestroy()
    {
        // Event listener kaldır
        OnPaint.RemoveListener(HandlePaint);
    }
}
```

### Texture Events

#### CwTextureEvent
```csharp
public class CwTextureEvent : UnityEvent<CwPaintableTexture>
{
    // Texture event'i
    // Parametre: texture
}
```

#### Kullanım Örneği
```csharp
// Texture event sistemi
public class TextureEventHandler : MonoBehaviour
{
    public CwTextureEvent OnTextureChanged = new CwTextureEvent();
    
    private void Start()
    {
        // Event listener ekle
        OnTextureChanged.AddListener(HandleTextureChanged);
    }
    
    private void HandleTextureChanged(CwPaintableTexture texture)
    {
        Debug.Log($"Texture değişti: {texture.Width}x{texture.Height}");
    }
}
```

## 8. Configuration (Yapılandırma)

### CwPaintSettings

#### Genel Bakış
Paint in 3D ayarları.

#### Public Properties
```csharp
public static CwPaintSettings Instance { get; }
// Singleton instance

public int DefaultTextureResolution { get; set; }
// Varsayılan texture çözünürlüğü

public TextureFormat DefaultTextureFormat { get; set; }
// Varsayılan texture formatı

public FilterMode DefaultFilterMode { get; set; }
// Varsayılan filtreleme modu

public bool AutoSave { get; set; }
// Otomatik kaydetme

public float AutoSaveInterval { get; set; }
// Otomatik kaydetme aralığı
```

#### Public Methods
```csharp
public static CwPaintSettings GetOrCreateInstance()
// Instance'ı al veya oluştur

public void SaveSettings()
// Ayarları kaydet

public void LoadSettings()
// Ayarları yükle

public void ResetToDefaults()
// Varsayılanlara sıfırla
```

#### Kullanım Örneği
```csharp
// Ayarları yapılandır
var settings = CwPaintSettings.GetOrCreateInstance();
settings.DefaultTextureResolution = 1024;
settings.DefaultTextureFormat = TextureFormat.RGBA32;
settings.DefaultFilterMode = FilterMode.Bilinear;
settings.AutoSave = true;
settings.AutoSaveInterval = 60f; // 60 saniye

// Ayarları kaydet
settings.SaveSettings();

// Ayarları yükle
settings.LoadSettings();

// Varsayılanlara sıfırla
settings.ResetToDefaults();
```

## 9. Error Handling (Hata Yönetimi)

### CwPaintException

#### Genel Bakış
Paint in 3D özel hata sınıfı.

#### Public Properties
```csharp
public string ErrorCode { get; }
// Hata kodu

public string ErrorMessage { get; }
// Hata mesajı

public Exception InnerException { get; }
// İç hata
```

#### Kullanım Örneği
```csharp
// Hata yönetimi
try
{
    var paintSphere = GetComponent<CwPaintSphere>();
    paintSphere.Paint(hit);
}
catch (CwPaintException ex)
{
    Debug.LogError($"Paint hatası: {ex.ErrorCode} - {ex.ErrorMessage}");
    
    switch (ex.ErrorCode)
    {
        case "TEXTURE_NOT_FOUND":
            Debug.LogError("Texture bulunamadı");
            break;
        case "INVALID_HIT":
            Debug.LogError("Geçersiz hit");
            break;
        case "OUT_OF_MEMORY":
            Debug.LogError("Bellek yetersiz");
            break;
        default:
            Debug.LogError("Bilinmeyen hata");
            break;
    }
}
catch (Exception ex)
{
    Debug.LogError($"Genel hata: {ex.Message}");
}
```

## 10. Performance Monitoring (Performans İzleme)

### CwPaintProfiler

#### Genel Bakış
Paint performansını izleyen sınıf.

#### Public Properties
```csharp
public static CwPaintProfiler Instance { get; }
// Singleton instance

public bool EnableProfiling { get; set; }
// Profiling aktif mi?

public float ProfilingInterval { get; set; }
// Profiling aralığı

public int PaintCallCount { get; }
// Boyama çağrı sayısı

public float AveragePaintTime { get; }
// Ortalama boyama süresi
```

#### Public Methods
```csharp
public static CwPaintProfiler GetOrCreateInstance()
// Instance'ı al veya oluştur

public void StartProfiling()
// Profiling başlat

public void StopProfiling()
// Profiling durdur

public void ResetMetrics()
// Metrikleri sıfırla

public void LogMetrics()
// Metrikleri logla
```

#### Kullanım Örneği
```csharp
// Performans izleme
var profiler = CwPaintProfiler.GetOrCreateInstance();
profiler.EnableProfiling = true;
profiler.ProfilingInterval = 1f;

// Profiling başlat
profiler.StartProfiling();

// Boyama işlemleri...

// Metrikleri logla
profiler.LogMetrics();

// Profiling durdur
profiler.StopProfiling();

// Metrikleri sıfırla
profiler.ResetMetrics();
```

## 11. Best Practices (En İyi Uygulamalar)

### API Kullanım Önerileri

#### 1. Singleton Pattern
```csharp
// Manager'ları singleton olarak kullan
var manager = CwPaintableManager.GetOrCreateInstance();
var settings = CwPaintSettings.GetOrCreateInstance();
var profiler = CwPaintProfiler.GetOrCreateInstance();
```

#### 2. Error Handling
```csharp
// Hata yönetimi yap
try
{
    // Paint işlemleri
}
catch (CwPaintException ex)
{
    // Paint hatalarını yakala
}
catch (Exception ex)
{
    // Genel hataları yakala
}
```

#### 3. Performance Optimization
```csharp
// Performans optimizasyonu
var paintSphere = GetComponent<CwPaintSphere>();
paintSphere.Radius = Mathf.Min(paintSphere.Radius, 1f); // Yarıçap sınırla
paintSphere.Opacity = Mathf.Clamp01(paintSphere.Opacity); // Opaklık sınırla
```

#### 4. Memory Management
```csharp
// Bellek yönetimi
var texture = new CwPaintableTexture();
texture.SetResolution(1024, 1024); // Uygun boyut

// Kullanım sonrası temizlik
texture.Clear();
DestroyImmediate(texture);
```

#### 5. Event Handling
```csharp
// Event yönetimi
public class PaintEventHandler : MonoBehaviour
{
    private void OnEnable()
    {
        // Event listener ekle
    }
    
    private void OnDisable()
    {
        // Event listener kaldır
    }
}
```

Bu API referansı, Paint in 3D paketinin tüm public API'lerini kapsamlı bir şekilde dokümante eder. Her sınıf, metod ve özellik için detaylı açıklamalar ve kullanım örnekleri bulunmaktadır.
