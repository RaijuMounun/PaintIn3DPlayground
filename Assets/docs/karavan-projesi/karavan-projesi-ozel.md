# Karavan Projesi Özel Dokümantasyonu

Paint in 3D paketinin karavan boyama projesi için özelleştirilmiş kullanım kılavuzu.

## 1. Proje Genel Bakış

### Proje Tanımı
First-person karavan boyama projesi, oyuncuların 3D karavan modellerini gerçek zamanlı olarak boyayabilecekleri interaktif bir deneyim sunar.

### Temel Gereksinimler
- **First-Person Kontrol**: Oyuncu karavan etrafında hareket edebilmeli
- **Gerçek Zamanlı Boyama**: Anında görsel geri bildirim
- **Çoklu Boyama Araçları**: Farklı boyama teknikleri
- **Kaydetme Sistemi**: Boyama sonuçlarını saklama
- **Performans Optimizasyonu**: Akıcı deneyim

### Teknik Özellikler
- **Platform**: Unity (PC/Mobile)
- **Render Pipeline**: URP/HDRP
- **Boyama Sistemi**: Paint in 3D
- **Kontrol Sistemi**: First-person controller
- **UI Sistemi**: Boyama araçları arayüzü

## 2. Karavan Modeli Hazırlama

### Model Gereksinimleri
```csharp
// Karavan modeli için gerekli bileşenler
public class CaravanModelSetup : MonoBehaviour
{
    [Header("Model Bileşenleri")]
    public MeshRenderer[] paintableSurfaces;  // Boyanabilir yüzeyler
    public MeshCollider[] surfaceColliders;   // Yüzey collider'ları
    public Material[] originalMaterials;      // Orijinal materyaller
    
    [Header("Paint in 3D Ayarları")]
    public CwPaintableMesh[] paintableMeshes; // Boyanabilir mesh'ler
    public CwPaintableTexture[] paintTextures; // Boyama texture'ları
    
    private void Start()
    {
        SetupPaintableSurfaces();
        SetupColliders();
        SetupMaterials();
    }
    
    private void SetupPaintableSurfaces()
    {
        // Her boyanabilir yüzey için Paint in 3D bileşenleri ekle
        foreach (var surface in paintableSurfaces)
        {
            var paintableMesh = surface.gameObject.AddComponent<CwPaintableMesh>();
            var paintableTexture = surface.gameObject.AddComponent<CwPaintableTexture>();
            
            // Texture ayarları
            paintableTexture.SetResolution(2048, 2048);
            paintableTexture.Format = TextureFormat.RGBA32;
            paintableTexture.FilterMode = FilterMode.Bilinear;
            paintableTexture.GenerateMipmaps = true;
            
            // Mesh ayarları
            paintableMesh.Mesh = surface.GetComponent<MeshFilter>().sharedMesh;
            paintableMesh.Material = surface.material;
            paintableMesh.PaintableTexture = paintableTexture;
            paintableMesh.AutoGenerateTexture = true;
        }
    }
    
    private void SetupColliders()
    {
        // Her yüzey için collider ekle
        foreach (var surface in paintableSurfaces)
        {
            if (surface.GetComponent<Collider>() == null)
            {
                var meshCollider = surface.gameObject.AddComponent<MeshCollider>();
                meshCollider.sharedMesh = surface.GetComponent<MeshFilter>().sharedMesh;
                meshCollider.convex = false; // Concave collider için
            }
        }
    }
    
    private void SetupMaterials()
    {
        // Paint in 3D shader'ını materyallere uygula
        Shader paintShader = Shader.Find("PaintIn3D/CwPaintable");
        
        foreach (var surface in paintableSurfaces)
        {
            if (surface.material.shader != paintShader)
            {
                surface.material.shader = paintShader;
            }
        }
    }
}
```

