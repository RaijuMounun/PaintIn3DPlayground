# Temel Örnekler

Paint in 3D paketinin temel kullanımını gösteren kapsamlı örnekler koleksiyonu.

## 1. Basit Boyama Örneği

### Temel Sphere Boyama
En basit boyama örneği - küre şeklinde boyama yapan temel sistem.

```csharp
using UnityEngine;
using PaintIn3D;

public class BasicPaintExample : MonoBehaviour
{
    [Header("Boyama Ayarları")]
    public Color paintColor = Color.red;
    public float paintRadius = 0.1f;
    public float paintOpacity = 1.0f;
    
    [Header("Referanslar")]
    public CwPaintableTexture paintableTexture;
    public CwPaintSphere paintSphere;
    
    private void Start()
    {
        // CwPaintableManager'ı kontrol et
        if (FindObjectOfType<CwPaintableManager>() == null)
        {
            var managerGO = new GameObject("PaintableManager");
            managerGO.AddComponent<CwPaintableManager>();
        }
        
        // Paint sphere oluştur
        if (paintSphere == null)
        {
            paintSphere = gameObject.AddComponent<CwPaintSphere>();
        }
        
        // Temel ayarları yap
        SetupPaintSphere();
    }
    
    private void SetupPaintSphere()
    {
        paintSphere.Color = paintColor;
        paintSphere.Radius = paintRadius;
        paintSphere.Opacity = paintOpacity;
        paintSphere.BlendMode = CwBlendMode.AlphaBlend;
        paintSphere.LayerMask = LayerMask.GetMask("Paintable");
        
        // Texture ata
        if (paintableTexture != null)
        {
            paintSphere.Texture = paintableTexture;
        }
    }
    
    private void Update()
    {
        // Mouse sol tuş ile boyama
        if (Input.GetMouseButton(0))
        {
            PaintAtMousePosition();
        }
    }
    
    private void PaintAtMousePosition()
    {
        // Mouse pozisyonunu dünya koordinatlarına çevir
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit))
        {
            // Hit oluştur
            var paintHit = new CwHit
            {
                Position = hit.point,
                Normal = hit.normal,
                UV = hit.textureCoord,
                Distance = hit.distance,
                Valid = true
            };
            
            // Boyama yap
            paintSphere.Paint(paintHit);
        }
    }
    
    // Renk değiştirme fonksiyonu
    public void SetPaintColor(Color newColor)
    {
        paintColor = newColor;
        paintSphere.Color = newColor;
    }
    
    // Yarıçap değiştirme fonksiyonu
    public void SetPaintRadius(float newRadius)
    {
        paintRadius = newRadius;
        paintSphere.Radius = newRadius;
    }
    
    // Opaklık değiştirme fonksiyonu
    public void SetPaintOpacity(float newOpacity)
    {
        paintOpacity = Mathf.Clamp01(newOpacity);
        paintSphere.Opacity = newOpacity;
    }
}
```

### Kurulum Adımları
1. **Sahneye CwPaintableManager ekleyin**
2. **Boyanacak nesneye CwPaintableTexture ekleyin**
3. **Bu scripti bir GameObject'e ekleyin**
4. **PaintableTexture referansını atayın**
5. **Boyanacak nesneyi "Paintable" layer'ına koyun**

## 2. Çoklu Renk Boyama Örneği

### Renk Paleti ile Boyama
Farklı renklerle boyama yapabilen gelişmiş sistem.

