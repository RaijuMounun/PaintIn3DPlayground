# 6. Boyama Araçları

## 🎯 Genel Bakış

**Boyama Araçları**, Paint in 3D paketinde farklı boyama efektleri ve teknikleri için kullanılan bileşenlerdir. Her araç, belirli bir boyama türü için optimize edilmiştir ve farklı görsel sonuçlar elde etmenizi sağlar.

### 📋 Mevcut Boyama Araçları
- **🖌️ CwPaintSphere**: Küre şeklinde boyama (en yaygın)
- **🎨 CwPaintDecal**: Dekal/çıkartma boyama
- **✏️ CwPaintLine**: Çizgi boyama
- **🔄 CwPaintFill**: Dolgu boyama
- **🔲 CwPaintBox**: Kutu şeklinde boyama
- **🎯 CwPaintStamp**: Damga boyama

---

## 🖌️ CwPaintSphere (Küre Boyama)

### 📖 Genel Bilgiler
**Ne işe yarar?** Küre şeklinde boyama - en yaygın kullanılan araç  
**Kullanım alanları:** Genel boyama, fırça efektleri, doku boyama  
**Performans:** Yüksek, GPU optimizasyonu mevcut  
**Karmaşıklık:** Düşük, kolay kullanım

### 🔧 Temel Ayarları

#### Boyama Parametreleri
```csharp
// Temel boyama ayarları
paintSphere.Color = Color.red;           // Boyama rengi
paintSphere.Radius = 0.1f;              // Boyama yarıçapı
paintSphere.Opacity = 1.0f;             // Şeffaflık
paintSphere.Hardness = 0.5f;            // Kenar sertliği
paintSphere.BlendMode = CwBlendMode.AlphaBlend; // Karışım modu
```

#### Gelişmiş Parametreler
```csharp
// Gelişmiş ayarlar
paintSphere.Scale = Vector3.one;        // Ölçek (elips için)
paintSphere.Falloff = CwFalloff.Smooth; // Düşüş eğrisi
paintSphere.Tiling = Vector2.one;       // Döşeme
paintSphere.Offset = Vector2.zero;      // Kaydırma
```

### 🛠️ Kullanım Örnekleri

#### Basit Küre Boyama
```csharp
public class SimpleSpherePainting : MonoBehaviour
{
    public CwPaintSphere paintSphere;
    public CwHitScreen hitScreen;
    
    void Start()
    {
        // Temel ayarlar
        paintSphere.Color = Color.blue;
        paintSphere.Radius = 0.1f;
        paintSphere.Opacity = 1.0f;
        paintSphere.BlendMode = CwBlendMode.AlphaBlend;
    }
    
    void Update()
    {
        // Mouse ile boyama
        if (Input.GetMouseButton(0))
        {
            hitScreen.HandleHit();
        }
    }
}
```

#### Dinamik Boyama Boyutu
```csharp
public class DynamicSpherePainting : MonoBehaviour
{
    public CwPaintSphere paintSphere;
    
    void Update()
    {
        // Mouse wheel ile boyama boyutu değiştir
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            paintSphere.Radius = Mathf.Clamp(paintSphere.Radius + scroll, 0.01f, 2f);
        }
        
        // Shift + Mouse wheel ile şeffaflık değiştir
        if (Input.GetKey(KeyCode.LeftShift))
        {
            float opacityScroll = Input.GetAxis("Mouse ScrollWheel");
            if (opacityScroll != 0)
            {
                paintSphere.Opacity = Mathf.Clamp(paintSphere.Opacity + opacityScroll, 0f, 1f);
            }
        }
    }
}
```

#### Basınç Duyarlı Boyama
```csharp
public class PressureSensitivePainting : MonoBehaviour
{
    public CwPaintSphere paintSphere;
    
    void Start()
    {
        // Basınç duyarlılığını etkinleştir
        paintSphere.PressureMode = CwPressureMode.Both; // Hem yarıçap hem şeffaflık
    }
    
    void Update()
    {
        // Mouse basıncını simüle et (0-1 arası)
        float pressure = Input.GetMouseButton(0) ? 1f : 0f;
        
        // Basınç ile boyama yap
        if (pressure > 0)
        {
            // Basınç değeri otomatik olarak kullanılır
            hitScreen.HandleHit();
        }
    }
}
```