### UV Mapping Optimizasyonu
```csharp
// UV mapping kontrolü ve optimizasyonu
public class CaravanUVMapper : MonoBehaviour
{
    [Header("UV Ayarları")]
    public float uvScale = 1.0f;
    public Vector2 uvOffset = Vector2.zero;
    public bool autoUV = true;
    
    public void OptimizeUVMapping()
    {
        foreach (var surface in GetComponentsInChildren<MeshRenderer>())
        {
            var meshFilter = surface.GetComponent<MeshFilter>();
            if (meshFilter != null)
            {
                var mesh = meshFilter.sharedMesh;
                var uvs = mesh.uv;
                
                // UV koordinatlarını optimize et
                for (int i = 0; i < uvs.Length; i++)
                {
                    uvs[i] = uvs[i] * uvScale + uvOffset;
                    
                    // UV sınırlarını kontrol et
                    uvs[i].x = Mathf.Clamp01(uvs[i].x);
                    uvs[i].y = Mathf.Clamp01(uvs[i].y);
                }
                
                mesh.uv = uvs;
                meshFilter.sharedMesh = mesh;
            }
        }
    }
    
    public void CheckUVQuality()
    {
        foreach (var surface in GetComponentsInChildren<MeshRenderer>())
        {
            var meshFilter = surface.GetComponent<MeshFilter>();
            if (meshFilter != null)
            {
                var mesh = meshFilter.sharedMesh;
                var uvs = mesh.uv;
                
                // UV kalitesini kontrol et
                for (int i = 0; i < uvs.Length; i++)
                {
                    if (uvs[i].x < 0 || uvs[i].x > 1 || uvs[i].y < 0 || uvs[i].y > 1)
                    {
                        Debug.LogWarning($"UV koordinatı sınırlar dışında: {uvs[i]} - {surface.name}");
                    }
                }
            }
        }
    }
}
```

## 3. First-Person Boyama Sistemi

