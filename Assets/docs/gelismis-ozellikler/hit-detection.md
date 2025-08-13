# Hit Detection (Vuruş Algılama)

Paint in 3D paketi, farklı vuruş algılama sistemleri sunar. Bu sistemler, boyama işlemlerinin nerede ve nasıl gerçekleşeceğini belirler.

## 1. Genel Bakış

Hit detection sistemleri, boyama araçlarının 3D dünyada nereye boyama yapacağını belirler. Her sistem farklı kullanım senaryoları için optimize edilmiştir.

### Temel Bileşenler
- **CwHit**: Vuruş bilgilerini içeren temel sınıf
- **CwHitCollisions**: Fizik tabanlı vuruş algılama
- **CwHitScreen**: Ekran tabanlı vuruş algılama
- **CwHitRaycast**: Raycast tabanlı vuruş algılama

## 2. CwHit Sınıfı

### Genel Bakış
`CwHit` sınıfı, tüm vuruş algılama sistemlerinin temelini oluşturur. Vuruş bilgilerini standart bir formatta tutar.

### Temel Özellikler
```csharp
public class CwHit
{
    public Vector3 Position;           // Vuruş pozisyonu (dünya koordinatları)
    public Vector3 Normal;             // Yüzey normali
    public Vector2 UV;                 // UV koordinatları
    public CwPaintableTexture Texture; // Hedef texture
    public CwModel Model;              // Hedef model
    public float Distance;             // Mesafe
    public bool Valid;                 // Geçerli vuruş mu?
}
```

### Kullanım Örneği
```csharp
// Vuruş bilgilerini alma
public void OnHit(CwHit hit)
{
    if (hit.Valid)
    {
        Debug.Log($"Vuruş pozisyonu: {hit.Position}");
        Debug.Log($"UV koordinatları: {hit.UV}");
        Debug.Log($"Yüzey normali: {hit.Normal}");
    }
}
```

## 3. CwHitCollisions (Fizik Tabanlı)

### Genel Bakış
`CwHitCollisions` bileşeni, Unity'nin fizik sistemi kullanarak vuruş algılama yapar. Collider'lar ile etkileşim kurar.

### Kullanım Senaryoları
- **VR Boyama**: VR kontrolcüleri ile fiziksel etkileşim
- **Touch Boyama**: Dokunmatik ekranlarda parmak etkileşimi
- **Mouse Boyama**: Mouse ile 3D nesnelere boyama
- **Controller Boyama**: Gamepad kontrolcüleri ile boyama

### Parametreler

#### Temel Parametreler
```csharp
public float Radius = 0.1f;            // Vuruş yarıçapı
public LayerMask LayerMask = -1;       // Katman filtresi
public bool UseColliders = true;       // Collider kullan
public bool UseTriggers = false;       // Trigger kullan
```

#### Gelişmiş Parametreler
```csharp
public bool UseRigidbody = false;      // Rigidbody kullan
public float Force = 1.0f;             // Uygulanan kuvvet
public bool UseGravity = false;        // Yerçekimi etkisi
public Vector3 Offset = Vector3.zero;  // Pozisyon offseti
```

### Performans Optimizasyonu
- **LayerMask**: Sadece gerekli katmanları kontrol edin
- **Radius**: Küçük radius değerleri daha hızlı
- **UseTriggers**: Trigger'lar daha hızlı işlem
- **Collision Detection**: Uygun collision detection mode seçin

### Örnek Kullanım
```csharp
// Basit collision detection
var hitCollisions = gameObject.AddComponent<CwHitCollisions>();
hitCollisions.Radius = 0.05f;
hitCollisions.LayerMask = LayerMask.GetMask("Paintable");

// VR kontrolcü için
hitCollisions.UseRigidbody = true;
hitCollisions.Force = 0.5f;
hitCollisions.UseGravity = false;
```

### Kurulum Adımları
1. **Collider Ekleme**: Boyanacak nesneye Collider ekleyin
2. **Layer Ayarlama**: Nesneyi uygun katmana yerleştirin
3. **CwHitCollisions Ekleme**: Boyama aracına bileşeni ekleyin
4. **Parametre Ayarlama**: Radius ve LayerMask değerlerini ayarlayın

## 4. CwHitScreen (Ekran Tabanlı)

### Genel Bakış
`CwHitScreen` bileşeni, ekran koordinatlarını kullanarak vuruş algılama yapar. Mouse ve touch input için optimize edilmiştir.

### Kullanım Senaryoları
- **Mouse Boyama**: Mouse ile ekranda boyama
- **Touch Boyama**: Dokunmatik ekranlarda boyama
- **Tablet Boyama**: Tablet kalemi ile boyama
- **UI Entegrasyonu**: UI elementleri ile entegrasyon

### Parametreler

