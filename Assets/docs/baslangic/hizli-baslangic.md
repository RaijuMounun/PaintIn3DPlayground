# 3. Hızlı Başlangıç

## 🚀 5 Dakikada İlk Boyama

Bu rehber, Paint in 3D paketini kullanarak 5 dakika içinde ilk boyamanızı yapmanızı sağlar. Adım adım takip edin ve hemen boyamaya başlayın!

---

## ⚡ Hızlı Kurulum

### Adım 1: PaintableManager Ekleme

Sahneye boş GameObject ekleyin ve adını "PaintableManager" yapın
Bu GameObject'e CwPaintableManager bileşenini ekleyin

**Unity Editor'da:**
1. Hierarchy'de sağ tık → **Create Empty**
2. Adını **"PaintableManager"** yapın
3. Inspector'da **Add Component** → **Paint in 3D** → **CwPaintableManager**

### Adım 2: Test Objesi Oluşturma

Küp oluşturun ve boyanabilir yapın

**Unity Editor'da:**
1. Hierarchy'de sağ tık → **3D Object** → **Cube**
2. Adını **"PaintableCube"** yapın
3. Inspector'da **Add Component** → **Paint in 3D** → **CwPaintableMesh**

### Adım 3: Boyama Aracı Oluşturma

Boyama aracı oluşturun ve ayarlayın


**Unity Editor'da:**
1. Hierarchy'de sağ tık → **Create Empty**
2. Adını **"PaintTool"** yapın
3. Position'ı **(0, 2, 0)** yapın
4. **Add Component** → **Paint in 3D** → **CwPaintSphere**
5. **Add Component** → **Paint in 3D** → **CwHitScreen**

**CwPaintSphere Ayarları:**
- **Color**: Kırmızı (255, 0, 0)
- **Radius**: 0.1
- **Opacity**: 1
- **Blend Mode**: AlphaBlend

**CwHitScreen Ayarları:**
- **Camera**: Main Camera
- **Layers**: Default
- **Distance**: 100

---

## 🎨 İlk Boyama

### Adım 4: Test Etme
1. **Play** butonuna basın
2. Mouse ile küpün üzerine tıklayın ve sürükleyin
3. Kırmızı boyama izlerini görün!

---

## 🔧 Temel Bileşenlerin Tanıtımı

### 🎯 CwPaintableManager
**Ne işe yarar?** Tüm boyama işlemlerini yöneten ana bileşen
**Nereye eklenir?** Sahneye bir kez (boş GameObject)
**Zorunlu mu?** Evet, boyama sistemi çalışması için gerekli

```csharp
// CwPaintableManager özellikleri
var manager = FindObjectOfType<CwPaintableManager>();
manager.ReadPixelsBudget = 1000; // GPU performans ayarı
manager.OnBeginPainting += () => Debug.Log("Boyama başladı!");
```

### 🎨 CwPaintableMesh
**Ne işe yarar?** Mesh tabanlı boyanabilir objeler
**Nereye eklenir?** Boyanacak mesh'in GameObject'ine
**Zorunlu bileşenler:** MeshFilter, MeshRenderer

```csharp
// CwPaintableMesh özellikleri
var paintableMesh = gameObject.AddComponent<CwPaintableMesh>();
paintableMesh.Activation = CwModel.ActivationType.Manual; // Manuel aktivasyon
```

### 🖌️ CwPaintSphere
**Ne işe yarar?** Küre şeklinde boyama aracı
**Nereye eklenir?** Boyama aracının GameObject'ine
**Temel ayarları:** Color, Radius, Opacity, BlendMode

```csharp
// CwPaintSphere özellikleri
var paintSphere = paintTool.AddComponent<CwPaintSphere>();
paintSphere.Color = Color.red;        // Boyama rengi
paintSphere.Radius = 0.1f;           // Boyama yarıçapı
paintSphere.Opacity = 1.0f;          // Şeffaflık
paintSphere.BlendMode = CwBlendMode.AlphaBlend; // Karışım modu
```

### 🖱️ CwHitScreen
**Ne işe yarar?** Ekran tabanlı boyama (mouse/touch)
**Nereye eklenir?** Boyama aracının GameObject'ine
**Temel ayarları:** Camera, Layers, Distance

```csharp
// CwHitScreen özellikleri
var hitScreen = paintTool.AddComponent<CwHitScreen>();
hitScreen.Camera = Camera.main;                    // Hangi kamera
hitScreen.Layers = LayerMask.GetMask("Default");   // Hangi layer'lar
hitScreen.Distance = 100f;                         // Maksimum mesafe
```

---