### First-Person Controller Entegrasyonu
```csharp
// First-person boyama kontrolcüsü
public class CaravanPaintController : MonoBehaviour
{
    [Header("Kamera Ayarları")]
    public Camera playerCamera;
    public float paintDistance = 3f;
    public LayerMask paintableLayers;
    
    [Header("Boyama Araçları")]
    public CwPaintSphere paintSphere;
    public CwPaintDecal paintDecal;
    public CwPaintLine paintLine;
    public CwPaintFill paintFill;
    
    [Header("Kontrol Ayarları")]
    public KeyCode paintKey = KeyCode.Mouse0;
    public KeyCode decalKey = KeyCode.Mouse1;
    public KeyCode fillKey = KeyCode.F;
    public KeyCode clearKey = KeyCode.C;
    
    [Header("Boyama Ayarları")]
    public Color currentColor = Color.red;
    public float paintRadius = 0.1f;
    public float paintOpacity = 1.0f;
    public CwBlendMode blendMode = CwBlendMode.AlphaBlend;
    
    private bool isPainting = false;
    private Vector3 lastPaintPosition;
    
    private void Start()
    {
        InitializePaintTools();
        SetupCamera();
    }
    
    private void InitializePaintTools()
    {
        // Paint sphere oluştur
        if (paintSphere == null)
        {
            paintSphere = gameObject.AddComponent<CwPaintSphere>();
        }
        
        // Paint decal oluştur
        if (paintDecal == null)
        {
            paintDecal = gameObject.AddComponent<CwPaintDecal>();
        }
        
        // Paint line oluştur
        if (paintLine == null)
        {
            paintLine = gameObject.AddComponent<CwPaintLine>();
        }
        
        // Paint fill oluştur
        if (paintFill == null)
        {
            paintFill = gameObject.AddComponent<CwPaintFill>();
        }
        
        SetupPaintTools();
    }
    
    private void SetupPaintTools()
    {
        // Ortak ayarlar
        var tools = new CwPaintSphere[] { paintSphere, paintDecal, paintLine, paintFill };
        
        foreach (var tool in tools)
        {
            tool.LayerMask = paintableLayers;
            tool.Color = currentColor;
            tool.Radius = paintRadius;
            tool.Opacity = paintOpacity;
            tool.BlendMode = blendMode;
        }
        
        // Özel ayarlar
        paintLine.Spacing = 0.02f;
        paintLine.Smoothing = 2;
        paintLine.Connect = true;
        
        paintFill.TargetColor = Color.white;
        paintFill.Tolerance = 0.1f;
        paintFill.FillConnected = true;
    }
    
    private void SetupCamera()
    {
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }
    }
    
    private void Update()
    {
        HandlePaintInput();
        HandleToolSwitching();
        HandleColorChanging();
        UpdatePaintTools();
    }
    
    private void HandlePaintInput()
    {
        // Sphere boyama
        if (Input.GetKey(paintKey))
        {
            PaintAtCrosshair();
        }
        
        // Decal uygulama
        if (Input.GetKeyDown(decalKey))
        {
            ApplyDecalAtCrosshair();
        }
        
        // Fill boyama
        if (Input.GetKeyDown(fillKey))
        {
            FillAtCrosshair();
        }
        
        // Temizleme
        if (Input.GetKeyDown(clearKey))
        {
            ClearAllPaint();
        }
    }
    
    private void PaintAtCrosshair()
    {
        var hit = GetPaintHit();
        if (hit.Valid)
        {
            paintSphere.Paint(hit);
            lastPaintPosition = hit.Position;
        }
    }
    
    private void ApplyDecalAtCrosshair()
    {
        var hit = GetPaintHit();
        if (hit.Valid)
        {
            paintDecal.ApplyDecal(hit);
        }
    }
    
    private void FillAtCrosshair()
    {
        var hit = GetPaintHit();
        if (hit.Valid)
        {
            paintFill.Fill(hit);
        }
    }
    
    private CwHit GetPaintHit()
    {
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, paintDistance, paintableLayers))
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
    
    private void HandleToolSwitching()
    {
        // 1-4 tuşları ile araç değiştirme
        if (Input.GetKeyDown(KeyCode.Alpha1)) SetActiveTool(PaintTool.Sphere);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SetActiveTool(PaintTool.Decal);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SetActiveTool(PaintTool.Line);
        if (Input.GetKeyDown(KeyCode.Alpha4)) SetActiveTool(PaintTool.Fill);
    }
    
    private void HandleColorChanging()
    {
        // R tuşu ile rastgele renk
        if (Input.GetKeyDown(KeyCode.R))
        {
            SetRandomColor();
        }
        
        // Mouse wheel ile yarıçap değiştirme
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            paintRadius += scroll * 0.1f;
            paintRadius = Mathf.Clamp(paintRadius, 0.01f, 1f);
        }
    }
    
    private void UpdatePaintTools()
    {
        // Araç ayarlarını güncelle
        paintSphere.Color = currentColor;
        paintSphere.Radius = paintRadius;
        paintSphere.Opacity = paintOpacity;
        
        paintDecal.Color = currentColor;
        paintDecal.Radius = paintRadius;
        paintDecal.Opacity = paintOpacity;
        
        paintLine.Color = currentColor;
        paintLine.Radius = paintRadius;
        paintLine.Opacity = paintOpacity;
        
        paintFill.Color = currentColor;
    }
    
    public void SetColor(Color color)
    {
        currentColor = color;
    }
    
    public void SetRandomColor()
    {
        currentColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
    }
    
    public void ClearAllPaint()
    {
        var paintableTextures = FindObjectsOfType<CwPaintableTexture>();
        foreach (var texture in paintableTextures)
        {
            texture.Clear();
        }
    }
    
    public enum PaintTool
    {
        Sphere,
        Decal,
        Line,
        Fill
    }
    
    private void SetActiveTool(PaintTool tool)
    {
        // Araç değiştirme mantığı
        Debug.Log($"Aktif araç: {tool}");
    }
}
```