### 🎨 Görsel Efektler

#### Yumuşak Kenarlar
```csharp
// Yumuşak kenarlı boyama
paintSphere.Hardness = 0.0f;        // Çok yumuşak
paintSphere.Falloff = CwFalloff.Smooth; // Yumuşak düşüş
```

#### Sert Kenarlar
```csharp
// Sert kenarlı boyama
paintSphere.Hardness = 1.0f;        // Çok sert
paintSphere.Falloff = CwFalloff.Linear; // Doğrusal düşüş
```

#### Elips Boyama
```csharp
// Elips şeklinde boyama
paintSphere.Scale = new Vector3(2, 1, 1); // Yatay elips
paintSphere.Scale = new Vector3(1, 2, 1); // Dikey elips
```

---

## 🎨 CwPaintDecal (Dekal Boyama)

### 📖 Genel Bilgiler
**Ne işe yarar?** Dekal/çıkartma boyama - texture tabanlı boyama  
**Kullanım alanları:** Logo, desen, texture boyama  
**Performans:** Orta, texture işleme gerektirir  
**Karmaşıklık:** Orta, texture hazırlama gerekli

### 🔧 Temel Ayarları

#### Dekal Parametreleri
```csharp
// Temel dekal ayarları
paintDecal.Color = Color.white;           // Renk (texture ile karışır)
paintDecal.Size = 1.0f;                  // Boyut
paintDecal.Opacity = 1.0f;               // Şeffaflık
paintDecal.Rotation = 0f;                // Döndürme
paintDecal.BlendMode = CwBlendMode.AlphaBlend; // Karışım modu
```

#### Texture Ayarları
```csharp
// Texture ayarları
paintDecal.Texture = myTexture;           // Dekal texture'ı
paintDecal.Tiling = Vector2.one;          // Döşeme
paintDecal.Offset = Vector2.zero;         // Kaydırma
paintDecal.WrapMode = TextureWrapMode.Clamp; // Sarma modu
```

### 🛠️ Kullanım Örnekleri

#### Basit Dekal Boyama
```csharp
public class SimpleDecalPainting : MonoBehaviour
{
    public CwPaintDecal paintDecal;
    public Texture2D decalTexture;
    
    void Start()
    {
        // Dekal ayarları
        paintDecal.Texture = decalTexture;
        paintDecal.Size = 0.5f;
        paintDecal.Opacity = 1.0f;
        paintDecal.BlendMode = CwBlendMode.AlphaBlend;
    }
    
    void Update()
    {
        // Mouse ile dekal boyama
        if (Input.GetMouseButton(0))
        {
            hitScreen.HandleHit();
        }
    }
}
```

#### Döndürülebilir Dekal
```csharp
public class RotatableDecalPainting : MonoBehaviour
{
    public CwPaintDecal paintDecal;
    
    void Update()
    {
        // R tuşu ile dekal döndür
        if (Input.GetKeyDown(KeyCode.R))
        {
            paintDecal.Rotation += 45f; // 45 derece döndür
        }
        
        // Mouse wheel ile boyut değiştir
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            paintDecal.Size = Mathf.Clamp(paintDecal.Size + scroll, 0.1f, 5f);
        }
    }
}
```

#### Çoklu Dekal Sistemi
```csharp
public class MultiDecalSystem : MonoBehaviour
{
    [System.Serializable]
    public class DecalData
    {
        public string name;
        public Texture2D texture;
        public Color tint = Color.white;
    }
    
    public CwPaintDecal paintDecal;
    public DecalData[] decals;
    
    private int currentDecalIndex = 0;
    
    void Start()
    {
        UpdateDecal();
    }
    
    void Update()
    {
        // Tab tuşu ile dekal değiştir
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            currentDecalIndex = (currentDecalIndex + 1) % decals.Length;
            UpdateDecal();
        }
    }
    
    void UpdateDecal()
    {
        if (decals.Length > 0)
        {
            paintDecal.Texture = decals[currentDecalIndex].texture;
            paintDecal.Color = decals[currentDecalIndex].tint;
        }
    }
}
```

---

## ✏️ CwPaintLine (Çizgi Boyama)