#### Temel Parametreler
```csharp
public Camera Camera;                  // Hedef kamera
public float Distance = 10.0f;         // Raycast mesafesi
public LayerMask LayerMask = -1;       // Katman filtresi
public bool UseMouse = true;           // Mouse kullan
public bool UseTouch = true;           // Touch kullan
```

#### Gelişmiş Parametreler
```csharp
public bool UseMultiTouch = false;     // Çoklu dokunma
public int MaxTouches = 1;             // Maksimum dokunma sayısı
public bool UsePressure = false;       // Basınç duyarlılığı
public float PressureScale = 1.0f;     // Basınç ölçeği
```

### Performans Optimizasyonu
- **Camera**: Uygun kamera referansı kullanın
- **Distance**: Gereksiz uzun mesafelerden kaçının
- **LayerMask**: Sadece gerekli katmanları kontrol edin
- **MultiTouch**: Gereksizse kapalı tutun

### Örnek Kullanım
```csharp
// Mouse boyama
var hitScreen = gameObject.AddComponent<CwHitScreen>();
hitScreen.Camera = Camera.main;
hitScreen.Distance = 100f;
hitScreen.LayerMask = LayerMask.GetMask("Paintable");

// Touch boyama
hitScreen.UseMouse = false;
hitScreen.UseTouch = true;
hitScreen.UseMultiTouch = true;
hitScreen.MaxTouches = 2;
```

### Kurulum Adımları
1. **Kamera Referansı**: Hedef kamerayı atayın
2. **Layer Ayarlama**: Boyanacak nesneleri uygun katmana yerleştirin
3. **CwHitScreen Ekleme**: Boyama aracına bileşeni ekleyin
4. **Input Ayarlama**: Mouse/Touch ayarlarını yapın

## 5. CwHitRaycast (Raycast Tabanlı)

### Genel Bakış
`CwHitRaycast` bileşeni, Unity'nin raycast sistemi kullanarak vuruş algılama yapar. En esnek ve özelleştirilebilir sistemdir.

### Kullanım Senaryoları
- **Özel Raycast**: Özel raycast mantığı gerektiren durumlar
- **Çoklu Raycast**: Birden fazla raycast ile hassas algılama
- **Fizik Olmayan**: Collider gerektirmeyen sistemler
- **Performans Kritik**: Yüksek performans gerektiren uygulamalar

### Parametreler

#### Temel Parametreler
```csharp
public Vector3 Origin;                 // Raycast başlangıç noktası
public Vector3 Direction;              // Raycast yönü
public float Distance = 100.0f;        // Raycast mesafesi
public LayerMask LayerMask = -1;       // Katman filtresi
```

#### Gelişmiş Parametreler
```csharp
public bool UseSphereCast = false;     // SphereCast kullan
public float SphereRadius = 0.1f;      // Sphere yarıçapı
public bool UseBoxCast = false;        // BoxCast kullan
public Vector3 BoxSize = Vector3.one;  // Box boyutu
```

### Performans Optimizasyonu
- **Distance**: Gereksiz uzun mesafelerden kaçının
- **LayerMask**: Sadece gerekli katmanları kontrol edin
- **SphereCast**: Küçük radius değerleri kullanın
- **BoxCast**: Küçük box boyutları kullanın

### Örnek Kullanım
```csharp
// Basit raycast
var hitRaycast = gameObject.AddComponent<CwHitRaycast>();
hitRaycast.Origin = transform.position;
hitRaycast.Direction = transform.forward;
hitRaycast.Distance = 50f;

// SphereCast
hitRaycast.UseSphereCast = true;
hitRaycast.SphereRadius = 0.05f;
hitRaycast.LayerMask = LayerMask.GetMask("Paintable");
```

### Özel Raycast Mantığı
```csharp
// Özel raycast implementasyonu
public class CustomHitRaycast : CwHitRaycast
{
    protected override CwHit GetHit()
    {
        // Özel raycast mantığı
        RaycastHit hit;
        if (Physics.Raycast(Origin, Direction, out hit, Distance, LayerMask))
        {
            return new CwHit
            {
                Position = hit.point,
                Normal = hit.normal,
                UV = hit.textureCoord,
                Distance = hit.distance,
                Valid = true
            };
        }
        return new CwHit { Valid = false };
    }
}
```

## 6. Çoklu Hit Detection

### Genel Bakış
Birden fazla hit detection sistemini aynı anda kullanarak daha esnek boyama sistemleri oluşturabilirsiniz.

### Kullanım Senaryoları
- **Hibrit Sistemler**: Farklı input türlerini destekleme
- **Fallback Sistemler**: Bir sistem başarısız olursa diğerini kullanma
- **Özel Gereksinimler**: Karmaşık boyama ihtiyaçları
- **Platform Uyumluluğu**: Farklı platformlarda farklı sistemler