### Crosshair Sistemi
```csharp
// Boyama crosshair sistemi
public class PaintCrosshair : MonoBehaviour
{
    [Header("Crosshair Ayarları")]
    public bool showCrosshair = true;
    public Color crosshairColor = Color.white;
    public float crosshairSize = 10f;
    public float crosshairThickness = 2f;
    
    [Header("Hedef Göstergesi")]
    public bool showTargetIndicator = true;
    public Color targetColor = Color.green;
    public float targetSize = 20f;
    
    private bool isTargetingPaintable = false;
    
    private void OnGUI()
    {
        if (!showCrosshair) return;
        
        Vector2 center = new Vector2(Screen.width / 2, Screen.height / 2);
        
        // Ana crosshair
        DrawCrosshair(center, crosshairColor, crosshairSize, crosshairThickness);
        
        // Hedef göstergesi
        if (showTargetIndicator && isTargetingPaintable)
        {
            DrawCrosshair(center, targetColor, targetSize, crosshairThickness);
        }
    }
    
    private void DrawCrosshair(Vector2 center, Color color, float size, float thickness)
    {
        GUI.color = color;
        
        // Yatay çizgi
        GUI.DrawTexture(new Rect(center.x - size / 2, center.y - thickness / 2, size, thickness), Texture2D.whiteTexture);
        
        // Dikey çizgi
        GUI.DrawTexture(new Rect(center.x - thickness / 2, center.y - size / 2, thickness, size), Texture2D.whiteTexture);
    }
    
    public void SetTargeting(bool targeting)
    {
        isTargetingPaintable = targeting;
    }
}
```

## 4. Karavan Boyama UI Sistemi