```csharp
using UnityEngine;
using PaintIn3D;
using System.Collections.Generic;

public class MultiColorPaintExample : MonoBehaviour
{
    [System.Serializable]
    public class PaintColor
    {
        public string name;
        public Color color;
        public KeyCode hotkey;
    }
    
    [Header("Renk Paleti")]
    public List<PaintColor> paintColors = new List<PaintColor>();
    public int currentColorIndex = 0;
    
    [Header("Boyama Ayarları")]
    public float paintRadius = 0.1f;
    public float paintOpacity = 1.0f;
    public CwBlendMode blendMode = CwBlendMode.AlphaBlend;
    
    [Header("Referanslar")]
    public CwPaintableTexture paintableTexture;
    public CwPaintSphere paintSphere;
    
    private void Start()
    {
        // Varsayılan renkleri ekle
        if (paintColors.Count == 0)
        {
            AddDefaultColors();
        }
        
        // Paint sphere oluştur
        if (paintSphere == null)
        {
            paintSphere = gameObject.AddComponent<CwPaintSphere>();
        }
        
        SetupPaintSphere();
    }
    
    private void AddDefaultColors()
    {
        paintColors.Add(new PaintColor { name = "Kırmızı", color = Color.red, hotkey = KeyCode.Alpha1 });
        paintColors.Add(new PaintColor { name = "Yeşil", color = Color.green, hotkey = KeyCode.Alpha2 });
        paintColors.Add(new PaintColor { name = "Mavi", color = Color.blue, hotkey = KeyCode.Alpha3 });
        paintColors.Add(new PaintColor { name = "Sarı", color = Color.yellow, hotkey = KeyCode.Alpha4 });
        paintColors.Add(new PaintColor { name = "Siyah", color = Color.black, hotkey = KeyCode.Alpha5 });
        paintColors.Add(new PaintColor { name = "Beyaz", color = Color.white, hotkey = KeyCode.Alpha6 });
    }
    
    private void SetupPaintSphere()
    {
        paintSphere.Radius = paintRadius;
        paintSphere.Opacity = paintOpacity;
        paintSphere.BlendMode = blendMode;
        paintSphere.LayerMask = LayerMask.GetMask("Paintable");
        
        if (paintableTexture != null)
        {
            paintSphere.Texture = paintableTexture;
        }
        
        // İlk rengi ayarla
        if (paintColors.Count > 0)
        {
            SetCurrentColor(0);
        }
    }
    
    private void Update()
    {
        // Hotkey kontrolü
        CheckHotkeys();
        
        // Mouse boyama
        if (Input.GetMouseButton(0))
        {
            PaintAtMousePosition();
        }
        
        // Mouse wheel ile yarıçap değiştirme
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            paintRadius += scroll * 0.1f;
            paintRadius = Mathf.Clamp(paintRadius, 0.01f, 1f);
            paintSphere.Radius = paintRadius;
        }
    }
    
    private void CheckHotkeys()
    {
        for (int i = 0; i < paintColors.Count; i++)
        {
            if (Input.GetKeyDown(paintColors[i].hotkey))
            {
                SetCurrentColor(i);
                break;
            }
        }
    }
    
    private void SetCurrentColor(int index)
    {
        if (index >= 0 && index < paintColors.Count)
        {
            currentColorIndex = index;
            paintSphere.Color = paintColors[index].color;
            Debug.Log($"Renk değiştirildi: {paintColors[index].name}");
        }
    }
    
    private void PaintAtMousePosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit))
        {
            var paintHit = new CwHit
            {
                Position = hit.point,
                Normal = hit.normal,
                UV = hit.textureCoord,
                Distance = hit.distance,
                Valid = true
            };
            
            paintSphere.Paint(paintHit);
        }
    }
    
    // UI için renk değiştirme fonksiyonu
    public void SetColorByIndex(int index)
    {
        SetCurrentColor(index);
    }
    
    // Renk ekleme fonksiyonu
    public void AddColor(string name, Color color, KeyCode hotkey)
    {
        paintColors.Add(new PaintColor { name = name, color = color, hotkey = hotkey });
    }
    
    // Mevcut rengi alma
    public Color GetCurrentColor()
    {
        if (currentColorIndex >= 0 && currentColorIndex < paintColors.Count)
        {
            return paintColors[currentColorIndex].color;
        }
        return Color.white;
    }
    
    // Mevcut renk adını alma
    public string GetCurrentColorName()
    {
        if (currentColorIndex >= 0 && currentColorIndex < paintColors.Count)
        {
            return paintColors[currentColorIndex].name;
        }
        return "Bilinmeyen";
    }
}
```

