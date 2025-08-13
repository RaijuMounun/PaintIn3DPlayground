# Performance Optimization (Performans Optimizasyonu)

Paint in 3D paketi, yüksek performanslı boyama işlemleri için çeşitli optimizasyon teknikleri sunar. Bu bölüm, performansı artırmak için kullanabileceğiniz yöntemleri detaylandırır.

## 1. Genel Bakış

Performans optimizasyonu, boyama işlemlerinin hızını ve verimliliğini artırmak için kritik öneme sahiptir. Özellikle büyük texture'lar ve çoklu boyama işlemleri için optimize edilmiş sistemler gereklidir.

### Optimizasyon Alanları
- **GPU Optimizasyonu**: Compute shader'lar ve batch işlemler
- **Bellek Yönetimi**: Texture streaming ve memory pooling
- **Ölçeklenebilirlik**: LOD sistemleri ve culling
- **Çoklu İşlem**: Multi-threading ve async işlemler

## 2. GPU Optimizasyonu

### Compute Shader Kullanımı

#### Genel Bakış
Compute shader'lar, GPU'nun paralel işlem gücünü kullanarak boyama işlemlerini hızlandırır.

#### Avantajlar
- **Paralel İşlem**: Binlerce piksel aynı anda işlenebilir
- **GPU Belleği**: Texture verileri GPU'da kalır
- **Batch İşlemler**: Çoklu boyama işlemleri optimize edilir
- **Özel Algoritmalar**: Karmaşık boyama algoritmaları

#### Kullanım Örneği
```csharp
// Compute shader ile boyama
public class ComputeShaderPainter : MonoBehaviour
{
    public ComputeShader PaintShader;
    public RenderTexture TargetTexture;
    
    public void PaintWithComputeShader(CwHit hit)
    {
        int kernel = PaintShader.FindKernel("Paint");
        
        PaintShader.SetTexture(kernel, "Result", TargetTexture);
        PaintShader.SetVector("HitPosition", hit.Position);
        PaintShader.SetVector("HitUV", hit.UV);
        PaintShader.SetFloat("Radius", 0.1f);
        PaintShader.SetVector("Color", Color.red);
        
        PaintShader.Dispatch(kernel, TargetTexture.width / 8, TargetTexture.height / 8, 1);
    }
}
```

#### Compute Shader Kodu Örneği
```hlsl
#pragma kernel Paint

Texture2D<float4> Result;
RWTexture2D<float4> Output;

float2 HitPosition;
float2 HitUV;
float Radius;
float4 Color;

[numthreads(8,8,1)]
void Paint (uint3 id : SV_DispatchThreadID)
{
    float2 uv = float2(id.xy) / float2(Output.Length.xy);
    float distance = length(uv - HitUV);
    
    if (distance < Radius)
    {
        float alpha = 1.0 - (distance / Radius);
        Output[id.xy] = lerp(Result[id.xy], Color, alpha);
    }
    else
    {
        Output[id.xy] = Result[id.xy];
    }
}
```

### Batch Processing (Toplu İşlem)

#### Genel Bakış
Birden fazla boyama işlemini birleştirerek GPU çağrılarını azaltır.

#### Avantajlar
- **GPU Çağrı Azaltma**: Tek seferde çoklu işlem
- **Bellek Transferi**: Daha az CPU-GPU veri transferi
- **Synchronization**: Daha az senkronizasyon gecikmesi
- **Command Pooling**: GPU komut havuzu optimizasyonu

#### Kullanım Örneği
```csharp
// Batch boyama sistemi
public class BatchPainter : MonoBehaviour
{
    private List<CwHit> pendingHits = new List<CwHit>();
    private const int BATCH_SIZE = 100;
    
    public void AddHit(CwHit hit)
    {
        pendingHits.Add(hit);
        
        if (pendingHits.Count >= BATCH_SIZE)
        {
            ProcessBatch();
        }
    }
    
    private void ProcessBatch()
    {
        // Tüm hit'leri tek seferde işle
        var batchData = new BatchPaintData
        {
            Hits = pendingHits.ToArray(),
            Colors = pendingHits.Select(h => Color.red).ToArray(),
            Radii = pendingHits.Select(h => 0.1f).ToArray()
        };
        
        PaintBatch(batchData);
        pendingHits.Clear();
    }
}
```