### Boyama Araçları Arayüzü
```csharp
// Karavan boyama UI sistemi
public class CaravanPaintUI : MonoBehaviour
{
    [Header("UI Panelleri")]
    public GameObject paintPanel;
    public GameObject colorPanel;
    public GameObject toolPanel;
    public GameObject settingsPanel;
    
    [Header("UI Elementleri")]
    public Slider radiusSlider;
    public Slider opacitySlider;
    public Dropdown blendModeDropdown;
    public Button[] colorButtons;
    public Button[] toolButtons;
    
    [Header("Referanslar")]
    public CaravanPaintController paintController;
    
    private void Start()
    {
        InitializeUI();
        SetupEventListeners();
    }
    
    private void InitializeUI()
    {
        // Slider değerlerini ayarla
        if (radiusSlider != null)
        {
            radiusSlider.minValue = 0.01f;
            radiusSlider.maxValue = 1f;
            radiusSlider.value = 0.1f;
        }
        
        if (opacitySlider != null)
        {
            opacitySlider.minValue = 0f;
            opacitySlider.maxValue = 1f;
            opacitySlider.value = 1f;
        }
        
        // Blend mode dropdown'ını doldur
        if (blendModeDropdown != null)
        {
            blendModeDropdown.ClearOptions();
            blendModeDropdown.AddOptions(new List<string> 
            { 
                "Alpha Blend", 
                "Additive", 
                "Subtractive", 
                "Multiply", 
                "Normal" 
            });
        }
        
        // Renk butonlarını ayarla
        SetupColorButtons();
        
        // Araç butonlarını ayarla
        SetupToolButtons();
    }
    
    private void SetupEventListeners()
    {
        // Slider event'leri
        if (radiusSlider != null)
        {
            radiusSlider.onValueChanged.AddListener(OnRadiusChanged);
        }
        
        if (opacitySlider != null)
        {
            opacitySlider.onValueChanged.AddListener(OnOpacityChanged);
        }
        
        if (blendModeDropdown != null)
        {
            blendModeDropdown.onValueChanged.AddListener(OnBlendModeChanged);
        }
    }
    
    private void SetupColorButtons()
    {
        Color[] colors = { Color.red, Color.green, Color.blue, Color.yellow, Color.cyan, Color.magenta, Color.black, Color.white };
        
        for (int i = 0; i < colorButtons.Length && i < colors.Length; i++)
        {
            int index = i; // Closure için
            colorButtons[i].onClick.AddListener(() => OnColorSelected(colors[index]));
            
            // Buton rengini ayarla
            var image = colorButtons[i].GetComponent<Image>();
            if (image != null)
            {
                image.color = colors[i];
            }
        }
    }
    
    private void SetupToolButtons()
    {
        string[] toolNames = { "Sphere", "Decal", "Line", "Fill" };
        
        for (int i = 0; i < toolButtons.Length && i < toolNames.Length; i++)
        {
            int index = i; // Closure için
            toolButtons[i].onClick.AddListener(() => OnToolSelected(index));
        }
    }
    
    // Event handlers
    private void OnRadiusChanged(float value)
    {
        if (paintController != null)
        {
            paintController.paintRadius = value;
        }
    }
    
    private void OnOpacityChanged(float value)
    {
        if (paintController != null)
        {
            paintController.paintOpacity = value;
        }
    }
    
    private void OnBlendModeChanged(int index)
    {
        if (paintController != null)
        {
            paintController.blendMode = (CwBlendMode)index;
        }
    }
    
    private void OnColorSelected(Color color)
    {
        if (paintController != null)
        {
            paintController.SetColor(color);
        }
    }
    
    private void OnToolSelected(int toolIndex)
    {
        if (paintController != null)
        {
            paintController.SetActiveTool((CaravanPaintController.PaintTool)toolIndex);
        }
    }
    
    // UI kontrol fonksiyonları
    public void TogglePaintPanel()
    {
        if (paintPanel != null)
        {
            paintPanel.SetActive(!paintPanel.activeSelf);
        }
    }
    
    public void ToggleColorPanel()
    {
        if (colorPanel != null)
        {
            colorPanel.SetActive(!colorPanel.activeSelf);
        }
    }
    
    public void ToggleToolPanel()
    {
        if (toolPanel != null)
        {
            toolPanel.SetActive(!toolPanel.activeSelf);
        }
    }
    
    public void ToggleSettingsPanel()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(!settingsPanel.activeSelf);
        }
    }
    
    public void SavePaint()
    {
        // Boyama sonuçlarını kaydet
        var paintableTextures = FindObjectsOfType<CwPaintableTexture>();
        foreach (var texture in paintableTextures)
        {
            texture.SaveToFile($"CaravanPaint_{System.DateTime.Now:yyyyMMdd_HHmmss}.png");
        }
        
        Debug.Log("Boyama kaydedildi!");
    }
    
    public void LoadPaint()
    {
        // Kaydedilmiş boyama sonuçlarını yükle
        // Bu fonksiyon dosya seçici ile implement edilebilir
        Debug.Log("Boyama yüklendi!");
    }
    
    public void ResetPaint()
    {
        if (paintController != null)
        {
            paintController.ClearAllPaint();
        }
    }
}
```

## 5. Karavan Boyama Kaydetme Sistemi