## 🎯 Basit Boyama Örneği

### Renk Değiştirme
```csharp
public class SimplePaintController : MonoBehaviour
{
    [Header("Paint Colors")]
    public Color[] colors = { Color.red, Color.blue, Color.green, Color.yellow };
    
    private CwPaintSphere paintSphere;
    private int currentColorIndex = 0;
    
    void Start()
    {
        paintSphere = GetComponent<CwPaintSphere>();
        if (paintSphere != null)
        {
            paintSphere.Color = colors[currentColorIndex];
        }
    }
    
    void Update()
    {
        // R tuşu ile renk değiştir
        if (Input.GetKeyDown(KeyCode.R))
        {
            currentColorIndex = (currentColorIndex + 1) % colors.Length;
            paintSphere.Color = colors[currentColorIndex];
            Debug.Log($"Renk değişti: {colors[currentColorIndex]}");
        }
        
        // Mouse wheel ile boyama boyutu değiştir
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            paintSphere.Radius = Mathf.Clamp(paintSphere.Radius + scroll, 0.01f, 1f);
            Debug.Log($"Boyama boyutu: {paintSphere.Radius:F2}");
        }
    }
}
```

### Kullanım:
1. Bu script'i PaintTool GameObject'ine ekleyin
2. Play modunda **R** tuşu ile renk değiştirin
3. **Mouse Wheel** ile boyama boyutunu ayarlayın

---

## 🚨 Yaygın Hatalar ve Çözümleri

### ❌ Hata 1: "No PaintableManager found"
**Belirtiler:** Console'da bu hata mesajı görünüyor
**Çözüm:** Sahneye CwPaintableManager ekleyin

### ❌ Hata 2: "No collision detected"
**Belirtiler:** Mouse ile tıklıyorsunuz ama boyama olmuyor
**Çözümler:**
1. CwHitScreen'in Camera ayarını kontrol edin
2. CwHitScreen'in Layers ayarını kontrol edin
3. Objenin Collider'ı olduğundan emin olun

### ❌ Hata 3: "Texture not updating"
**Belirtiler:** Boyama yapılıyor ama texture güncellenmiyor
**Çözümler:**
1. Material'ın doğru shader kullandığını kontrol edin
2. CwPaintableTexture'ın doğru slot'a bağlandığını kontrol edin

### ❌ Hata 4: "Performance issues"
**Belirtiler:** FPS düşüyor, lag oluyor
**Çözümler:**
1. CwPaintableManager'da ReadPixelsBudget'ı düşürün
2. CwPaintSphere'de Radius'u küçültün
3. Texture boyutunu küçültün

---

## 🎮 İnteraktif Test Sahnesi

### Gelişmiş Test Sahnesi
```csharp
using UnityEngine;
using PaintIn3D;

public class InteractivePaintTest : MonoBehaviour
{
    [Header("Paint Settings")]
    public Color[] paintColors = { Color.red, Color.blue, Color.green, Color.yellow, Color.magenta };
    public float[] brushSizes = { 0.05f, 0.1f, 0.2f, 0.5f };
    
    [Header("UI References")]
    public UnityEngine.UI.Text colorText;
    public UnityEngine.UI.Text sizeText;
    
    private CwPaintSphere paintSphere;
    private int currentColorIndex = 0;
    private int currentSizeIndex = 1;
    
    void Start()
    {
        SetupPaintSystem();
        UpdateUI();
    }
    
    void SetupPaintSystem()
    {
        // PaintableManager kontrolü
        var manager = FindObjectOfType<CwPaintableManager>();
        if (manager == null)
        {
            var managerGO = new GameObject("PaintableManager");
            manager = managerGO.AddComponent<CwPaintableManager>();
        }
        
        // Test objesi oluştur
        var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.name = "TestCube";
        cube.transform.position = Vector3.zero;
        cube.AddComponent<CwPaintableMesh>();
        
        // Material ayarla
        var material = new Material(Shader.Find("Standard"));
        material.color = Color.white;
        cube.GetComponent<Renderer>().material = material;
        
        // Boyama aracı oluştur
        var paintTool = new GameObject("PaintTool");
        paintTool.transform.position = new Vector3(0, 2, 0);
        
        paintSphere = paintTool.AddComponent<CwPaintSphere>();
        paintSphere.Color = paintColors[currentColorIndex];
        paintSphere.Radius = brushSizes[currentSizeIndex];
        paintSphere.Opacity = 1.0f;
        paintSphere.BlendMode = CwBlendMode.AlphaBlend;
        
        var hitScreen = paintTool.AddComponent<CwHitScreen>();
        hitScreen.Camera = Camera.main;
        hitScreen.Layers = LayerMask.GetMask("Default");
        
        Debug.Log("İnteraktif boyama sistemi hazır!");
        Debug.Log("Kontroller:");
        Debug.Log("R - Renk değiştir");
        Debug.Log("T - Boyama boyutu değiştir");
        Debug.Log("C - Temizle");
        Debug.Log("Mouse - Boyama yap");
    }
    
    void Update()
    {
        HandleInput();
    }
    
    void HandleInput()
    {
        // Renk değiştir
        if (Input.GetKeyDown(KeyCode.R))
        {
            currentColorIndex = (currentColorIndex + 1) % paintColors.Length;
            paintSphere.Color = paintColors[currentColorIndex];
            UpdateUI();
        }
        
        // Boyama boyutu değiştir
        if (Input.GetKeyDown(KeyCode.T))
        {
            currentSizeIndex = (currentSizeIndex + 1) % brushSizes.Length;
            paintSphere.Radius = brushSizes[currentSizeIndex];
            UpdateUI();
        }
        
        // Temizle
        if (Input.GetKeyDown(KeyCode.C))
        {
            var paintableTextures = FindObjectsOfType<CwPaintableTexture>();
            foreach (var texture in paintableTextures)
            {
                texture.Clear();
            }
            Debug.Log("Tüm boyamalar temizlendi!");
        }
    }
    
    void UpdateUI()
    {
        if (colorText != null)
        {
            colorText.text = $"Renk: {paintColors[currentColorIndex]}";
        }
        
        if (sizeText != null)
        {
            sizeText.text = $"Boyut: {brushSizes[currentSizeIndex]:F2}";
        }
    }
}
```

