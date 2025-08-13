# Genel Sorunlar ve Çözümleri

Paint in 3D paketi kullanırken karşılaşabileceğiniz yaygın sorunlar ve çözümleri bu bölümde detaylandırılmıştır.

## 1. Boyama Çalışmıyor

### Sorun: Boyama işlemi gerçekleşmiyor
**Belirtiler:**
- Fırça hareket ettirildiğinde boyama olmuyor
- Texture'da değişiklik görünmüyor
- Boyama aracı aktif ama etkisiz

### Olası Nedenler ve Çözümler

#### 1.1 CwPaintableManager Eksik
**Neden:** CwPaintableManager sahneye eklenmemiş
**Çözüm:**
```csharp
// Sahneye CwPaintableManager ekleyin
var manager = FindObjectOfType<CwPaintableManager>();
if (manager == null)
{
    var managerGO = new GameObject("PaintableManager");
    manager = managerGO.AddComponent<CwPaintableManager>();
}
```

#### 1.2 CwPaintableTexture Atanmamış
**Neden:** Boyama aracına texture atanmamış
**Çözüm:**
```csharp
// Boyama aracına texture ata
var paintSphere = GetComponent<CwPaintSphere>();
if (paintSphere.Texture == null)
{
    var paintableTexture = FindObjectOfType<CwPaintableTexture>();
    paintSphere.Texture = paintableTexture;
}
```

#### 1.3 Layer Mask Sorunu
**Neden:** Yanlış layer mask ayarları
**Çözüm:**
```csharp
// Layer mask'i kontrol edin
var paintSphere = GetComponent<CwPaintSphere>();
paintSphere.LayerMask = LayerMask.GetMask("Paintable");

// Boyanacak nesnenin layer'ını kontrol edin
var paintableObject = GetComponent<CwPaintableMesh>();
paintableObject.gameObject.layer = LayerMask.NameToLayer("Paintable");
```

#### 1.4 Collider Eksik
**Neden:** Boyanacak nesnede collider yok
**Çözüm:**
```csharp
// Collider ekleyin
var meshRenderer = GetComponent<MeshRenderer>();
if (GetComponent<Collider>() == null)
{
    var meshCollider = gameObject.AddComponent<MeshCollider>();
    meshCollider.sharedMesh = GetComponent<MeshFilter>().sharedMesh;
}
```

### Debug Adımları
1. **Console Kontrolü:** Unity Console'da hata mesajları var mı?
2. **Component Kontrolü:** Gerekli bileşenler ekli mi?
3. **Layer Kontrolü:** Layer ayarları doğru mu?
4. **Texture Kontrolü:** Texture atanmış ve aktif mi?

## 2. Performans Sorunları