### Command Pooling

#### Genel Bakış
GPU komutlarını yeniden kullanarak bellek ayırma maliyetlerini azaltır.

#### Avantajlar
- **Bellek Ayırma**: Daha az garbage collection
- **Komut Yeniden Kullanımı**: Hazır komutlar
- **Senkronizasyon**: Daha az CPU-GPU senkronizasyonu
- **Predictable Performance**: Daha tutarlı performans

#### Kullanım Örneği
```csharp
// Command pooling sistemi
public class CommandPool : MonoBehaviour
{
    private Queue<GraphicsCommandBuffer> commandPool = new Queue<GraphicsCommandBuffer>();
    private const int POOL_SIZE = 10;
    
    public GraphicsCommandBuffer GetCommand()
    {
        if (commandPool.Count > 0)
        {
            var cmd = commandPool.Dequeue();
            cmd.Clear();
            return cmd;
        }
        
        return new GraphicsCommandBuffer();
    }
    
    public void ReturnCommand(GraphicsCommandBuffer cmd)
    {
        if (commandPool.Count < POOL_SIZE)
        {
            commandPool.Enqueue(cmd);
        }
        else
        {
            cmd.Dispose();
        }
    }
}
```

## 3. Bellek Yönetimi

### Texture Streaming

#### Genel Bakış
Büyük texture'ları parça parça yükleyerek bellek kullanımını optimize eder.

#### Avantajlar
- **Bellek Tasarrufu**: Sadece gerekli parçalar yüklenir
- **Lazy Loading**: İhtiyaç duyulduğunda yükleme
- **Mipmap Optimizasyonu**: Uygun çözünürlük seçimi
- **Cache Management**: Akıllı önbellek yönetimi

#### Kullanım Örneği
```csharp
// Texture streaming sistemi
public class TextureStreamer : MonoBehaviour
{
    public int TileSize = 256;
    public int MaxLoadedTiles = 16;
    
    private Dictionary<Vector2Int, Texture2D> loadedTiles = new Dictionary<Vector2Int, Texture2D>();
    private Queue<Vector2Int> tileLoadQueue = new Queue<Vector2Int>();
    
    public Texture2D GetTile(Vector2Int tileCoord)
    {
        if (loadedTiles.ContainsKey(tileCoord))
        {
            return loadedTiles[tileCoord];
        }
        
        // Tile'ı yükle
        LoadTile(tileCoord);
        return loadedTiles[tileCoord];
    }
    
    private void LoadTile(Vector2Int tileCoord)
    {
        // Bellek kontrolü
        if (loadedTiles.Count >= MaxLoadedTiles)
        {
            UnloadOldestTile();
        }
        
        // Tile'ı yükle
        var tile = LoadTileFromDisk(tileCoord);
        loadedTiles[tileCoord] = tile;
    }
}
```

### Memory Pooling

#### Genel Bakış
Sık kullanılan nesneleri havuzda tutarak bellek ayırma maliyetlerini azaltır.

#### Avantajlar
- **Garbage Collection**: Daha az GC baskısı
- **Hızlı Erişim**: Hazır nesneler
- **Bellek Fragmantasyonu**: Daha az bellek parçalanması
- **Predictable Performance**: Tutarlı performans

#### Kullanım Örneği
```csharp
// Memory pooling sistemi
public class PaintObjectPool : MonoBehaviour
{
    private Queue<CwPaintSphere> spherePool = new Queue<CwPaintSphere>();
    private Queue<CwHit> hitPool = new Queue<CwHit>();
    
    public CwPaintSphere GetPaintSphere()
    {
        if (spherePool.Count > 0)
        {
            return spherePool.Dequeue();
        }
        
        return new CwPaintSphere();
    }
    
    public void ReturnPaintSphere(CwPaintSphere sphere)
    {
        sphere.Reset();
        spherePool.Enqueue(sphere);
    }
    
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

### Garbage Collection Optimizasyonu

#### Genel Bakış
Garbage collection'ı minimize ederek performans düşüşlerini önler.

#### Stratejiler
- **Object Pooling**: Nesneleri yeniden kullan
- **Struct Kullanımı**: Value type'lar tercih et
- **String Pooling**: String'leri havuzda tut
- **Event Optimization**: Event subscription'ları optimize et

#### Kullanım Örneği
```csharp
// GC optimizasyonu
public class GCOptimizedPainter : MonoBehaviour
{
    // Struct kullanımı (value type)
    public struct PaintData
    {
        public Vector3 Position;
        public Vector2 UV;
        public Color Color;
        public float Radius;
    }
    
