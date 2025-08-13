# Boyama Türleri

Paint in 3D paketi, farklı boyama ihtiyaçları için çeşitli boyama türleri sunar. Her boyama türü, belirli kullanım senaryoları için optimize edilmiştir.

## 1. Sphere Boyama (Küre Boyama)

### Genel Bakış
`CwPaintSphere` bileşeni, en temel ve yaygın kullanılan boyama türüdür. Küre şeklinde bir boyama alanı oluşturur.

### Kullanım Senaryoları
- **Fırça Boyama**: Geleneksel fırça benzeri boyama
- **Spray Boyama**: Püskürtme efekti
- **Dokunmatik Boyama**: Parmak/tablet ile boyama
- **VR Boyama**: VR kontrolcüleri ile boyama

### Parametreler

#### Temel Parametreler
```csharp
public float Radius = 1.0f;           // Boyama yarıçapı
public float Hardness = 1.0f;         // Boyama sertliği (0-1)
public float Opacity = 1.0f;          // Boyama opaklığı (0-1)
public Color Color = Color.white;     // Boyama rengi
```

#### Gelişmiş Parametreler
```csharp
public CwBlendMode BlendMode = CwBlendMode.AlphaBlend;  // Karışım modu
public float Pressure = 1.0f;         // Basınç duyarlılığı
public bool Invert = false;           // Ters boyama
public LayerMask LayerMask = -1;      // Katman filtresi
```

### Performans Optimizasyonu
- **Radius**: Küçük radius değerleri daha hızlı işlem
- **Hardness**: Yüksek hardness değerleri daha az hesaplama gerektirir
- **Batch Processing**: Birden fazla sphere aynı anda işlenebilir

### Örnek Kullanım
```csharp
// Basit sphere boyama
var paintSphere = gameObject.AddComponent<CwPaintSphere>();
paintSphere.Radius = 0.5f;
paintSphere.Color = Color.red;
paintSphere.BlendMode = CwBlendMode.Additive;

// Basınç duyarlı boyama
paintSphere.Pressure = 0.8f;
paintSphere.Hardness = 0.5f;
```

## 2. Decal Boyama (Dekal Boyama)

### Genel Bakış
`CwPaintDecal` bileşeni, önceden tanımlanmış görselleri (dekal) yüzeye uygular.

### Kullanım Senaryoları
- **Logo Uygulama**: Şirket logoları, işaretler
- **Doku Ekleme**: Belirli desenler, yazılar
- **Sticker Sistemi**: Oyuncu tarafından seçilen çıkartmalar
- **Marka Uygulama**: Ürün markaları, etiketler

### Parametreler

#### Temel Parametreler
```csharp
public Texture2D Texture;             // Uygulanacak doku
public float Scale = 1.0f;            // Ölçek faktörü
public float Rotation = 0.0f;         // Rotasyon (derece)
public Vector2 Offset = Vector2.zero; // Pozisyon offseti
```

#### Gelişmiş Parametreler
```csharp
public bool Tiling = false;           // Tekrarlama
public Vector2 TilingScale = Vector2.one; // Tekrarlama ölçeği
public bool RandomRotation = false;   // Rastgele rotasyon
public float RandomRotationRange = 360f; // Rotasyon aralığı
```

### Performans Optimizasyonu
- **Texture Boyutu**: Küçük texturelar daha hızlı işlenir
- **Tiling**: Tiling kapalıyken daha hızlı
- **Mipmap**: Texture mipmap'leri performansı artırır

### Örnek Kullanım
```csharp
// Logo uygulama
var paintDecal = gameObject.AddComponent<CwPaintDecal>();
paintDecal.Texture = logoTexture;
paintDecal.Scale = 2.0f;
paintDecal.Rotation = 45f;

// Rastgele sticker sistemi
paintDecal.Texture = stickerTextures[Random.Range(0, stickerTextures.Length)];
paintDecal.RandomRotation = true;
paintDecal.RandomRotationRange = 180f;
```

## 3. Line Boyama (Çizgi Boyama)

### Genel Bakış
`CwPaintLine` bileşeni, sürekli çizgi boyama için kullanılır. Mouse/touch hareketlerini takip eder.

### Kullanım Senaryoları
- **Çizim Uygulamaları**: Serbest çizim, imza
- **Yol Çizme**: Navigasyon, işaretleme
- **Dekoratif Çizgiler**: Süsleme, desen
- **VR Çizim**: VR kontrolcüleri ile çizim

### Parametreler