### Örnek Implementasyon
```csharp
// Çoklu hit detection sistemi
public class MultiHitDetection : MonoBehaviour
{
    public CwHitCollisions CollisionDetection;
    public CwHitScreen ScreenDetection;
    public CwHitRaycast RaycastDetection;
    
    public CwHit GetBestHit()
    {
        // Öncelik sırası: Collision > Screen > Raycast
        var hit = CollisionDetection.GetHit();
        if (hit.Valid) return hit;
        
        hit = ScreenDetection.GetHit();
        if (hit.Valid) return hit;
        
        return RaycastDetection.GetHit();
    }
}
```

### Performans Optimizasyonu
- **Öncelik Sırası**: En hızlı sistemi önce kontrol edin
- **Early Exit**: Geçerli hit bulunduğunda diğerlerini kontrol etmeyin
- **Caching**: Hit sonuçlarını cache'leyin
- **Lazy Loading**: Sistemleri gerektiğinde yükleyin

## 7. Hit Detection Optimizasyonu

### Genel Optimizasyonlar

#### Layer Optimizasyonu
```csharp
// Sadece gerekli katmanları kontrol edin
public LayerMask OptimizedLayerMask = LayerMask.GetMask("Paintable", "UI");
```

#### Distance Optimizasyonu
```csharp
// Uygun mesafe değerleri kullanın
public float OptimizedDistance = 10f; // Gereksiz uzun mesafelerden kaçının
```

#### Frequency Optimizasyonu
```csharp
// Hit detection sıklığını kontrol edin
public float HitDetectionInterval = 0.016f; // 60 FPS
private float lastHitTime;

public void Update()
{
    if (Time.time - lastHitTime >= HitDetectionInterval)
    {
        PerformHitDetection();
        lastHitTime = Time.time;
    }
}
```

### Platform Spesifik Optimizasyonlar

#### Mobile Optimizasyonu
```csharp
// Mobil platformlar için optimizasyon
#if UNITY_ANDROID || UNITY_IOS
public bool UseLowQualityHitDetection = true;
public float MobileHitDetectionInterval = 0.033f; // 30 FPS
#endif
```

#### VR Optimizasyonu
```csharp
// VR platformlar için optimizasyon
#if UNITY_XR
public bool UseVRHitDetection = true;
public float VRHitDetectionInterval = 0.011f; // 90 FPS
#endif
```

## 8. Sorun Giderme

### Yaygın Sorunlar

#### Hit Detection Çalışmıyor
**Sorun**: Vuruş algılama sistemi çalışmıyor
**Çözümler**:
- LayerMask ayarlarını kontrol edin
- Collider'ların doğru ayarlandığından emin olun
- Kamera referansının doğru olduğunu kontrol edin
- Distance değerinin yeterli olduğunu kontrol edin

#### Performans Sorunları
**Sorun**: Hit detection yavaş çalışıyor
**Çözümler**:
- LayerMask'i daraltın
- Distance değerini azaltın
- Hit detection sıklığını azaltın
- Gereksiz sistemleri kapatın

#### Yanlış Vuruş Pozisyonları
**Sorun**: Vuruş pozisyonları yanlış hesaplanıyor
**Çözümler**:
- Kamera transform'unu kontrol edin
- UV koordinatlarını doğrulayın
- Normal vektörlerini kontrol edin
- Offset değerlerini kontrol edin

### Debug Araçları
```csharp
// Hit detection debug sistemi
public class HitDetectionDebug : MonoBehaviour
{
    public bool ShowHitPoints = true;
    public bool ShowHitNormals = true;
    public bool ShowHitUVs = true;
    
    private void OnDrawGizmos()
    {
        if (!ShowHitPoints) return;
        
        // Hit noktalarını göster
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(hit.Position, 0.1f);
        
        if (ShowHitNormals)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(hit.Position, hit.Normal);
        }
    }
}
```

## 9. İleri Seviye Kullanım

### Özel Hit Detection Sistemleri
```csharp
// Özel hit detection sistemi
public class CustomHitDetection : MonoBehaviour
{
    public CwHit GetCustomHit()
    {
        // Özel hit detection mantığı
        // Örnek: Çoklu raycast, özel fizik, vs.
        return new CwHit();
    }
}
```

### Event-Based Hit Detection
```csharp
// Event tabanlı hit detection
public class EventBasedHitDetection : MonoBehaviour
{
    public UnityEvent<CwHit> OnHitDetected;
    
    public void DetectHit()
    {
        var hit = GetHit();
        if (hit.Valid)
        {
            OnHitDetected.Invoke(hit);
        }
    }
}
```

### Async Hit Detection
```csharp
// Asenkron hit detection
public class AsyncHitDetection : MonoBehaviour
{
    public async Task<CwHit> GetHitAsync()
    {
        return await Task.Run(() => {
            // Ağır hit detection işlemi
            return GetHit();
        });
    }
}
```