### 📖 Genel Bilgiler
**Ne işe yarar?** Çizgi boyama - sürekli çizgi çizme  
**Kullanım alanları:** Çizim, yazı, çizgi sanatı  
**Performans:** Yüksek, GPU optimizasyonu mevcut  
**Karmaşıklık:** Düşük, kolay kullanım

### 🔧 Temel Ayarları

#### Çizgi Parametreleri
```csharp
// Temel çizgi ayarları
paintLine.Color = Color.black;            // Çizgi rengi
paintLine.Size = 0.05f;                  // Çizgi kalınlığı
paintLine.Opacity = 1.0f;                // Şeffaflık
paintLine.BlendMode = CwBlendMode.AlphaBlend; // Karışım modu
```

#### Çizgi Stili
```csharp
// Çizgi stili ayarları
paintLine.Style = CwPaintLine.StyleType.Solid; // Düz çizgi
paintLine.Style = CwPaintLine.StyleType.Dashed; // Kesikli çizgi
paintLine.Style = CwPaintLine.StyleType.Dotted; // Noktalı çizgi
```

### 🛠️ Kullanım Örnekleri

#### Basit Çizgi Boyama
```csharp
public class SimpleLinePainting : MonoBehaviour
{
    public CwPaintLine paintLine;
    public CwHitScreen hitScreen;
    
    private Vector3 lastHitPoint;
    private bool isDrawing = false;
    
    void Start()
    {
        // Çizgi ayarları
        paintLine.Color = Color.black;
        paintLine.Size = 0.05f;
        paintLine.Opacity = 1.0f;
    }
    
    void Update()
    {
        // Mouse basılı tutulduğunda çizgi çiz
        if (Input.GetMouseButtonDown(0))
        {
            isDrawing = true;
            lastHitPoint = GetHitPoint();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDrawing = false;
        }
        
        if (isDrawing)
        {
            Vector3 currentHitPoint = GetHitPoint();
            if (currentHitPoint != Vector3.zero && lastHitPoint != Vector3.zero)
            {
                paintLine.HandleHitLine(lastHitPoint, currentHitPoint, 1.0f);
                lastHitPoint = currentHitPoint;
            }
        }
    }
    
    Vector3 GetHitPoint()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            return hit.point;
        }
        return Vector3.zero;
    }
}
```

#### Kalınlık Değişken Çizgi
```csharp
public class VariableThicknessLine : MonoBehaviour
{
    public CwPaintLine paintLine;
    
    void Update()
    {
        // Mouse wheel ile çizgi kalınlığı değiştir
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            paintLine.Size = Mathf.Clamp(paintLine.Size + scroll, 0.01f, 1f);
        }
        
        // Shift + Mouse wheel ile şeffaflık değiştir
        if (Input.GetKey(KeyCode.LeftShift))
        {
            float opacityScroll = Input.GetAxis("Mouse ScrollWheel");
            if (opacityScroll != 0)
            {
                paintLine.Opacity = Mathf.Clamp(paintLine.Opacity + opacityScroll, 0f, 1f);
            }
        }
    }
}
```

---

## 🔄 CwPaintFill (Dolgu Boyama)

### 📖 Genel Bilgiler
**Ne işe yarar?** Dolgu boyama - kapalı alanları doldurma  
**Kullanım alanları:** Büyük alan boyama, renk değiştirme  
**Performans:** Orta, flood fill algoritması  
**Karmaşıklık:** Orta, UV koordinatları önemli

### 🔧 Temel Ayarları

#### Dolgu Parametreleri
```csharp
// Temel dolgu ayarları
paintFill.Color = Color.red;              // Dolgu rengi
paintFill.Opacity = 1.0f;                // Şeffaflık
paintFill.Tolerance = 0.1f;              // Tolerans
paintFill.BlendMode = CwBlendMode.AlphaBlend; // Karışım modu
```

### 🛠️ Kullanım Örnekleri

