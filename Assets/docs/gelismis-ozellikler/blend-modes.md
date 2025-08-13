# Blend Modes (Karışım Modları)

Paint in 3D paketi, farklı karışım modları sunar. Bu modlar, boyama işlemlerinin mevcut texture ile nasıl birleştirileceğini belirler.

## 1. Genel Bakış

Blend modları, yeni boyama katmanının altındaki mevcut texture ile nasıl etkileşime gireceğini tanımlar. Her mod farklı görsel efektler ve kullanım senaryoları için optimize edilmiştir.

### Temel Bileşen
- **CwBlendMode**: Tüm karışım modlarını içeren enum
- **Blend Shader**: Karışım işlemlerini gerçekleştiren shader
- **Blend Parameters**: Karışım parametreleri

## 2. CwBlendMode Enum

### Mevcut Modlar
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

### Kullanım Örneği
```csharp
// Blend mode ayarlama
public CwBlendMode blendMode = CwBlendMode.AlphaBlend;

// Boyama aracında kullanım
paintSphere.BlendMode = CwBlendMode.Additive;
```

## 3. AlphaBlend (Alfa Karışımı)

### Genel Bakış
En yaygın kullanılan karışım modudur. Alfa değerine göre yumuşak geçiş sağlar.

### Matematiksel Formül
```
Result = (Source * SourceAlpha) + (Destination * (1 - SourceAlpha))
```

### Kullanım Senaryoları
- **Geleneksel Boyama**: Fırça benzeri boyama
- **Yumuşak Geçişler**: Doğal görünümlü boyama
- **Şeffaflık Efektleri**: Yarı şeffaf boyama
- **Katmanlı Boyama**: Çoklu katman sistemleri

### Parametreler
```csharp
public float Alpha = 1.0f;             // Alfa değeri (0-1)
public bool UseAlpha = true;           // Alfa kullan
public float BlendStrength = 1.0f;     // Karışım gücü
```

### Performans Özellikleri
- **Hız**: Orta seviye performans
- **Bellek**: Düşük bellek kullanımı
- **GPU**: Standart GPU işlemi
- **Optimizasyon**: Alfa değeri 0 veya 1 olduğunda hızlanır

### Örnek Kullanım
```csharp
// Yumuşak boyama
var paintSphere = gameObject.AddComponent<CwPaintSphere>();
paintSphere.BlendMode = CwBlendMode.AlphaBlend;
paintSphere.Alpha = 0.5f;
paintSphere.Color = Color.red;

// Şeffaf boyama
paintSphere.Alpha = 0.3f;
paintSphere.BlendStrength = 0.8f;
```

## 4. Additive (Toplama)

### Genel Bakış
Piksel değerlerini toplayarak parlaklık artışı sağlar. Işık efektleri için idealdir.

### Matematiksel Formül
```
Result = min(Source + Destination, 1.0)
```

### Kullanım Senaryoları
- **Işık Efektleri**: Parlaklık artışı
- **Glow Efektleri**: Parıltı efektleri
- **Highlight**: Vurgulama efektleri
- **Enerji Efektleri**: Enerji, ateş efektleri

### Parametreler
```csharp
public float AdditiveStrength = 1.0f;  // Toplama gücü
public bool ClampResult = true;        // Sonucu sınırla
public float Brightness = 1.0f;        // Parlaklık faktörü
```

### Performans Özellikleri
- **Hız**: Yüksek performans
- **Bellek**: Düşük bellek kullanımı
- **GPU**: Basit toplama işlemi
- **Optimizasyon**: Siyah arka plan üzerinde en hızlı

### Örnek Kullanım
```csharp
// Işık efekti
var paintSphere = gameObject.AddComponent<CwPaintSphere>();
paintSphere.BlendMode = CwBlendMode.Additive;
paintSphere.Color = Color.yellow;
paintSphere.AdditiveStrength = 0.8f;

// Glow efekti
paintSphere.Color = Color.cyan;
paintSphere.Brightness = 1.5f;
```