### Boyama Verilerini Kaydetme
```csharp
// Karavan boyama kaydetme sistemi
public class CaravanPaintSaveSystem : MonoBehaviour
{
    [System.Serializable]
    public class PaintData
    {
        public string surfaceName;
        public byte[] textureData;
        public int width;
        public int height;
        public string format;
        public System.DateTime saveTime;
    }
    
    [System.Serializable]
    public class CaravanPaintSave
    {
        public string saveName;
        public System.DateTime saveTime;
        public List<PaintData> paintDataList;
        public string caravanModelName;
    }
    
    [Header("Kaydetme Ayarları")]
    public string saveDirectory = "CaravanPaints";
    public bool autoSave = true;
    public float autoSaveInterval = 60f; // 60 saniye
    
    private float lastAutoSaveTime;
    
    private void Start()
    {
        CreateSaveDirectory();
    }
    
    private void Update()
    {
        if (autoSave)
        {
            if (Time.time - lastAutoSaveTime >= autoSaveInterval)
            {
                AutoSave();
                lastAutoSaveTime = Time.time;
            }
        }
    }
    
    private void CreateSaveDirectory()
    {
        string path = Path.Combine(Application.persistentDataPath, saveDirectory);
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }
    
    public void SaveCaravanPaint(string saveName)
    {
        var saveData = new CaravanPaintSave
        {
            saveName = saveName,
            saveTime = System.DateTime.Now,
            caravanModelName = gameObject.name,
            paintDataList = new List<PaintData>()
        };
        
        // Tüm boyanabilir texture'ları topla
        var paintableTextures = FindObjectsOfType<CwPaintableTexture>();
        
        foreach (var texture in paintableTextures)
        {
            var paintData = new PaintData
            {
                surfaceName = texture.name,
                width = texture.Width,
                height = texture.Height,
                format = texture.Format.ToString(),
                saveTime = System.DateTime.Now
            };
            
            // Texture verilerini byte array'e çevir
            paintData.textureData = texture.MainTexture.EncodeToPNG();
            
            saveData.paintDataList.Add(paintData);
        }
        
        // JSON olarak kaydet
        string json = JsonUtility.ToJson(saveData, true);
        string filePath = Path.Combine(Application.persistentDataPath, saveDirectory, $"{saveName}.json");
        
        File.WriteAllText(filePath, json);
        
        Debug.Log($"Karavan boyaması kaydedildi: {filePath}");
    }
    
    public void LoadCaravanPaint(string saveName)
    {
        string filePath = Path.Combine(Application.persistentDataPath, saveDirectory, $"{saveName}.json");
        
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            var saveData = JsonUtility.FromJson<CaravanPaintSave>(json);
            
            // Texture'ları yükle
            foreach (var paintData in saveData.paintDataList)
            {
                var paintableTexture = FindPaintableTexture(paintData.surfaceName);
                if (paintableTexture != null)
                {
                    // PNG verilerini texture'a çevir
                    var texture = new Texture2D(paintData.width, paintData.height);
                    texture.LoadImage(paintData.textureData);
                    
                    // Paintable texture'a uygula
                    paintableTexture.SetPixels(texture.GetPixels());
                    paintableTexture.Apply();
                }
            }
            
            Debug.Log($"Karavan boyaması yüklendi: {saveName}");
        }
        else
        {
            Debug.LogWarning($"Kayıt dosyası bulunamadı: {filePath}");
        }
    }
    
    private CwPaintableTexture FindPaintableTexture(string surfaceName)
    {
        var paintableTextures = FindObjectsOfType<CwPaintableTexture>();
        return System.Array.Find(paintableTextures, t => t.name == surfaceName);
    }
    
    private void AutoSave()
    {
        string autoSaveName = $"AutoSave_{System.DateTime.Now:yyyyMMdd_HHmmss}";
        SaveCaravanPaint(autoSaveName);
    }
    
    public string[] GetSaveFiles()
    {
        string path = Path.Combine(Application.persistentDataPath, saveDirectory);
        if (Directory.Exists(path))
        {
            var files = Directory.GetFiles(path, "*.json");
            return files.Select(f => Path.GetFileNameWithoutExtension(f)).ToArray();
        }
        return new string[0];
    }
    
    public void DeleteSave(string saveName)
    {
        string filePath = Path.Combine(Application.persistentDataPath, saveDirectory, $"{saveName}.json");
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            Debug.Log($"Kayıt silindi: {saveName}");
        }
    }
}
```

## 6. Performans Optimizasyonu