## 3. Dekal Boyama Örneği

### Sticker/Dekal Uygulama
Önceden tanımlanmış görselleri yüzeye uygulayan sistem.

```csharp
using UnityEngine;
using PaintIn3D;
using System.Collections.Generic;

public class DecalPaintExample : MonoBehaviour
{
    [System.Serializable]
    public class DecalData
    {
        public string name;
        public Texture2D texture;
        public float scale = 1f;
        public float rotation = 0f;
        public KeyCode hotkey;
    }
    
    [Header("Dekal Koleksiyonu")]
    public List<DecalData> decals = new List<DecalData>();
    public int currentDecalIndex = 0;
    
    [Header("Dekal Ayarları")]
    public bool randomRotation = false;
    public float randomRotationRange = 360f;
    public bool randomScale = false;
    public Vector2 randomScaleRange = new Vector2(0.5f, 1.5f);
    
    [Header("Referanslar")]
    public CwPaintableTexture paintableTexture;
    public CwPaintDecal paintDecal;
    
    private void Start()
    {
        // Varsayılan dekal'ları ekle
        if (decals.Count == 0)
        {
            AddDefaultDecals();
        }
        
        // Paint decal oluştur
        if (paintDecal == null)
        {
            paintDecal = gameObject.AddComponent<CwPaintDecal>();
        }
        
        SetupPaintDecal();
    }
    
    private void AddDefaultDecals()
    {
        // Bu örnekte varsayılan dekal'lar yok
        // Kendi texture'larınızı ekleyin
        Debug.Log("Lütfen dekal texture'larını ekleyin!");
    }
    
    private void SetupPaintDecal()
    {
        paintDecal.LayerMask = LayerMask.GetMask("Paintable");
        
        if (paintableTexture != null)
        {
            paintDecal.Texture = paintableTexture;
        }
        
        // İlk dekal'ı ayarla
        if (decals.Count > 0)
        {
            SetCurrentDecal(0);
        }
    }
    
    private void Update()
    {
        // Hotkey kontrolü
        CheckHotkeys();
        
        // Mouse sol tuş ile dekal uygula
        if (Input.GetMouseButtonDown(0))
        {
            ApplyDecalAtMousePosition();
        }
    }
    
    private void CheckHotkeys()
    {
        for (int i = 0; i < decals.Count; i++)
        {
            if (Input.GetKeyDown(decals[i].hotkey))
            {
                SetCurrentDecal(i);
                break;
            }
        }
    }
    
    private void SetCurrentDecal(int index)
    {
        if (index >= 0 && index < decals.Count)
        {
            currentDecalIndex = index;
            paintDecal.DecalTexture = decals[index].texture;
            paintDecal.Scale = decals[index].scale;
            paintDecal.Rotation = decals[index].rotation;
            Debug.Log($"Dekal değiştirildi: {decals[index].name}");
        }
    }
    
    private void ApplyDecalAtMousePosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit))
        {
            // Rastgele ayarları uygula
            if (randomRotation)
            {
                paintDecal.Rotation = Random.Range(-randomRotationRange, randomRotationRange);
            }
            
            if (randomScale)
            {
                float randomScaleValue = Random.Range(randomScaleRange.x, randomScaleRange.y);
                paintDecal.Scale = decals[currentDecalIndex].scale * randomScaleValue;
            }
            
            var paintHit = new CwHit
            {
                Position = hit.point,
                Normal = hit.normal,
                UV = hit.textureCoord,
                Distance = hit.distance,
                Valid = true
            };
            
            paintDecal.ApplyDecal(paintHit);
        }
    }
    
    // UI için dekal değiştirme fonksiyonu
    public void SetDecalByIndex(int index)
    {
        SetCurrentDecal(index);
    }
    
    // Dekal ekleme fonksiyonu
    public void AddDecal(string name, Texture2D texture, float scale, float rotation, KeyCode hotkey)
    {
        decals.Add(new DecalData 
        { 
            name = name, 
            texture = texture, 
            scale = scale, 
            rotation = rotation, 
            hotkey = hotkey 
        });
    }
    
    // Mevcut dekal adını alma
    public string GetCurrentDecalName()
    {
        if (currentDecalIndex >= 0 && currentDecalIndex < decals.Count)
        {
            return decals[currentDecalIndex].name;
        }
        return "Bilinmeyen";
    }
    
    // Rastgele dekal seçme
    public void SelectRandomDecal()
    {
        if (decals.Count > 0)
        {
            int randomIndex = Random.Range(0, decals.Count);
            SetCurrentDecal(randomIndex);
        }
    }
}
```