    // Object pooling
    private ObjectPool<PaintData> paintDataPool;
    
    // String pooling
    private static readonly string[] BlendModeNames = 
    {
        "AlphaBlend", "Additive", "Subtractive", "Multiply", "Normal"
    };
    
    public void OptimizedPaint(PaintData data)
    {
        // Struct kullanımı - GC yok
        ProcessPaintData(data);
        
        // Pool'dan al ve geri ver
        var pooledData = paintDataPool.Get();
        // İşlemler...
        paintDataPool.Return(pooledData);
    }
}
```

## 4. Ölçeklenebilirlik

### LOD (Level of Detail) Sistemi

#### Genel Bakış
Mesafeye göre boyama detayını ayarlayarak performansı optimize eder.

#### Avantajlar
- **Mesafe Bazlı Optimizasyon**: Uzak nesneler için düşük detay
- **Adaptive Quality**: Dinamik kalite ayarı
- **Performance Scaling**: Performans ölçeklenebilirliği
- **Battery Optimization**: Mobil cihazlarda pil tasarrufu

#### Kullanım Örneği
```csharp
// LOD sistemi
public class PaintLODSystem : MonoBehaviour
{
    public float[] LODDistances = { 1f, 5f, 10f, 20f };
    public int[] LODTextureSizes = { 2048, 1024, 512, 256 };
    
    public int GetLODLevel(Vector3 cameraPosition, Vector3 objectPosition)
    {
        float distance = Vector3.Distance(cameraPosition, objectPosition);
        
        for (int i = 0; i < LODDistances.Length; i++)
        {
            if (distance <= LODDistances[i])
            {
                return i;
            }
        }
        
        return LODDistances.Length - 1;
    }
    
    public void UpdatePaintQuality(CwPaintableTexture texture, int lodLevel)
    {
        int targetSize = LODTextureSizes[lodLevel];
        texture.SetResolution(targetSize, targetSize);
    }
}
```

### Culling Sistemleri

#### Genel Bakış
Görünmeyen alanları işlemeyerek gereksiz hesaplamaları önler.

#### Culling Türleri
- **Frustum Culling**: Kamera görüş alanı dışındaki nesneler
- **Occlusion Culling**: Gizli nesneler
- **Distance Culling**: Çok uzak nesneler
- **Layer Culling**: Belirli katmanlardaki nesneler

#### Kullanım Örneği
```csharp
// Culling sistemi
public class PaintCullingSystem : MonoBehaviour
{
    public Camera MainCamera;
    public float MaxPaintDistance = 50f;
    public LayerMask PaintableLayers;
    
    public bool ShouldPaint(GameObject paintableObject)
    {
        // Frustum culling
        if (!IsInFrustum(paintableObject))
            return false;
        
        // Distance culling
        float distance = Vector3.Distance(MainCamera.transform.position, paintableObject.transform.position);
        if (distance > MaxPaintDistance)
            return false;
        
        // Layer culling
        if (((1 << paintableObject.layer) & PaintableLayers) == 0)
            return false;
        
        return true;
    }
    
    private bool IsInFrustum(GameObject obj)
    {
        var bounds = obj.GetComponent<Renderer>()?.bounds;
        if (bounds == null) return false;
        
        return GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(MainCamera), bounds.Value);
    }
}
```

### Multi-Threading

#### Genel Bakış
CPU yoğun işlemleri ayrı thread'lerde çalıştırarak ana thread'i rahatlatır.

#### Avantajlar
- **CPU Utilization**: Çoklu çekirdek kullanımı
- **Responsiveness**: Ana thread'in donmaması
- **Parallel Processing**: Paralel işlemler
- **Background Tasks**: Arka plan görevleri

#### Kullanım Örneği
```csharp
// Multi-threading sistemi
public class ThreadedPainter : MonoBehaviour
{
    private Thread paintThread;
    private Queue<PaintJob> paintQueue = new Queue<PaintJob>();
    private object queueLock = new object();
    