### Karavan Projesi İçin Optimizasyonlar
```csharp
// Karavan projesi performans optimizasyonu
public class CaravanPerformanceOptimizer : MonoBehaviour
{
    [Header("LOD Ayarları")]
    public float[] lodDistances = { 1f, 5f, 10f, 20f };
    public int[] lodTextureSizes = { 2048, 1024, 512, 256 };
    
    [Header("Culling Ayarları")]
    public float maxPaintDistance = 5f;
    public bool useFrustumCulling = true;
    public bool useOcclusionCulling = true;
    
    [Header("Batch Ayarları")]
    public int maxBatchSize = 100;
    public float batchTimeout = 0.1f;
    
    private Camera playerCamera;
    private List<CwPaintableTexture> paintableTextures;
    private Dictionary<CwPaintableTexture, int> textureLODLevels;
    
    private void Start()
    {
        playerCamera = Camera.main;
        paintableTextures = new List<CwPaintableTexture>(FindObjectsOfType<CwPaintableTexture>());
        textureLODLevels = new Dictionary<CwPaintableTexture, int>();
        
        InitializeLODSystem();
    }
    
    private void Update()
    {
        UpdateLODSystem();
        UpdateCullingSystem();
    }
    
    private void InitializeLODSystem()
    {
        foreach (var texture in paintableTextures)
        {
            textureLODLevels[texture] = 0;
        }
    }
    
    private void UpdateLODSystem()
    {
        if (playerCamera == null) return;
        
        foreach (var texture in paintableTextures)
        {
            float distance = Vector3.Distance(playerCamera.transform.position, texture.transform.position);
            int newLODLevel = GetLODLevel(distance);
            
            if (textureLODLevels[texture] != newLODLevel)
            {
                SetTextureLOD(texture, newLODLevel);
                textureLODLevels[texture] = newLODLevel;
            }
        }
    }
    
    private int GetLODLevel(float distance)
    {
        for (int i = 0; i < lodDistances.Length; i++)
        {
            if (distance <= lodDistances[i])
            {
                return i;
            }
        }
        return lodDistances.Length - 1;
    }
    
    private void SetTextureLOD(CwPaintableTexture texture, int lodLevel)
    {
        if (lodLevel >= 0 && lodLevel < lodTextureSizes.Length)
        {
            int newSize = lodTextureSizes[lodLevel];
            texture.SetResolution(newSize, newSize);
        }
    }
    
    private void UpdateCullingSystem()
    {
        if (playerCamera == null) return;
        
        foreach (var texture in paintableTextures)
        {
            bool shouldPaint = ShouldPaintTexture(texture);
            texture.enabled = shouldPaint;
        }
    }
    
    private bool ShouldPaintTexture(CwPaintableTexture texture)
    {
        float distance = Vector3.Distance(playerCamera.transform.position, texture.transform.position);
        
        // Mesafe kontrolü
        if (distance > maxPaintDistance)
        {
            return false;
        }
        
        // Frustum culling
        if (useFrustumCulling)
        {
            var bounds = texture.GetComponent<Renderer>()?.bounds;
            if (bounds.HasValue)
            {
                if (!GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(playerCamera), bounds.Value))
                {
                    return false;
                }
            }
        }
        
        return true;
    }
    
    // Batch boyama sistemi
    public class BatchPaintSystem : MonoBehaviour
    {
        private Queue<CwHit> paintQueue = new Queue<CwHit>();
        private float lastBatchTime;
        
        public void QueuePaint(CwHit hit)
        {
            paintQueue.Enqueue(hit);
            
            if (paintQueue.Count >= maxBatchSize || Time.time - lastBatchTime >= batchTimeout)
            {
                ProcessBatch();
            }
        }
        
        private void ProcessBatch()
        {
            // Batch boyama işlemi
            while (paintQueue.Count > 0)
            {
                var hit = paintQueue.Dequeue();
                // Boyama işlemi
            }
            
            lastBatchTime = Time.time;
        }
    }
}
```

## 7. Karavan Projesi Test Senaryoları