## 4. Çizgi Boyama Örneği

### Sürekli Çizgi Çizme
Mouse hareketlerini takip ederek sürekli çizgi çizen sistem.

```csharp
using UnityEngine;
using PaintIn3D;

public class LinePaintExample : MonoBehaviour
{
    [Header("Çizgi Ayarları")]
    public Color lineColor = Color.black;
    public float lineRadius = 0.05f;
    public float lineSpacing = 0.02f;
    public int lineSmoothing = 2;
    public bool connectPoints = true;
    
    [Header("Basınç Ayarları")]
    public bool usePressure = false;
    public float pressureScale = 1.0f;
    public AnimationCurve pressureCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    [Header("Referanslar")]
    public CwPaintableTexture paintableTexture;
    public CwPaintLine paintLine;
    
    private bool isDrawing = false;
    private Vector3 lastMousePosition;
    
    private void Start()
    {
        // Paint line oluştur
        if (paintLine == null)
        {
            paintLine = gameObject.AddComponent<CwPaintLine>();
        }
        
        SetupPaintLine();
    }
    
    private void SetupPaintLine()
    {
        paintLine.Color = lineColor;
        paintLine.Radius = lineRadius;
        paintLine.Spacing = lineSpacing;
        paintLine.Smoothing = lineSmoothing;
        paintLine.Connect = connectPoints;
        paintLine.UsePressure = usePressure;
        paintLine.PressureScale = pressureScale;
        paintLine.PressureCurve = pressureCurve;
        paintLine.LayerMask = LayerMask.GetMask("Paintable");
        
        if (paintableTexture != null)
        {
            paintLine.Texture = paintableTexture;
        }
    }
    
    private void Update()
    {
        HandleMouseInput();
    }
    
    private void HandleMouseInput()
    {
        // Mouse sol tuş basıldığında çizgi başlat
        if (Input.GetMouseButtonDown(0))
        {
            StartDrawing();
        }
        
        // Mouse sol tuş bırakıldığında çizgi bitir
        if (Input.GetMouseButtonUp(0))
        {
            StopDrawing();
        }
        
        // Mouse hareket ederken çizgi çiz
        if (isDrawing)
        {
            ContinueDrawing();
        }
    }
    
    private void StartDrawing()
    {
        isDrawing = true;
        lastMousePosition = Input.mousePosition;
        
        // Çizgi başlangıç noktasını ayarla
        Vector3 worldPosition = GetMouseWorldPosition();
        Vector2 uvPosition = GetMouseUVPosition();
        
        if (worldPosition != Vector3.zero)
        {
            paintLine.StartLine(worldPosition, uvPosition);
        }
    }
    
    private void ContinueDrawing()
    {
        Vector3 currentMousePosition = Input.mousePosition;
        
        // Mouse pozisyonu değiştiyse çizgi çiz
        if (Vector3.Distance(currentMousePosition, lastMousePosition) > 1f)
        {
            Vector3 worldPosition = GetMouseWorldPosition();
            Vector2 uvPosition = GetMouseUVPosition();
            
            if (worldPosition != Vector3.zero)
            {
                paintLine.ContinueLine(worldPosition, uvPosition);
            }
            
            lastMousePosition = currentMousePosition;
        }
    }
    
    private void StopDrawing()
    {
        isDrawing = false;
        paintLine.EndLine();
    }
    
    private Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit))
        {
            return hit.point;
        }
        
        return Vector3.zero;
    }
    
    private Vector2 GetMouseUVPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit))
        {
            return hit.textureCoord;
        }
        
        return Vector2.zero;
    }
    
    // Çizgi rengini değiştirme
    public void SetLineColor(Color newColor)
    {
        lineColor = newColor;
        paintLine.Color = newColor;
    }
    
    // Çizgi kalınlığını değiştirme
    public void SetLineRadius(float newRadius)
    {
        lineRadius = newRadius;
        paintLine.Radius = newRadius;
    }
    
    // Basınç kullanımını açma/kapama
    public void SetUsePressure(bool use)
    {
        usePressure = use;
        paintLine.UsePressure = use;
    }
    
    // Basınç ölçeğini ayarlama
    public void SetPressureScale(float scale)
    {
        pressureScale = scale;
        paintLine.PressureScale = scale;
    }
    
    // Çizgi ayarlarını sıfırlama
    public void ResetLineSettings()
    {
        lineColor = Color.black;
        lineRadius = 0.05f;
        lineSpacing = 0.02f;
        lineSmoothing = 2;
        connectPoints = true;
        
        SetupPaintLine();
    }
}
```