    public void QueuePaintJob(PaintJob job)
    {
        lock (queueLock)
        {
            paintQueue.Enqueue(job);
        }
    }
    
    private void StartPaintThread()
    {
        paintThread = new Thread(PaintWorker);
        paintThread.Start();
    }
    
    private void PaintWorker()
    {
        while (true)
        {
            PaintJob job = null;
            
            lock (queueLock)
            {
                if (paintQueue.Count > 0)
                {
                    job = paintQueue.Dequeue();
                }
            }
            
            if (job != null)
            {
                // CPU yoğun boyama işlemi
                ProcessPaintJob(job);
            }
            else
            {
                Thread.Sleep(1); // CPU kullanımını azalt
            }
        }
    }
}
```

## 5. Platform Spesifik Optimizasyonlar

### Mobile Optimizasyonu

#### Genel Bakış
Mobil cihazlar için özel optimizasyonlar.

#### Stratejiler
- **Texture Compression**: ETC2, ASTC kullanımı
- **Battery Optimization**: Düşük güç tüketimi
- **Thermal Management**: Isı yönetimi
- **Memory Constraints**: Sınırlı bellek kullanımı

#### Kullanım Örneği
```csharp
// Mobil optimizasyon
public class MobilePaintOptimizer : MonoBehaviour
{
    #if UNITY_ANDROID || UNITY_IOS
    public bool UseLowQualityMode = true;
    public int MaxTextureSize = 1024;
    public float PaintUpdateInterval = 0.033f; // 30 FPS
    
    private void Start()
    {
        // Mobil cihaz ayarları
        QualitySettings.SetQualityLevel(1); // Düşük kalite
        Application.targetFrameRate = 30; // 30 FPS
        
        // Texture ayarları
        if (UseLowQualityMode)
        {
            SetLowQualityTextures();
        }
    }
    
    private void SetLowQualityTextures()
    {
        var paintableTextures = FindObjectsOfType<CwPaintableTexture>();
        foreach (var texture in paintableTextures)
        {
            texture.SetResolution(MaxTextureSize, MaxTextureSize);
        }
    }
    #endif
}
```

### VR Optimizasyonu

#### Genel Bakış
VR cihazlar için özel optimizasyonlar.

#### Stratejiler
- **90 FPS Requirement**: Yüksek frame rate
- **Low Latency**: Düşük gecikme
- **Stereoscopic Rendering**: Çift göz render
- **Motion Sickness Prevention**: Hareket hastalığını önleme

#### Kullanım Örneği
```csharp
// VR optimizasyon
public class VRPaintOptimizer : MonoBehaviour
{
    #if UNITY_XR
    public bool UseVRMode = true;
    public float VRPaintUpdateInterval = 0.011f; // 90 FPS
    public bool UseAsyncTimeWarp = true;
    
    private void Start()
    {
        if (UseVRMode)
        {
            // VR ayarları
            Application.targetFrameRate = 90;
            QualitySettings.vSyncCount = 0;
            
            // Async Time Warp
            if (UseAsyncTimeWarp)
            {
                XRSettings.eyeTextureResolutionScale = 1.0f;
            }
        }
    }
    
    private void Update()
    {
        if (UseVRMode)
        {
            // VR için optimize edilmiş boyama
            UpdateVRPaint();
        }
    }
    #endif
}
```

## 6. Profiling ve Monitoring

### Performance Profiling

#### Genel Bakış
Performans sorunlarını tespit etmek için profiling araçları.

#### Profiling Araçları
- **Unity Profiler**: Unity'nin built-in profiler'ı
- **Custom Metrics**: Özel performans metrikleri
- **Frame Rate Monitoring**: FPS takibi
- **Memory Profiling**: Bellek kullanım analizi

#### Kullanım Örneği
```csharp
// Performance profiler
public class PaintProfiler : MonoBehaviour
{
    public bool EnableProfiling = true;
    public float ProfilingInterval = 1.0f;
    