### Sorun: Düşük FPS veya lag
**Belirtiler:**
- FPS düşük (30'ın altında)
- Boyama sırasında lag
- Bellek kullanımı yüksek

### Olası Nedenler ve Çözümler

#### 2.1 Texture Boyutu Çok Büyük
**Neden:** Yüksek çözünürlüklü texture'lar
**Çözüm:**
```csharp
// Texture boyutunu küçültün
var paintableTexture = GetComponent<CwPaintableTexture>();
paintableTexture.SetResolution(1024, 1024); // 2048 yerine 1024

// LOD sistemi kullanın
public class TextureLOD : MonoBehaviour
{
    public int[] LODResolutions = { 2048, 1024, 512, 256 };
    public float[] LODDistances = { 1f, 5f, 10f, 20f };
    
    public void UpdateTextureResolution(float distance)
    {
        int resolution = LODResolutions[0];
        for (int i = 0; i < LODDistances.Length; i++)
        {
            if (distance > LODDistances[i])
            {
                resolution = LODResolutions[i];
                break;
            }
        }
        
        var paintableTexture = GetComponent<CwPaintableTexture>();
        paintableTexture.SetResolution(resolution, resolution);
    }
}
```

#### 2.2 Çok Fazla Boyama Aracı
**Neden:** Aynı anda çok fazla boyama aracı aktif
**Çözüm:**
```csharp
// Boyama araçlarını havuzda tutun
public class PaintToolPool : MonoBehaviour
{
    private Queue<CwPaintSphere> paintTools = new Queue<CwPaintSphere>();
    private const int MAX_ACTIVE_TOOLS = 5;
    
    public CwPaintSphere GetPaintTool()
    {
        if (paintTools.Count > 0)
        {
            return paintTools.Dequeue();
        }
        
        if (transform.childCount < MAX_ACTIVE_TOOLS)
        {
            return CreateNewPaintTool();
        }
        
        return null;
    }
    
    public void ReturnPaintTool(CwPaintSphere tool)
    {
        tool.enabled = false;
        paintTools.Enqueue(tool);
    }
}
```

#### 2.3 Garbage Collection Sorunu
**Neden:** Sürekli nesne oluşturma/silme
**Çözüm:**
```csharp
// Object pooling kullanın
public class PaintObjectPool : MonoBehaviour
{
    private Queue<CwHit> hitPool = new Queue<CwHit>();
    private Queue<Color> colorPool = new Queue<Color>();
    
    public CwHit GetHit()
    {
        if (hitPool.Count > 0)
        {
            return hitPool.Dequeue();
        }
        return new CwHit();
    }
    
    public void ReturnHit(CwHit hit)
    {
        hit.Reset();
        hitPool.Enqueue(hit);
    }
}
```

### Performans Optimizasyonu Checklist
- [ ] Texture boyutları optimize edildi
- [ ] LOD sistemi aktif
- [ ] Object pooling kullanılıyor
- [ ] Gereksiz Update çağrıları kaldırıldı
- [ ] Shader'lar optimize edildi

## 3. Görsel Sorunlar

### Sorun: Boyama görünmüyor veya yanlış görünüyor
**Belirtiler:**
- Boyama yapılıyor ama görünmüyor
- Renkler yanlış
- Boyama kalitesi düşük

### Olası Nedenler ve Çözümler

#### 3.1 Shader Sorunu
**Neden:** Yanlış shader kullanımı
**Çözüm:**
```csharp
// Doğru shader'ı kullanın
public class ShaderFixer : MonoBehaviour
{
    public Shader PaintShader;
    
    private void Start()
    {
        var renderer = GetComponent<Renderer>();
        if (renderer.material.shader != PaintShader)
        {
            renderer.material.shader = PaintShader;
        }
    }
}
```

#### 3.2 UV Mapping Sorunu
**Neden:** Yanlış UV koordinatları
**Çözüm:**
```csharp
// UV mapping'i kontrol edin
public class UVChecker : MonoBehaviour
{
    private void Start()
    {
        var mesh = GetComponent<MeshFilter>().sharedMesh;
        var uvs = mesh.uv;
        
        // UV koordinatlarını kontrol edin
        for (int i = 0; i < uvs.Length; i++)
        {
            if (uvs[i].x < 0 || uvs[i].x > 1 || uvs[i].y < 0 || uvs[i].y > 1)
            {
                Debug.LogWarning($"UV koordinatı sınırlar dışında: {uvs[i]}");
            }
        }
    }
}
```

#### 3.3 Blend Mode Sorunu
**Neden:** Yanlış karışım modu
**Çözüm:**
```csharp
// Blend mode'u kontrol edin
public class BlendModeFixer : MonoBehaviour
{
    public CwBlendMode CorrectBlendMode = CwBlendMode.AlphaBlend;
    
    private void Start()
    {
        var paintSphere = GetComponent<CwPaintSphere>();
        if (paintSphere.BlendMode != CorrectBlendMode)
        {
            paintSphere.BlendMode = CorrectBlendMode;
        }
    }
}
```

### Görsel Kalite Optimizasyonu
```csharp
// Görsel kalite ayarları
public class VisualQualityOptimizer : MonoBehaviour
{
    public bool UseHighQuality = true;
    public bool UseMipmaps = true;
    public FilterMode TextureFilter = FilterMode.Bilinear;
    
    private void Start()
    {
        var paintableTexture = GetComponent<CwPaintableTexture>();
        
        if (UseHighQuality)
        {
            paintableTexture.SetResolution(2048, 2048);
        }
        else
        {
            paintableTexture.SetResolution(1024, 1024);
        }
        
        paintableTexture.GenerateMipmaps = UseMipmaps;
        paintableTexture.FilterMode = TextureFilter;
    }
}
```

## 4. Platform Spesifik Sorunlar

### Mobile Sorunları

#### 4.1 Mobil Performans Sorunu
**Neden:** Mobil cihazlarda yavaş çalışma
**Çözüm:**
```csharp
// Mobil optimizasyon
public class MobileOptimizer : MonoBehaviour
{
    #if UNITY_ANDROID || UNITY_IOS
    private void Start()
    {
        // Mobil ayarları
        QualitySettings.SetQualityLevel(1); // Düşük kalite
        Application.targetFrameRate = 30; // 30 FPS
        
        // Texture boyutlarını küçült
        var paintableTextures = FindObjectsOfType<CwPaintableTexture>();
        foreach (var texture in paintableTextures)
        {
            texture.SetResolution(512, 512);
        }
    }
    #endif
}
```

#### 4.2 Mobil Bellek Sorunu
**Neden:** Mobil cihazlarda bellek yetersizliği
**Çözüm:**
```csharp
// Bellek yönetimi
public class MobileMemoryManager : MonoBehaviour
{
    private void Start()
    {
        // Bellek limitini ayarla
        SystemInfo.graphicsMemorySize = 512; // 512MB limit
        
        // Texture compression kullan
        var paintableTextures = FindObjectsOfType<CwPaintableTexture>();
        foreach (var texture in paintableTextures)
        {
            texture.Compression = TextureCompression.ETC2;
        }
    }
    
    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            // Uygulama duraklatıldığında bellek temizle
            Resources.UnloadUnusedAssets();
            System.GC.Collect();
        }
    }
}
```

### VR Sorunları

#### 4.3 VR Performans Sorunu
**Neden:** VR'da düşük FPS
**Çözüm:**
```csharp
// VR optimizasyon
public class VROptimizer : MonoBehaviour
{
    #if UNITY_XR
    private void Start()
    {
        // VR ayarları
        Application.targetFrameRate = 90; // 90 FPS
        QualitySettings.vSyncCount = 0;
        
        // Async Time Warp
        XRSettings.eyeTextureResolutionScale = 1.0f;
        
        // Texture boyutlarını optimize et
        var paintableTextures = FindObjectsOfType<CwPaintableTexture>();
        foreach (var texture in paintableTextures)
        {
            texture.SetResolution(1024, 1024);
        }
    }
    #endif
}
```

#### 4.4 VR Motion Sickness
**Neden:** VR'da hareket hastalığı
**Çözüm:**
```csharp
// Motion sickness önleme
public class VRMotionSicknessPrevention : MonoBehaviour
{
    public float MaxPaintDistance = 2f;
    public float PaintUpdateInterval = 0.011f; // 90 FPS
    
    private float lastPaintTime;
    
    private void Update()
    {
        if (Time.time - lastPaintTime >= PaintUpdateInterval)
        {
            UpdateVRPaint();
            lastPaintTime = Time.time;
        }
    }
    
    private void UpdateVRPaint()
    {
        // VR için optimize edilmiş boyama
        var paintSphere = GetComponent<CwPaintSphere>();
        paintSphere.Radius = Mathf.Min(paintSphere.Radius, MaxPaintDistance);
    }
}
```

## 5. Build Sorunları

### Sorun: Build'de boyama çalışmıyor
**Belirtiler:**
- Editor'de çalışıyor ama build'de çalışmıyor
- Build'de hata veriyor
- Platform spesifik sorunlar

### Olası Nedenler ve Çözümler

#### 5.1 Scripting Define Symbols
**Neden:** Platform spesifik kodlar build'de çalışmıyor
**Çözüm:**
```csharp
// Scripting define symbols ekleyin
// Project Settings > Player > Other Settings > Scripting Define Symbols
// CW_PAINT_IN_3D ekleyin

#if CW_PAINT_IN_3D
    // Paint in 3D kodları
    var paintSphere = GetComponent<CwPaintSphere>();
#else
    // Fallback kodları
    Debug.LogWarning("Paint in 3D paketi bulunamadı!");
#endif
```

#### 5.2 Assembly Definition Sorunu
**Neden:** Assembly reference sorunları
**Çözüm:**
```csharp
// Assembly definition dosyasını kontrol edin
// Plugins/CW/PaintIn3D/PaintIn3D.asmdef
{
    "name": "PaintIn3D",
    "references": [
        "PaintCore"
    ],
    "includePlatforms": [],
    "excludePlatforms": [],
    "allowUnsafeCode": false,
    "overrideReferences": false,
    "precompiledReferences": [],
    "autoReferenced": true,
    "defineConstraints": [],
    "versionDefines": []
}
```

#### 5.3 Platform Spesifik Dosyalar
**Neden:** Platform spesifik dosyalar eksik
**Çözüm:**
```csharp
// Platform kontrolü
public class PlatformChecker : MonoBehaviour
{
    private void Start()
    {
        #if UNITY_ANDROID
            // Android spesifik ayarlar
            CheckAndroidCompatibility();
        #elif UNITY_IOS
            // iOS spesifik ayarlar
            CheckIOSCompatibility();
        #elif UNITY_STANDALONE_WIN
            // Windows spesifik ayarlar
            CheckWindowsCompatibility();
        #endif
    }
    
    private void CheckAndroidCompatibility()
    {
        // Android uyumluluk kontrolü
        if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.OpenGLES2)
        {
            Debug.LogWarning("OpenGL ES 2.0 kullanılıyor, bazı özellikler sınırlı olabilir");
        }
    }
}
```

## 6. Debug Araçları

### Debug Sistemi
```csharp
// Kapsamlı debug sistemi
public class PaintDebugger : MonoBehaviour
{
    public bool EnableDebug = true;
    public bool ShowHitPoints = true;
    public bool ShowPaintInfo = true;
    public bool LogPaintEvents = true;
    
    private void OnDrawGizmos()
    {
        if (!EnableDebug) return;
        
        // Hit noktalarını göster
        if (ShowHitPoints)
        {
            var paintSphere = GetComponent<CwPaintSphere>();
            if (paintSphere != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(transform.position, paintSphere.Radius);
            }
        }
    }
    
    private void OnGUI()
    {
        if (!EnableDebug || !ShowPaintInfo) return;
        
        // Paint bilgilerini göster
        var paintSphere = GetComponent<CwPaintSphere>();
        if (paintSphere != null)
        {
            GUI.Label(new Rect(10, 10, 300, 20), $"Paint Color: {paintSphere.Color}");
            GUI.Label(new Rect(10, 30, 300, 20), $"Paint Radius: {paintSphere.Radius}");
            GUI.Label(new Rect(10, 50, 300, 20), $"Blend Mode: {paintSphere.BlendMode}");
        }
    }
    
    public void LogPaintEvent(CwHit hit, Color color, float radius)
    {
        if (!LogPaintEvents) return;
        
        Debug.Log($"Paint Event - Position: {hit.Position}, UV: {hit.UV}, Color: {color}, Radius: {radius}");
    }
}
```

### Performance Monitor
```csharp
// Performans izleme sistemi
public class PaintPerformanceMonitor : MonoBehaviour
{
    public bool EnableMonitoring = true;
    public float UpdateInterval = 1f;
    
    private float lastUpdateTime;
    private int paintCallCount;
    private float totalPaintTime;
    
    private void Update()
    {
        if (!EnableMonitoring) return;
        
        if (Time.time - lastUpdateTime >= UpdateInterval)
        {
            LogPerformanceMetrics();
            ResetMetrics();
        }
    }
    
    public void RecordPaintCall(float paintTime)
    {
        paintCallCount++;
        totalPaintTime += paintTime;
    }
    
    private void LogPerformanceMetrics()
    {
        float avgPaintTime = paintCallCount > 0 ? totalPaintTime / paintCallCount : 0f;
        float fps = 1f / Time.deltaTime;
        
        Debug.Log($"Paint Performance - FPS: {fps:F1}, Paint Calls: {paintCallCount}, Avg Paint Time: {avgPaintTime*1000:F2}ms");
        
        // Bellek kullanımı
        long totalMemory = GC.GetTotalMemory(false);
        Debug.Log($"Memory Usage: {totalMemory / 1024 / 1024}MB");
    }
    
    private void ResetMetrics()
    {
        paintCallCount = 0;
        totalPaintTime = 0f;
        lastUpdateTime = Time.time;
    }
}
```

## 7. Sorun Giderme Checklist

### Genel Kontrol Listesi
- [ ] CwPaintableManager sahneye ekli mi?
- [ ] CwPaintableTexture atanmış mı?
- [ ] Layer mask ayarları doğru mu?
- [ ] Collider'lar ekli mi?
- [ ] Shader doğru mu?
- [ ] UV mapping doğru mu?
- [ ] Blend mode uygun mu?

### Performans Kontrol Listesi
- [ ] Texture boyutları optimize edildi mi?
- [ ] LOD sistemi aktif mi?
- [ ] Object pooling kullanılıyor mu?
- [ ] Gereksiz Update çağrıları kaldırıldı mı?
- [ ] Shader'lar optimize edildi mi?
- [ ] Platform spesifik optimizasyonlar yapıldı mı?

### Build Kontrol Listesi
- [ ] Scripting define symbols eklendi mi?
- [ ] Assembly definition dosyaları doğru mu?
- [ ] Platform spesifik dosyalar mevcut mu?
- [ ] Platform uyumluluk kontrolü yapıldı mı?
- [ ] Test build'de test edildi mi?

### Debug Kontrol Listesi
- [ ] Debug araçları aktif mi?
- [ ] Console'da hata var mı?
- [ ] Performance monitor çalışıyor mu?
- [ ] Gizmos görünüyor mu?
- [ ] Log mesajları yazılıyor mu?