#### Temel Parametreler
```csharp
public float Radius = 0.1f;           // Çizgi kalınlığı
public float Spacing = 0.1f;          // Nokta arası mesafe
public int Smoothing = 1;             // Yumuşatma seviyesi
public bool Connect = true;           // Noktaları birleştir
```

#### Gelişmiş Parametreler
```csharp
public float PressureScale = 1.0f;    // Basınç ölçeği
public AnimationCurve PressureCurve;  // Basınç eğrisi
public bool UsePressure = false;      // Basınç kullan
public float MinSpacing = 0.01f;      // Minimum mesafe
```

### Performans Optimizasyonu
- **Spacing**: Büyük spacing değerleri daha az hesaplama
- **Smoothing**: Düşük smoothing değerleri daha hızlı
- **Connect**: Kapalıyken daha hızlı işlem

### Örnek Kullanım
```csharp
// Kalem çizimi
var paintLine = gameObject.AddComponent<CwPaintLine>();
paintLine.Radius = 0.05f;
paintLine.Spacing = 0.02f;
paintLine.Smoothing = 2;
paintLine.Color = Color.black;

// Basınç duyarlı çizim
paintLine.UsePressure = true;
paintLine.PressureScale = 2.0f;
paintLine.PressureCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
```

## 4. Fill Boyama (Doldurma Boyama)

### Genel Bakış
`CwPaintFill` bileşeni, belirli bir alanı tamamen doldurur (flood fill).

### Kullanım Senaryoları
- **Renk Değiştirme**: Büyük alanları hızlıca boyama
- **Bölge Seçimi**: Belirli renkli alanları seçme
- **Temizleme**: Belirli renkleri silme
- **Maske Oluşturma**: Belirli alanları maskeleme

### Parametreler

#### Temel Parametreler
```csharp
public Color TargetColor = Color.white; // Hedef renk
public float Tolerance = 0.1f;         // Renk toleransı
public bool FillConnected = true;      // Sadece bağlı alanları doldur
public int MaxPixels = 10000;          // Maksimum piksel sayısı
```

#### Gelişmiş Parametreler
```csharp
public bool UseMask = false;           // Maske kullan
public Texture2D MaskTexture;          // Maske dokusu
public float MaskThreshold = 0.5f;     // Maske eşiği
public bool InvertMask = false;        // Maskeyi ters çevir
```

### Performans Optimizasyonu
- **MaxPixels**: Düşük değerler daha hızlı işlem
- **Tolerance**: Yüksek tolerance daha az hesaplama
- **FillConnected**: Kapalıyken tüm texture'ı tarar

### Örnek Kullanım
```csharp
// Basit doldurma
var paintFill = gameObject.AddComponent<CwPaintFill>();
paintFill.TargetColor = Color.red;
paintFill.Tolerance = 0.05f;
paintFill.Color = Color.blue;

// Maske ile doldurma
paintFill.UseMask = true;
paintFill.MaskTexture = maskTexture;
paintFill.MaskThreshold = 0.7f;
```

## 5. Box Boyama (Kutu Boyama)

### Genel Bakış
`CwPaintBox` bileşeni, dikdörtgen/küp şeklinde boyama alanları oluşturur.

### Kullanım Senaryoları
- **Seçim Aracı**: Belirli alanları seçme
- **Dikdörtgen Boyama**: Geometrik şekiller
- **Maske Oluşturma**: Belirli bölgeleri maskeleme
- **Batch İşlemler**: Toplu boyama işlemleri

### Parametreler

#### Temel Parametreler
```csharp
public Vector3 Size = Vector3.one;     // Kutu boyutu
public Vector3 Center = Vector3.zero;  // Merkez pozisyonu
public Quaternion Rotation = Quaternion.identity; // Rotasyon
```

#### Gelişmiş Parametreler
```csharp
public bool UseGradient = false;       // Gradyan kullan
public Gradient Gradient;              // Gradyan tanımı
public bool InvertGradient = false;    // Gradyanı ters çevir
public float Falloff = 1.0f;           // Geçiş yumuşaklığı
```

### Performans Optimizasyonu
- **Size**: Küçük boyutlar daha hızlı işlem
- **Gradient**: Gradyan kapalıyken daha hızlı
- **Falloff**: Yüksek falloff daha az hesaplama

### Örnek Kullanım
```csharp
// Basit kutu boyama
var paintBox = gameObject.AddComponent<CwPaintBox>();
paintBox.Size = new Vector3(2, 1, 2);
paintBox.Color = Color.green;

// Gradyan kutu
paintBox.UseGradient = true;
paintBox.Gradient = new Gradient();
paintBox.Falloff = 0.5f;
```