    private float lastProfilingTime;
    private int frameCount;
    private float totalFrameTime;
    
    private void Update()
    {
        if (!EnableProfiling) return;
        
        frameCount++;
        totalFrameTime += Time.deltaTime;
        
        if (Time.time - lastProfilingTime >= ProfilingInterval)
        {
            LogPerformanceMetrics();
            ResetMetrics();
        }
    }
    
    private void LogPerformanceMetrics()
    {
        float avgFrameTime = totalFrameTime / frameCount;
        float fps = 1.0f / avgFrameTime;
        
        Debug.Log($"Paint Performance - FPS: {fps:F1}, Avg Frame Time: {avgFrameTime*1000:F2}ms");
        
        // Bellek kullanımı
        long totalMemory = GC.GetTotalMemory(false);
        Debug.Log($"Memory Usage: {totalMemory / 1024 / 1024}MB");
    }
}
```

### Real-Time Monitoring

#### Genel Bakış
Gerçek zamanlı performans izleme sistemi.

#### Monitoring Özellikleri
- **Frame Rate Display**: Ekranda FPS gösterimi
- **Memory Usage**: Bellek kullanım takibi
- **Paint Statistics**: Boyama istatistikleri
- **Performance Alerts**: Performans uyarıları

#### Kullanım Örneği
```csharp
// Real-time monitoring
public class PaintMonitor : MonoBehaviour
{
    public bool ShowOnScreen = true;
    public Vector2 DisplayPosition = new Vector2(10, 10);
    
    private GUIStyle style;
    private float[] frameTimes = new float[60];
    private int frameTimeIndex = 0;
    
    private void OnGUI()
    {
        if (!ShowOnScreen) return;
        
        if (style == null)
        {
            style = new GUIStyle(GUI.skin.label);
            style.fontSize = 14;
            style.normal.textColor = Color.white;
        }
        
        // FPS gösterimi
        float fps = 1.0f / Time.deltaTime;
        GUI.Label(new Rect(DisplayPosition.x, DisplayPosition.y, 200, 20), 
                 $"FPS: {fps:F1}", style);
        
        // Bellek kullanımı
        long memory = GC.GetTotalMemory(false);
        GUI.Label(new Rect(DisplayPosition.x, DisplayPosition.y + 20, 200, 20), 
                 $"Memory: {memory / 1024 / 1024}MB", style);
        
        // Boyama istatistikleri
        GUI.Label(new Rect(DisplayPosition.x, DisplayPosition.y + 40, 200, 20), 
                 $"Paint Calls: {GetPaintCallCount()}", style);
    }
}
```

## 7. Best Practices

### Genel Optimizasyon Kuralları

#### Texture Optimizasyonu
- **Uygun Boyut**: İhtiyaç duyulan minimum boyut
- **Compression**: Platform uyumlu sıkıştırma
- **Mipmap**: Uygun mipmap kullanımı
- **Format**: Platform uyumlu format

#### Shader Optimizasyonu
- **Minimal Instructions**: Minimum shader komutları
- **Texture Sampling**: Optimize edilmiş texture okuma
- **Branching**: Shader'da branching'den kaçınma
- **Precision**: Uygun precision kullanımı

#### Code Optimizasyonu
- **Object Pooling**: Nesne havuzları kullan
- **Struct Usage**: Value type'lar tercih et
- **Garbage Collection**: GC'yi minimize et
- **Caching**: Sık kullanılan değerleri cache'le

### Performance Checklist

#### Setup Checklist
- [ ] Texture boyutları optimize edildi
- [ ] Shader'lar optimize edildi
- [ ] Object pooling sistemi kuruldu
- [ ] LOD sistemi aktif
- [ ] Culling sistemleri aktif

#### Runtime Checklist
- [ ] FPS hedefi karşılanıyor
- [ ] Bellek kullanımı kabul edilebilir
- [ ] GC baskısı minimum
- [ ] GPU kullanımı optimize
- [ ] CPU kullanımı dengeli

#### Platform Checklist
- [ ] Mobil optimizasyonlar aktif
- [ ] VR optimizasyonlar aktif
- [ ] Platform spesifik ayarlar yapıldı
- [ ] Test edildi ve doğrulandı