## 5. Doldurma Boyama Örneği

### Flood Fill (Doldurma) Sistemi
Belirli bir alanı tamamen dolduran sistem.

```csharp
using UnityEngine;
using PaintIn3D;

public class FillPaintExample : MonoBehaviour
{
    [Header("Doldurma Ayarları")]
    public Color fillColor = Color.blue;
    public Color targetColor = Color.white;
    public float tolerance = 0.1f;
    public bool fillConnected = true;
    public int maxPixels = 10000;
    
    [Header("Maske Ayarları")]
    public bool useMask = false;
    public Texture2D maskTexture;
    public float maskThreshold = 0.5f;
    public bool invertMask = false;
    
    [Header("Referanslar")]
    public CwPaintableTexture paintableTexture;
    public CwPaintFill paintFill;
    
    private void Start()
    {
        // Paint fill oluştur
        if (paintFill == null)
        {
            paintFill = gameObject.AddComponent<CwPaintFill>();
        }
        
        SetupPaintFill();
    }
    
    private void SetupPaintFill()
    {
        paintFill.Color = fillColor;
        paintFill.TargetColor = targetColor;
        paintFill.Tolerance = tolerance;
        paintFill.FillConnected = fillConnected;
        paintFill.MaxPixels = maxPixels;
        paintFill.UseMask = useMask;
        paintFill.MaskTexture = maskTexture;
        paintFill.MaskThreshold = maskThreshold;
        paintFill.InvertMask = invertMask;
        paintFill.LayerMask = LayerMask.GetMask("Paintable");
        
        if (paintableTexture != null)
        {
            paintFill.Texture = paintableTexture;
        }
    }
    
    private void Update()
    {
        // Mouse sağ tuş ile doldurma
        if (Input.GetMouseButtonDown(1))
        {
            FillAtMousePosition();
        }
        
        // F tuşu ile doldurma
        if (Input.GetKeyDown(KeyCode.F))
        {
            FillAtMousePosition();
        }
    }
    
    private void FillAtMousePosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit))
        {
            var paintHit = new CwHit
            {
                Position = hit.point,
                Normal = hit.normal,
                UV = hit.textureCoord,
                Distance = hit.distance,
                Valid = true
            };
            
            paintFill.Fill(paintHit);
        }
    }
    
    // Doldurma rengini değiştirme
    public void SetFillColor(Color newColor)
    {
        fillColor = newColor;
        paintFill.Color = newColor;
    }
    
    // Hedef rengi değiştirme
    public void SetTargetColor(Color newColor)
    {
        targetColor = newColor;
        paintFill.TargetColor = newColor;
    }
    
    // Tolerans değiştirme
    public void SetTolerance(float newTolerance)
    {
        tolerance = Mathf.Clamp01(newTolerance);
        paintFill.Tolerance = tolerance;
    }
    
    // Maske kullanımını açma/kapama
    public void SetUseMask(bool use)
    {
        useMask = use;
        paintFill.UseMask = use;
    }
    
    // Maske texture'ını ayarlama
    public void SetMaskTexture(Texture2D texture)
    {
        maskTexture = texture;
        paintFill.MaskTexture = texture;
    }
    
    // Maske eşiğini ayarlama
    public void SetMaskThreshold(float threshold)
    {
        maskThreshold = Mathf.Clamp01(threshold);
        paintFill.MaskThreshold = threshold;
    }
    
    // Maske ters çevirme
    public void SetInvertMask(bool invert)
    {
        invertMask = invert;
        paintFill.InvertMask = invert;
    }
    
    // Tüm texture'ı doldurma
    public void FillAll()
    {
        if (paintableTexture != null)
        {
            paintFill.Fill(new Vector2(0.5f, 0.5f));
        }
    }
    
    // Texture'ı temizleme
    public void ClearTexture()
    {
        if (paintableTexture != null)
        {
            paintableTexture.Clear();
        }
    }
    
    // Doldurma ayarlarını sıfırlama
    public void ResetFillSettings()
    {
        fillColor = Color.blue;
        targetColor = Color.white;
        tolerance = 0.1f;
        fillConnected = true;
        maxPixels = 10000;
        useMask = false;
        maskThreshold = 0.5f;
        invertMask = false;
        
        SetupPaintFill();
    }
}
```