## 5. Subtractive (Çıkarma)

### Genel Bakış
Piksel değerlerini çıkararak karanlık efektler oluşturur. Gölge efektleri için idealdir.

### Matematiksel Formül
```
Result = max(Destination - Source, 0.0)
```

### Kullanım Senaryoları
- **Gölge Efektleri**: Karanlık alanlar
- **Dirt Efektleri**: Kir, leke efektleri
- **Damage Efektleri**: Hasar efektleri
- **Wear Efektleri**: Aşınma efektleri

### Parametreler
```csharp
public float SubtractiveStrength = 1.0f; // Çıkarma gücü
public bool ClampResult = true;           // Sonucu sınırla
public float Darkness = 1.0f;             // Karanlık faktörü
```

### Performans Özellikleri
- **Hız**: Yüksek performans
- **Bellek**: Düşük bellek kullanımı
- **GPU**: Basit çıkarma işlemi
- **Optimizasyon**: Beyaz arka plan üzerinde en hızlı

### Örnek Kullanım
```csharp
// Gölge efekti
var paintSphere = gameObject.AddComponent<CwPaintSphere>();
paintSphere.BlendMode = CwBlendMode.Subtractive;
paintSphere.Color = Color.black;
paintSphere.SubtractiveStrength = 0.6f;

// Kir efekti
paintSphere.Color = Color.brown;
paintSphere.Darkness = 0.8f;
```

## 6. Multiply (Çarpma)

### Genel Bakış
Piksel değerlerini çarparak karanlık efektler oluşturur. Renk filtreleri için idealdir.

### Matematiksel Formül
```
Result = Source * Destination
```

### Kullanım Senaryoları
- **Renk Filtreleri**: Renk değiştirme
- **Tint Efektleri**: Renk tonu değiştirme
- **Shadow Efektleri**: Gölge efektleri
- **Texture Overlay**: Doku üstleme

### Parametreler
```csharp
public float MultiplyStrength = 1.0f;  // Çarpma gücü
public bool InvertSource = false;      // Kaynağı ters çevir
public float Contrast = 1.0f;          // Kontrast faktörü
```

### Performans Özellikleri
- **Hız**: Yüksek performans
- **Bellek**: Düşük bellek kullanımı
- **GPU**: Basit çarpma işlemi
- **Optimizasyon**: Beyaz kaynak üzerinde en hızlı

### Örnek Kullanım
```csharp
// Renk filtresi
var paintSphere = gameObject.AddComponent<CwPaintSphere>();
paintSphere.BlendMode = CwBlendMode.Multiply;
paintSphere.Color = Color.red;
paintSphere.MultiplyStrength = 0.7f;

// Gölge efekti
paintSphere.Color = Color.gray;
paintSphere.Contrast = 1.2f;
```

## 7. Normal (Üzerine Yazma)

### Genel Bakış
Mevcut texture'ı tamamen değiştirir. En basit karışım modudur.

### Matematiksel Formül
```
Result = Source
```

### Kullanım Senaryoları
- **Temiz Boyama**: Mevcut boyamayı silme
- **Base Layer**: Temel katman oluşturma
- **Reset İşlemleri**: Texture'ı sıfırlama
- **Mask Oluşturma**: Maske texture'ları

### Parametreler
```csharp
public bool Overwrite = true;          // Üzerine yaz
public float Opacity = 1.0f;           // Opaklık
public bool UseMask = false;           // Maske kullan
```

### Performans Özellikleri
- **Hız**: En yüksek performans
- **Bellek**: En düşük bellek kullanımı
- **GPU**: Direkt kopyalama
- **Optimizasyon**: Hiçbir hesaplama gerektirmez