#### Basit Dolgu Boyama
```csharp
public class SimpleFillPainting : MonoBehaviour
{
    public CwPaintFill paintFill;
    public CwHitScreen hitScreen;
    
    void Start()
    {
        // Dolgu ayarları
        paintFill.Color = Color.blue;
        paintFill.Opacity = 1.0f;
        paintFill.Tolerance = 0.1f;
    }
    
    void Update()
    {
        // F tuşu ile dolgu boyama
        if (Input.GetKeyDown(KeyCode.F))
        {
            Vector3 hitPoint = GetHitPoint();
            if (hitPoint != Vector3.zero)
            {
                paintFill.HandleHitPoint(hitPoint, 1.0f);
            }
        }
    }
    
    Vector3 GetHitPoint()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            return hit.point;
        }
        return Vector3.zero;
    }
}
```

---

## 🔲 CwPaintBox (Kutu Boyama)

### 📖 Genel Bilgiler
**Ne işe yarar?** Kutu şeklinde boyama - dikdörtgen alanlar  
**Kullanım alanları:** Geometrik şekiller, düz alanlar  
**Performans:** Yüksek, GPU optimizasyonu mevcut  
**Karmaşıklık:** Düşük, kolay kullanım

### 🔧 Temel Ayarları

#### Kutu Parametreleri
```csharp
// Temel kutu ayarları
paintBox.Color = Color.green;             // Kutu rengi
paintBox.Size = Vector3.one;              // Kutu boyutu
paintBox.Opacity = 1.0f;                 // Şeffaflık
paintBox.BlendMode = CwBlendMode.AlphaBlend; // Karışım modu
```

### 🛠️ Kullanım Örnekleri

#### Basit Kutu Boyama
```csharp
public class SimpleBoxPainting : MonoBehaviour
{
    public CwPaintBox paintBox;
    public CwHitScreen hitScreen;
    
    void Start()
    {
        // Kutu ayarları
        paintBox.Color = Color.green;
        paintBox.Size = new Vector3(0.5f, 0.5f, 0.1f);
        paintBox.Opacity = 1.0f;
    }
    
    void Update()
    {
        // B tuşu ile kutu boyama
        if (Input.GetKeyDown(KeyCode.B))
        {
            hitScreen.HandleHit();
        }
    }
}
```

---

## 🎯 CwPaintStamp (Damga Boyama)

### 📖 Genel Bilgiler
**Ne işe yarar?** Damga boyama - önceden tanımlanmış şekiller  
**Kullanım alanları:** Logo, sembol, tekrar eden desenler  
**Performans:** Orta, texture işleme gerektirir  
**Karmaşıklık:** Orta, damga hazırlama gerekli

### 🔧 Temel Ayarları

#### Damga Parametreleri
```csharp
// Temel damga ayarları
paintStamp.Color = Color.white;           // Damga rengi
paintStamp.Size = 1.0f;                  // Damga boyutu
paintStamp.Opacity = 1.0f;               // Şeffaflık
paintStamp.Rotation = 0f;                // Döndürme
paintStamp.BlendMode = CwBlendMode.AlphaBlend; // Karışım modu
```

### 🛠️ Kullanım Örnekleri

#### Basit Damga Boyama
```csharp
public class SimpleStampPainting : MonoBehaviour
{
    public CwPaintStamp paintStamp;
    public CwHitScreen hitScreen;
    
    void Start()
    {
        // Damga ayarları
        paintStamp.Color = Color.white;
        paintStamp.Size = 0.5f;
        paintStamp.Opacity = 1.0f;
    }
    
    void Update()
    {
        // S tuşu ile damga boyama
        if (Input.GetKeyDown(KeyCode.S))
        {
            hitScreen.HandleHit();
        }
    }
}
```

---

## 🎮 Gelişmiş Boyama Sistemi