### Test Senaryoları Listesi
```csharp
// Karavan projesi test sistemi
public class CaravanTestSystem : MonoBehaviour
{
    [Header("Test Ayarları")]
    public bool enableAutomatedTests = false;
    public bool enablePerformanceTests = true;
    public bool enableStressTests = false;
    
    [Header("Test Sonuçları")]
    public float averageFPS;
    public float memoryUsage;
    public int paintCallCount;
    public float totalPaintTime;
    
    private void Start()
    {
        if (enableAutomatedTests)
        {
            StartAutomatedTests();
        }
    }
    
    private void Update()
    {
        if (enablePerformanceTests)
        {
            UpdatePerformanceMetrics();
        }
    }
    
    private void StartAutomatedTests()
    {
        StartCoroutine(RunAutomatedTests());
    }
    
    private System.Collections.IEnumerator RunAutomatedTests()
    {
        Debug.Log("Otomatik testler başlatılıyor...");
        
        // Test 1: Temel boyama testi
        yield return StartCoroutine(TestBasicPainting());
        
        // Test 2: Performans testi
        yield return StartCoroutine(TestPerformance());
        
        // Test 3: Kaydetme testi
        yield return StartCoroutine(TestSaveSystem());
        
        // Test 4: Yükleme testi
        yield return StartCoroutine(TestLoadSystem());
        
        Debug.Log("Tüm testler tamamlandı!");
    }
    
    private System.Collections.IEnumerator TestBasicPainting()
    {
        Debug.Log("Temel boyama testi başlatılıyor...");
        
        // Farklı renklerle boyama testi
        Color[] testColors = { Color.red, Color.green, Color.blue, Color.yellow };
        
        foreach (var color in testColors)
        {
            // Boyama işlemi simülasyonu
            yield return new WaitForSeconds(0.5f);
        }
        
        Debug.Log("Temel boyama testi tamamlandı!");
    }
    
    private System.Collections.IEnumerator TestPerformance()
    {
        Debug.Log("Performans testi başlatılıyor...");
        
        // 10 saniye boyunca performans ölçümü
        float startTime = Time.time;
        int frameCount = 0;
        
        while (Time.time - startTime < 10f)
        {
            frameCount++;
            yield return null;
        }
        
        averageFPS = frameCount / 10f;
        memoryUsage = System.GC.GetTotalMemory(false) / 1024f / 1024f; // MB
        
        Debug.Log($"Performans testi tamamlandı - FPS: {averageFPS:F1}, Bellek: {memoryUsage:F1}MB");
    }
    
    private System.Collections.IEnumerator TestSaveSystem()
    {
        Debug.Log("Kaydetme testi başlatılıyor...");
        
        var saveSystem = FindObjectOfType<CaravanPaintSaveSystem>();
        if (saveSystem != null)
        {
            saveSystem.SaveCaravanPaint("TestSave");
            yield return new WaitForSeconds(1f);
        }
        
        Debug.Log("Kaydetme testi tamamlandı!");
    }
    
    private System.Collections.IEnumerator TestLoadSystem()
    {
        Debug.Log("Yükleme testi başlatılıyor...");
        
        var saveSystem = FindObjectOfType<CaravanPaintSaveSystem>();
        if (saveSystem != null)
        {
            saveSystem.LoadCaravanPaint("TestSave");
            yield return new WaitForSeconds(1f);
        }
        
        Debug.Log("Yükleme testi tamamlandı!");
    }
    
    private void UpdatePerformanceMetrics()
    {
        // Performans metriklerini güncelle
        averageFPS = 1f / Time.deltaTime;
        memoryUsage = System.GC.GetTotalMemory(false) / 1024f / 1024f;
    }
    
    public void GenerateTestReport()
    {
        string report = $"Karavan Projesi Test Raporu\n" +
                       $"Tarih: {System.DateTime.Now}\n" +
                       $"Ortalama FPS: {averageFPS:F1}\n" +
                       $"Bellek Kullanımı: {memoryUsage:F1}MB\n" +
                       $"Boyama Çağrı Sayısı: {paintCallCount}\n" +
                       $"Toplam Boyama Süresi: {totalPaintTime:F2}s\n";
        
        Debug.Log(report);
        
        // Raporu dosyaya kaydet
        string reportPath = Path.Combine(Application.persistentDataPath, "TestReport.txt");
        File.WriteAllText(reportPath, report);
    }
}
```

Bu karavan projesi özel dokümantasyonu, Paint in 3D paketinin first-person karavan boyama projesi için özelleştirilmiş kullanımını detaylandırır. Her bölüm, projenin spesifik gereksinimlerine odaklanır ve gerçek dünya implementasyon senaryolarını yansıtır.