### Kullanım:
1. Bu script'i boş GameObject'e ekleyin
2. Play modunda test edin:
   - **R** tuşu: Renk değiştir
   - **T** tuşu: Boyama boyutu değiştir
   - **C** tuşu: Temizle
   - **Mouse**: Boyama yap

---

## ✅ Hızlı Başlangıç Kontrol Listesi

### Temel Kontroller
- [ ] CwPaintableManager sahneye eklendi
- [ ] Test objesi oluşturuldu ve CwPaintableMesh eklendi
- [ ] Boyama aracı oluşturuldu
- [ ] CwPaintSphere ve CwHitScreen eklendi
- [ ] Play modunda mouse ile boyama çalışıyor

### Gelişmiş Kontroller
- [ ] Renk değiştirme çalışıyor
- [ ] Boyama boyutu değiştirme çalışıyor
- [ ] Temizleme fonksiyonu çalışıyor
- [ ] Performans kabul edilebilir
- [ ] Hata mesajı yok

---

## 🎯 Sonraki Adımlar

Hızlı başlangıç tamamlandı! Şimdi şunları öğrenebilirsiniz:

### 🔧 Temel Bileşenler
- **[PaintCore Altyapısı](temel-bilesenler/paintcore.md)** - Temel sistem nasıl çalışır?
- **[PaintIn3D Özellikleri](temel-bilesenler/paintin3d.md)** - 3D boyama özellikleri
- **[Boyama Araçları](temel-bilesenler/boyama-araclari.md)** - Farklı boyama araçları

### ⚡ Gelişmiş Özellikler
- **[Boyama Türleri](gelismis-ozellikler/boyama-turleri.md)** - Küre, dekal, çizgi boyama
- **[Karışım Modları](gelismis-ozellikler/karisim-modlari.md)** - Alpha, additive, multiply
- **[Maskeleme Sistemi](gelismis-ozellikler/maskeleme.md)** - Belirli alanları maskeleme

### 🎮 Karavan Projesi
- **[Proje Kurulumu](karavan-projesi/proje-kurulumu.md)** - Karavan boyama sistemi
- **[First-Person Boyama](karavan-projesi/first-person-boyama.md)** - First-person kamera
- **[Boyama Araçları](karavan-projesi/boyama-araclari.md)** - Özel boyama araçları

---

## 🎉 Başarı!

Artık Paint in 3D paketini kullanarak 3D objeleri boyayabiliyorsunuz! Bu temel bilgilerle başlayarak, karavan boyama projeniz için gerekli tüm özellikleri öğrenebilirsiniz.

**🚀 Devam etmek için:** [Temel Bileşenler](temel-bilesenler/paintcore.md) bölümüne geçin!

---

*Önceki: [2. Kurulum Rehberi](kurulum-rehberi.md) | Sonraki: [4. PaintCore Altyapısı](temel-bilesenler/paintcore.md)*