## 6. Entegre Boyama Sistemi

### Tüm Boyama Araçlarını Birleştiren Sistem
Farklı boyama araçlarını tek bir sistemde birleştiren kapsamlı örnek.

```csharp
using UnityEngine;
using PaintIn3D;
using System.Collections.Generic;

public class IntegratedPaintSystem : MonoBehaviour
{
    [System.Serializable]
    public enum PaintTool
    {
        Sphere,
        Decal,
        Line,
        Fill
    }
    
    [Header("Aktif Araç")]
    public PaintTool currentTool = PaintTool.Sphere;
    
    [Header("Renk Paleti")]
    public List<Color> colorPalette = new List<Color>();
    public int currentColorIndex = 0;
    
    [Header("Araç Ayarları")]
    public float toolRadius = 0.1f;
    public float toolOpacity = 1.0f;
    public CwBlendMode blendMode = CwBlendMode.AlphaBlend;
    
    [Header("Dekal Ayarları")]
    public List<Texture2D> decalTextures = new List<Texture2D>();
    public int currentDecalIndex = 0;
    public float decalScale = 1f;
    public float decalRotation = 0f;
    
    [Header("Referanslar")]
    public CwPaintableTexture paintableTexture;
    
    // Boyama araçları
    private CwPaintSphere paintSphere;
    private CwPaintDecal paintDecal;
    private CwPaintLine paintLine;
    private CwPaintFill paintFill;
    
    private bool isDrawing = false;
    private Vector3 lastMousePosition;
    
    private void Start()
    {
        InitializeColorPalette();
        InitializePaintTools();
        SetupCurrentTool();
    }
    
    private void InitializeColorPalette()
    {
        if (colorPalette.Count == 0)
        {
            colorPalette.Add(Color.red);
            colorPalette.Add(Color.green);
            colorPalette.Add(Color.blue);
            colorPalette.Add(Color.yellow);
            colorPalette.Add(Color.cyan);
            colorPalette.Add(Color.magenta);
            colorPalette.Add(Color.black);
            colorPalette.Add(Color.white);
        }
    }
    
    private void InitializePaintTools()
    {
        // Sphere painter
        paintSphere = gameObject.AddComponent<CwPaintSphere>();
        paintSphere.LayerMask = LayerMask.GetMask("Paintable");
        if (paintableTexture != null) paintSphere.Texture = paintableTexture;
        
        // Decal painter
        paintDecal = gameObject.AddComponent<CwPaintDecal>();
        paintDecal.LayerMask = LayerMask.GetMask("Paintable");
        if (paintableTexture != null) paintDecal.Texture = paintableTexture;
        
        // Line painter
        paintLine = gameObject.AddComponent<CwPaintLine>();
        paintLine.LayerMask = LayerMask.GetMask("Paintable");
        if (paintableTexture != null) paintLine.Texture = paintableTexture;
        
        // Fill painter
        paintFill = gameObject.AddComponent<CwPaintFill>();
        paintFill.LayerMask = LayerMask.GetMask("Paintable");
        if (paintableTexture != null) paintFill.Texture = paintableTexture;
    }
    
    private void Update()
    {
        HandleToolSwitching();
        HandleColorSwitching();
        HandleMouseInput();
        HandleKeyboardInput();
    }
    
    private void HandleToolSwitching()
    {
        // 1-4 tuşları ile araç değiştirme
        if (Input.GetKeyDown(KeyCode.Alpha1)) SetTool(PaintTool.Sphere);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SetTool(PaintTool.Decal);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SetTool(PaintTool.Line);
        if (Input.GetKeyDown(KeyCode.Alpha4)) SetTool(PaintTool.Fill);
    }
    
    private void HandleColorSwitching()
    {
        // Q-P tuşları ile renk değiştirme
        for (int i = 0; i < Mathf.Min(colorPalette.Count, 10); i++)
        {
            if (Input.GetKeyDown(KeyCode.Q + i))
            {
                SetColor(i);
                break;
            }
        }
    }
    
    private void HandleMouseInput()
    {
        switch (currentTool)
        {
            case PaintTool.Sphere:
                HandleSphereInput();
                break;
            case PaintTool.Decal:
                HandleDecalInput();
                break;
            case PaintTool.Line:
                HandleLineInput();
                break;
            case PaintTool.Fill:
                HandleFillInput();
                break;
        }
    }
    
    private void HandleSphereInput()
    {
        if (Input.GetMouseButton(0))
        {
            PaintAtMousePosition();
        }
    }
    
    private void HandleDecalInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ApplyDecalAtMousePosition();
        }
    }
    
    private void HandleLineInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartLine();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            EndLine();
        }
        else if (Input.GetMouseButton(0))
        {
            ContinueLine();
        }
    }
    
    private void HandleFillInput()
    {
        if (Input.GetMouseButtonDown(1)) // Sağ tuş
        {
            FillAtMousePosition();
        }
    }
    
    private void HandleKeyboardInput()
    {
        // Mouse wheel ile yarıçap değiştirme
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            toolRadius += scroll * 0.1f;
            toolRadius = Mathf.Clamp(toolRadius, 0.01f, 1f);
            UpdateToolSettings();
        }
        
        // R tuşu ile rastgele renk
        if (Input.GetKeyDown(KeyCode.R))
        {
            SetRandomColor();
        }
        
        // C tuşu ile texture temizleme
        if (Input.GetKeyDown(KeyCode.C))
        {
            ClearTexture();
        }
    }
    
    private void SetTool(PaintTool tool)
    {
        currentTool = tool;
        SetupCurrentTool();
        Debug.Log($"Araç değiştirildi: {tool}");
    }
    
    private void SetupCurrentTool()
    {
        UpdateToolSettings();
        
        switch (currentTool)
        {
            case PaintTool.Sphere:
                paintSphere.enabled = true;
                paintDecal.enabled = false;
                paintLine.enabled = false;
                paintFill.enabled = false;
                break;
            case PaintTool.Decal:
                paintSphere.enabled = false;
                paintDecal.enabled = true;
                paintLine.enabled = false;
                paintFill.enabled = false;
                break;
            case PaintTool.Line:
                paintSphere.enabled = false;
                paintDecal.enabled = false;
                paintLine.enabled = true;
                paintFill.enabled = false;
                break;
            case PaintTool.Fill:
                paintSphere.enabled = false;
                paintDecal.enabled = false;
                paintLine.enabled = false;
                paintFill.enabled = true;
                break;
        }
    }
    
    private void UpdateToolSettings()
    {
        Color currentColor = GetCurrentColor();
        
        // Sphere ayarları
        paintSphere.Color = currentColor;
        paintSphere.Radius = toolRadius;
        paintSphere.Opacity = toolOpacity;
        paintSphere.BlendMode = blendMode;
        
        // Decal ayarları
        if (decalTextures.Count > 0)
        {
            paintDecal.DecalTexture = decalTextures[currentDecalIndex];
        }
        paintDecal.Scale = decalScale;
        paintDecal.Rotation = decalRotation;
        
        // Line ayarları
        paintLine.Color = currentColor;
        paintLine.Radius = toolRadius;
        
        // Fill ayarları
        paintFill.Color = currentColor;
    }
    
    private void SetColor(int index)
    {
        if (index >= 0 && index < colorPalette.Count)
        {
            currentColorIndex = index;
            UpdateToolSettings();
            Debug.Log($"Renk değiştirildi: {colorPalette[index]}");
        }
    }
    
    private void SetRandomColor()
    {
        int randomIndex = Random.Range(0, colorPalette.Count);
        SetColor(randomIndex);
    }
    
    private Color GetCurrentColor()
    {
        if (currentColorIndex >= 0 && currentColorIndex < colorPalette.Count)
        {
            return colorPalette[currentColorIndex];
        }
        return Color.white;
    }
    
    private void PaintAtMousePosition()
    {
        var hit = GetMouseHit();
        if (hit.Valid)
        {
            paintSphere.Paint(hit);
        }
    }
    
    private void ApplyDecalAtMousePosition()
    {
        var hit = GetMouseHit();
        if (hit.Valid)
        {
            paintDecal.ApplyDecal(hit);
        }
    }
    
    private void StartLine()
    {
        isDrawing = true;
        lastMousePosition = Input.mousePosition;
        
        var hit = GetMouseHit();
        if (hit.Valid)
        {
            paintLine.StartLine(hit.Position, hit.UV);
        }
    }
    
    private void ContinueLine()
    {
        if (isDrawing)
        {
            Vector3 currentMousePosition = Input.mousePosition;
            
            if (Vector3.Distance(currentMousePosition, lastMousePosition) > 1f)
            {
                var hit = GetMouseHit();
                if (hit.Valid)
                {
                    paintLine.ContinueLine(hit.Position, hit.UV);
                }
                
                lastMousePosition = currentMousePosition;
            }
        }
    }
    
    private void EndLine()
    {
        isDrawing = false;
        paintLine.EndLine();
    }
    
    private void FillAtMousePosition()
    {
        var hit = GetMouseHit();
        if (hit.Valid)
        {
            paintFill.Fill(hit);
        }
    }
    
    private CwHit GetMouseHit()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit))
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
    
    private void ClearTexture()
    {
        if (paintableTexture != null)
        {
            paintableTexture.Clear();
            Debug.Log("Texture temizlendi");
        }
    }
    
    // UI için public fonksiyonlar
    public void SetToolByIndex(int index)
    {
        SetTool((PaintTool)index);
    }
    
    public void SetColorByIndex(int index)
    {
        SetColor(index);
    }
    
    public void SetDecalByIndex(int index)
    {
        if (index >= 0 && index < decalTextures.Count)
        {
            currentDecalIndex = index;
            UpdateToolSettings();
        }
    }
    
    public string GetCurrentToolName()
    {
        return currentTool.ToString();
    }
    
    public string GetCurrentColorName()
    {
        Color color = GetCurrentColor();
        return $"RGB({color.r:F1}, {color.g:F1}, {color.b:F1})";
    }
}
```

Bu temel örnekler, Paint in 3D paketinin farklı özelliklerini göstermektedir. Her örnek, belirli bir boyama türüne odaklanır ve gerçek dünya kullanım senaryolarını yansıtır.