### Örnek Kullanım
```csharp
// Temiz boyama
var paintSphere = gameObject.AddComponent<CwPaintSphere>();
paintSphere.BlendMode = CwBlendMode.Normal;
paintSphere.Color = Color.white;
paintSphere.Opacity = 1.0f;

// Maske oluşturma
paintSphere.Color = Color.black;
paintSphere.UseMask = true;
```

## 8. Custom (Özel Karışım)

### Genel Bakış
Kendi karışım mantığınızı tanımlayabilirsiniz. En esnek moddur.

### Kullanım Senaryoları
- **Özel Efektler**: Benzersiz görsel efektler
- **Shader Entegrasyonu**: Özel shader'lar
- **Matematiksel Efektler**: Karmaşık formüller
- **Artistic Efektler**: Sanatsal efektler

### Parametreler
```csharp
public Shader CustomBlendShader;       // Özel karışım shader'ı
public Material CustomBlendMaterial;   // Özel karışım materyali
public float CustomParameter = 1.0f;   // Özel parametre
```

### Performans Özellikleri
- **Hız**: Shader'a bağlı
- **Bellek**: Shader'a bağlı
- **GPU**: Shader'a bağlı
- **Optimizasyon**: Shader optimizasyonuna bağlı

### Örnek Kullanım
```csharp
// Özel karışım shader'ı
public class CustomBlendMode : CwPaintSphere
{
    public Shader CustomShader;
    
    protected override void OnPaint(CwPaintableTexture texture, CwHit hit)
    {
        var material = new Material(CustomShader);
        material.SetFloat("_BlendStrength", 0.5f);
        // Özel karışım mantığı
    }
}
```

## 9. Blend Mode Karşılaştırması

| Blend Mode | Hız | Görsel Efekt | Kullanım Kolaylığı | Bellek Kullanımı |
|------------|-----|--------------|-------------------|------------------|
| AlphaBlend | Orta | Yumuşak geçiş | Yüksek | Düşük |
| Additive | Yüksek | Parlaklık artışı | Yüksek | Düşük |
| Subtractive | Yüksek | Karanlık efekt | Yüksek | Düşük |
| Multiply | Yüksek | Renk filtresi | Orta | Düşük |
| Normal | En Yüksek | Direkt değişim | En Yüksek | En Düşük |
| Custom | Değişken | Özel efekt | Düşük | Değişken |

## 10. Blend Mode Seçim Rehberi

### Görsel Efekt İhtiyacına Göre