## 6. Stamp Boyama (Damga Boyama)

### Genel Bakış
`CwPaintStamp` bileşeni, önceden tanımlanmış desenleri damga gibi uygular.

### Kullanım Senaryoları
- **Desen Uygulama**: Tekrarlayan desenler
- **İmza Sistemi**: Oyuncu imzaları
- **Özel Efektler**: Özel boyama efektleri
- **Batch İşlemler**: Toplu desen uygulama

### Parametreler

#### Temel Parametreler
```csharp
public Texture2D StampTexture;         // Damga dokusu
public float Scale = 1.0f;             // Ölçek faktörü
public float Rotation = 0.0f;          // Rotasyon
public Vector2 Offset = Vector2.zero;  // Pozisyon offseti
```

#### Gelişmiş Parametreler
```csharp
public bool RandomRotation = false;    // Rastgele rotasyon
public float RandomRotationRange = 360f; // Rotasyon aralığı
public bool RandomScale = false;       // Rastgele ölçek
public Vector2 RandomScaleRange = new Vector2(0.5f, 1.5f); // Ölçek aralığı
```

### Performans Optimizasyonu
- **StampTexture**: Küçük texturelar daha hızlı
- **RandomRotation**: Kapalıyken daha hızlı
- **RandomScale**: Kapalıyken daha hızlı

### Örnek Kullanım
```csharp
// Basit damga
var paintStamp = gameObject.AddComponent<CwPaintStamp>();
paintStamp.StampTexture = stampTexture;
paintStamp.Scale = 1.5f;
paintStamp.Rotation = 30f;

// Rastgele damga sistemi
paintStamp.RandomRotation = true;
paintStamp.RandomRotationRange = 90f;
paintStamp.RandomScale = true;
paintStamp.RandomScaleRange = new Vector2(0.8f, 1.2f);
```

## 7. Özel Boyama Türleri

### Custom Shader Boyama
Kendi shader'larınızı kullanarak özel boyama efektleri oluşturabilirsiniz.

```csharp
// Özel shader ile boyama
public class CustomPaintTool : CwPaintSphere
{
    public Shader CustomShader;
    
    protected override void OnPaint(CwPaintableTexture texture, CwHit hit)
    {
        // Özel shader mantığı
        var material = new Material(CustomShader);
        // Boyama işlemi
    }
}
```

### Multi-Layer Boyama
Birden fazla katman üzerinde aynı anda boyama yapabilirsiniz.

```csharp
// Çoklu katman boyama
public class MultiLayerPainter : MonoBehaviour
{
    public CwPaintableTexture[] Layers;
    
    public void PaintOnAllLayers(CwHit hit)
    {
        foreach (var layer in Layers)
        {
            // Her katmana boyama uygula
        }
    }
}
```

## 8. Boyama Türleri Karşılaştırması

| Boyama Türü | Hız | Doğruluk | Kullanım Kolaylığı | Bellek Kullanımı |
|-------------|-----|----------|-------------------|------------------|
| Sphere | Yüksek | Orta | Yüksek | Düşük |
| Decal | Orta | Yüksek | Orta | Orta |
| Line | Orta | Yüksek | Yüksek | Düşük |
| Fill | Düşük | Yüksek | Orta | Yüksek |
| Box | Yüksek | Orta | Yüksek | Düşük |
| Stamp | Orta | Yüksek | Orta | Orta |

## 9. Performans İpuçları

### Genel Optimizasyonlar
- **Boyama Türü Seçimi**: İhtiyacınıza en uygun boyama türünü seçin
- **Parametre Optimizasyonu**: Gereksiz parametreleri kapatın
- **Batch İşlemler**: Birden fazla boyama işlemini birleştirin
- **LOD Sistemi**: Uzak mesafelerde basitleştirilmiş boyama kullanın

### Bellek Yönetimi
- **Texture Boyutu**: Uygun texture boyutları kullanın
- **Mipmap**: Gerektiğinde mipmap kullanın
- **Garbage Collection**: Gereksiz nesneleri temizleyin
- **Object Pooling**: Boyama nesnelerini yeniden kullanın

### GPU Optimizasyonu
- **Compute Shader**: Mümkün olduğunda compute shader kullanın
- **Command Pooling**: GPU komutlarını havuzda tutun
- **Async İşlemler**: Asenkron boyama işlemleri kullanın
- **Multi-threading**: CPU yoğun işlemleri ayrı thread'lerde yapın