### Çoklu Araç Sistemi
```csharp
using UnityEngine;
using PaintIn3D;

public class AdvancedPaintToolSystem : MonoBehaviour
{
    [Header("Paint Tools")]
    public CwPaintSphere paintSphere;
    public CwPaintDecal paintDecal;
    public CwPaintLine paintLine;
    public CwPaintFill paintFill;
    public CwPaintBox paintBox;
    public CwPaintStamp paintStamp;
    
    [Header("Hit Detection")]
    public CwHitScreen hitScreen;
    
    [Header("Settings")]
    public PaintToolType currentTool = PaintToolType.Sphere;
    public Color[] paintColors = { Color.red, Color.blue, Color.green, Color.yellow, Color.magenta };
    public float[] toolSizes = { 0.05f, 0.1f, 0.2f, 0.5f, 1.0f };
    
    private int currentColorIndex = 0;
    private int currentSizeIndex = 1;
    
    public enum PaintToolType
    {
        Sphere,
        Decal,
        Line,
        Fill,
        Box,
        Stamp
    }
    
    void Start()
    {
        SetupPaintSystem();
    }
    
    void SetupPaintSystem()
    {
        // Tüm araçları deaktifleştir
        DisableAllTools();
        
        // Aktif aracı etkinleştir
        EnableCurrentTool();
        
        // Temel ayarları yap
        UpdatePaintColor();
        UpdatePaintSize();
    }
    
    void Update()
    {
        HandleInput();
    }
    
    void HandleInput()
    {
        // Araç değiştirme
        if (Input.GetKeyDown(KeyCode.Alpha1)) SwitchTool(PaintToolType.Sphere);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SwitchTool(PaintToolType.Decal);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SwitchTool(PaintToolType.Line);
        if (Input.GetKeyDown(KeyCode.Alpha4)) SwitchTool(PaintToolType.Fill);
        if (Input.GetKeyDown(KeyCode.Alpha5)) SwitchTool(PaintToolType.Box);
        if (Input.GetKeyDown(KeyCode.Alpha6)) SwitchTool(PaintToolType.Stamp);
        
        // Renk değiştirme
        if (Input.GetKeyDown(KeyCode.R))
        {
            currentColorIndex = (currentColorIndex + 1) % paintColors.Length;
            UpdatePaintColor();
        }
        
        // Boyut değiştirme
        if (Input.GetKeyDown(KeyCode.T))
        {
            currentSizeIndex = (currentSizeIndex + 1) % toolSizes.Length;
            UpdatePaintSize();
        }
        
        // Boyama yapma
        if (Input.GetMouseButton(0))
        {
            PerformPaint();
        }
    }
    
    void SwitchTool(PaintToolType toolType)
    {
        currentTool = toolType;
        DisableAllTools();
        EnableCurrentTool();
        UpdatePaintColor();
        UpdatePaintSize();
        
        Debug.Log($"Araç değişti: {toolType}");
    }
    
    void DisableAllTools()
    {
        if (paintSphere != null) paintSphere.enabled = false;
        if (paintDecal != null) paintDecal.enabled = false;
        if (paintLine != null) paintLine.enabled = false;
        if (paintFill != null) paintFill.enabled = false;
        if (paintBox != null) paintBox.enabled = false;
        if (paintStamp != null) paintStamp.enabled = false;
    }
    
    void EnableCurrentTool()
    {
        switch (currentTool)
        {
            case PaintToolType.Sphere:
                if (paintSphere != null) paintSphere.enabled = true;
                break;
            case PaintToolType.Decal:
                if (paintDecal != null) paintDecal.enabled = true;
                break;
            case PaintToolType.Line:
                if (paintLine != null) paintLine.enabled = true;
                break;
            case PaintToolType.Fill:
                if (paintFill != null) paintFill.enabled = true;
                break;
            case PaintToolType.Box:
                if (paintBox != null) paintBox.enabled = true;
                break;
            case PaintToolType.Stamp:
                if (paintStamp != null) paintStamp.enabled = true;
                break;
        }
    }
    
    void UpdatePaintColor()
    {
        Color color = paintColors[currentColorIndex];
        
        if (paintSphere != null) paintSphere.Color = color;
        if (paintDecal != null) paintDecal.Color = color;
        if (paintLine != null) paintLine.Color = color;
        if (paintFill != null) paintFill.Color = color;
        if (paintBox != null) paintBox.Color = color;
        if (paintStamp != null) paintStamp.Color = color;
    }
    
    void UpdatePaintSize()
    {
        float size = toolSizes[currentSizeIndex];
        
        if (paintSphere != null) paintSphere.Radius = size;
        if (paintDecal != null) paintDecal.Size = size;
        if (paintLine != null) paintLine.Size = size;
        if (paintBox != null) paintBox.Size = Vector3.one * size;
        if (paintStamp != null) paintStamp.Size = size;
    }
    
    void PerformPaint()
    {
        switch (currentTool)
        {
            case PaintToolType.Sphere:
            case PaintToolType.Decal:
            case PaintToolType.Box:
            case PaintToolType.Stamp:
                hitScreen.HandleHit();
                break;
            case PaintToolType.Line:
                // Çizgi için özel işlem gerekli
                HandleLinePainting();
                break;
            case PaintToolType.Fill:
                // Dolgu için özel işlem gerekli
                HandleFillPainting();
                break;
        }
    }
    
    void HandleLinePainting()
    {
        // Çizgi boyama için özel mantık
        // Bu örnek basitleştirilmiş
        hitScreen.HandleHit();
    }
    
    void HandleFillPainting()
    {
        // Dolgu boyama için özel mantık
        Vector3 hitPoint = GetHitPoint();
        if (hitPoint != Vector3.zero)
        {
            paintFill.HandleHitPoint(hitPoint, 1.0f);
        }
    }
    
    Vector3 GetHitPoint()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            return hit.point;
        }
        return Vector3.zero;
    }
}
```