#### Yumuşak Geçişler
- **Önerilen**: AlphaBlend
- **Alternatif**: Custom (yumuşak geçiş shader'ı)

#### Parlaklık Efektleri
- **Önerilen**: Additive
- **Alternatif**: Custom (parlaklık shader'ı)

#### Karanlık Efektler
- **Önerilen**: Subtractive, Multiply
- **Alternatif**: Custom (gölge shader'ı)

#### Renk Değiştirme
- **Önerilen**: Multiply
- **Alternatif**: Custom (renk shader'ı)

#### Temiz Boyama
- **Önerilen**: Normal
- **Alternatif**: AlphaBlend (yüksek alfa)

### Performans İhtiyacına Göre

#### Yüksek Performans
- **Önerilen**: Normal, Additive, Subtractive, Multiply
- **Kaçınılacak**: Custom (karmaşık shader'lar)

#### Orta Performans
- **Önerilen**: AlphaBlend
- **Alternatif**: Custom (basit shader'lar)

#### Düşük Performans (Özel Efektler)
- **Önerilen**: Custom
- **Kaçınılacak**: Karmaşık hesaplamalar

## 11. İleri Seviye Blend Teknikleri

### Çoklu Blend Modları
```csharp
// Birden fazla blend modunu birleştirme
public class MultiBlendPainter : MonoBehaviour
{
    public CwBlendMode[] BlendModes;
    public float[] BlendWeights;
    
    public void PaintWithMultiBlend(CwHit hit)
    {
        for (int i = 0; i < BlendModes.Length; i++)
        {
            // Her blend modu için ayrı boyama
            PaintWithBlendMode(hit, BlendModes[i], BlendWeights[i]);
        }
    }
}
```

### Dinamik Blend Modları
```csharp
// Zamanla değişen blend modları
public class DynamicBlendPainter : MonoBehaviour
{
    public AnimationCurve BlendModeCurve;
    
    public void UpdateBlendMode()
    {
        float time = Time.time;
        float blendValue = BlendModeCurve.Evaluate(time);
        
        // Blend değerine göre mod seçimi
        if (blendValue < 0.2f)
            currentBlendMode = CwBlendMode.Normal;
        else if (blendValue < 0.4f)
            currentBlendMode = CwBlendMode.Additive;
        else if (blendValue < 0.6f)
            currentBlendMode = CwBlendMode.Multiply;
        else if (blendValue < 0.8f)
            currentBlendMode = CwBlendMode.Subtractive;
        else
            currentBlendMode = CwBlendMode.AlphaBlend;
    }
}
```

### Conditional Blend Modları
```csharp
// Koşullu blend modları
public class ConditionalBlendPainter : MonoBehaviour
{
    public float Temperature;
    public float Humidity;
    
    public CwBlendMode GetBlendModeForConditions()
    {
        if (Temperature > 30f)
            return CwBlendMode.Additive; // Sıcak efektler
        else if (Humidity > 80f)
            return CwBlendMode.Multiply; // Nem efektleri
        else
            return CwBlendMode.AlphaBlend; // Normal boyama
    }
}
```

## 12. Performans Optimizasyonu

### Blend Mode Optimizasyonu
```csharp
// Blend modu optimizasyonu
public class OptimizedBlendPainter : MonoBehaviour
{
    private CwBlendMode lastBlendMode;
    private Material cachedMaterial;
    
    public void OptimizeBlendMode(CwBlendMode blendMode)
    {
        if (blendMode != lastBlendMode)
        {
            // Yeni blend modu için materyal oluştur
            cachedMaterial = CreateMaterialForBlendMode(blendMode);
            lastBlendMode = blendMode;
        }
    }
}
```

### Batch Blend İşlemleri
```csharp
// Toplu blend işlemleri
public class BatchBlendPainter : MonoBehaviour
{
    public void BatchPaint(List<CwHit> hits, CwBlendMode blendMode)
    {
        // Tüm hit'leri aynı blend modu ile işle
        foreach (var hit in hits)
        {
            PaintWithBlendMode(hit, blendMode);
        }
    }
}
```

## 13. Sorun Giderme

### Yaygın Blend Sorunları

#### Blend Modu Çalışmıyor
**Sorun**: Seçilen blend modu etkisiz
**Çözümler**:
- Blend modu enum değerini kontrol edin
- Shader'ın blend modunu desteklediğinden emin olun
- Parametre değerlerini kontrol edin

#### Performans Sorunları
**Sorun**: Blend işlemi yavaş
**Çözümler**:
- Daha basit blend modu kullanın
- Custom shader'ları optimize edin
- Batch işlemler kullanın

#### Görsel Sorunlar
**Sorun**: Beklenmeyen görsel sonuçlar
**Çözümler**:
- Blend modu matematiksel formülünü kontrol edin
- Renk değerlerini doğrulayın
- Alfa değerlerini kontrol edin

### Debug Araçları
```csharp
// Blend modu debug sistemi
public class BlendModeDebug : MonoBehaviour
{
    public bool ShowBlendInfo = true;
    public bool LogBlendOperations = true;
    
    public void DebugBlendMode(CwBlendMode blendMode, Color source, Color destination)
    {
        if (ShowBlendInfo)
        {
            Debug.Log($"Blend Mode: {blendMode}");
            Debug.Log($"Source Color: {source}");
            Debug.Log($"Destination Color: {destination}");
        }
    }
}
```