---

## 📊 Performans İpuçları

### Araç Seçimi
```csharp
// Performans sırası (en hızlıdan en yavaşa)
// 1. CwPaintSphere - En hızlı
// 2. CwPaintLine - Çok hızlı
// 3. CwPaintBox - Hızlı
// 4. CwPaintFill - Orta
// 5. CwPaintDecal - Orta
// 6. CwPaintStamp - Orta
```

### Optimizasyon Teknikleri
```csharp
// Boyama boyutunu sınırla
paintSphere.Radius = Mathf.Clamp(paintSphere.Radius, 0.01f, 2f);

// Şeffaflığı optimize et
paintSphere.Opacity = Mathf.Clamp(paintSphere.Opacity, 0f, 1f);

// Texture boyutunu küçült (dekal için)
paintDecal.Texture = ResizeTexture(originalTexture, 512, 512);
```

---

## 🎯 Karavan Projesi İçin Özel Ayarlar

### First-Person Boyama Araçları
```csharp
// First-person için optimize edilmiş araçlar
paintSphere.Radius = 0.05f;              // Küçük boyama (hassas)
paintDecal.Size = 0.3f;                  // Orta boyut dekal
paintLine.Size = 0.03f;                  // İnce çizgi
paintBox.Size = new Vector3(0.2f, 0.2f, 0.1f); // Küçük kutu
```

### Büyük Obje Optimizasyonu
```csharp
// Karavan boyutunda objeler için
paintSphere.Radius = 0.2f;               // Büyük boyama
paintDecal.Size = 1.0f;                  // Büyük dekal
paintLine.Size = 0.1f;                   // Kalın çizgi
paintBox.Size = new Vector3(1f, 1f, 0.2f); // Büyük kutu
```

### Çoklu Bölge Boyama
```csharp
// Karavan için farklı bölgeler ve araçlar
var bodySphere = bodyTool.AddComponent<CwPaintSphere>();    // Gövde için küre
var wheelDecal = wheelTool.AddComponent<CwPaintDecal>();    // Tekerlek için dekal
var windowLine = windowTool.AddComponent<CwPaintLine>();    // Pencere için çizgi
var doorBox = doorTool.AddComponent<CwPaintBox>();          // Kapı için kutu
```

---

## ✅ Kontrol Listesi

### Temel Kurulum
- [ ] CwPaintSphere eklendi ve çalışıyor
- [ ] CwPaintDecal eklendi ve texture atandı
- [ ] CwPaintLine eklendi ve çalışıyor
- [ ] CwPaintFill eklendi ve çalışıyor
- [ ] CwPaintBox eklendi ve çalışıyor
- [ ] CwPaintStamp eklendi ve texture atandı

### Araç Testi
- [ ] Tüm araçlar arasında geçiş çalışıyor
- [ ] Renk değiştirme tüm araçlarda çalışıyor
- [ ] Boyut değiştirme tüm araçlarda çalışıyor
- [ ] Performans kabul edilebilir

### Gelişmiş Özellikler
- [ ] Basınç duyarlılığı çalışıyor
- [ ] Çoklu araç sistemi çalışıyor
- [ ] Araç geçişleri sorunsuz
- [ ] Hata mesajı yok

---

*Önceki: [5. PaintIn3D Özellikleri](paintin3d.md) | Sonraki: [7. Boyama Türleri](gelismis-ozellikler/boyama-turleri.md)*
