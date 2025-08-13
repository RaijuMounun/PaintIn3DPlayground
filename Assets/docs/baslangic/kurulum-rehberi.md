# 2. Kurulum Rehberi

## 🎯 İlk Sahne Kurulumu

### Adım 1: Yeni Sahne Oluşturma

### Adım 2: Temel Bileşenleri Ekleme

#### CwPaintableManager Ekleme
```csharp
// Sahneye boş GameObject ekleyin
GameObject managerGO = new GameObject("PaintableManager");
CwPaintableManager manager = managerGO.AddComponent<CwPaintableManager>();

// Ayarları yapılandırın
manager.ReadPixelsBudget = 1000; // GPU performans ayarı
manager.OnBeginPainting += () => Debug.Log("Boyama başladı!");
```

#### Test Objesi Oluşturma
```csharp
// Küp oluşturun (test için)
GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
cube.name = "PaintableCube";

// CwPaintableMesh ekleyin
CwPaintableMesh paintableMesh = cube.AddComponent<CwPaintableMesh>();

// Material ayarlayın
Material material = new Material(Shader.Find("Standard"));
material.color = Color.white;
cube.GetComponent<Renderer>().material = material;
```

### Adım 3: Boyama Aracı Oluşturma
```csharp
// Boyama aracı GameObject'i
GameObject paintTool = new GameObject("PaintTool");
paintTool.transform.position = new Vector3(0, 2, 0);

// CwPaintSphere ekleyin
CwPaintSphere paintSphere = paintTool.AddComponent<CwPaintSphere>();
paintSphere.Color = Color.red;
paintSphere.Radius = 0.1f;
paintSphere.Opacity = 1.0f;
paintSphere.BlendMode = CwBlendMode.AlphaBlend;

// CwHitScreen ekleyin (mouse/touch için)
CwHitScreen hitScreen = paintTool.AddComponent<CwHitScreen>();
hitScreen.Camera = Camera.main;
hitScreen.Layers = LayerMask.GetMask("Default");
hitScreen.Distance = 100f;
```

---

## 🧪 Test Sahnesi Oluşturma

### Basit Test Sahnesi
```csharp
using UnityEngine;
using PaintIn3D;

public class PaintTestSetup : MonoBehaviour
{
    void Start()
    {
        SetupPaintSystem();
    }

    void SetupPaintSystem()
    {
        // 1. PaintableManager kontrolü
        var manager = FindObjectOfType<CwPaintableManager>();
        if (manager == null)
        {
            var managerGO = new GameObject("PaintableManager");
            manager = managerGO.AddComponent<CwPaintableManager>();
        }

        // 2. Test objesi oluştur
        var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.name = "TestCube";
        cube.transform.position = Vector3.zero;
        
        var paintableMesh = cube.AddComponent<CwPaintableMesh>();
        
        // 3. Material ayarla
        var material = new Material(Shader.Find("Standard"));
        material.color = Color.white;
        cube.GetComponent<Renderer>().material = material;

        // 4. Boyama aracı oluştur
        var paintTool = new GameObject("PaintTool");
        paintTool.transform.position = new Vector3(0, 2, 0);
        
        var paintSphere = paintTool.AddComponent<CwPaintSphere>();
        paintSphere.Color = Color.red;
        paintSphere.Radius = 0.1f;
        
        var hitScreen = paintTool.AddComponent<CwHitScreen>();
        hitScreen.Camera = Camera.main;
        hitScreen.Layers = LayerMask.GetMask("Default");

        Debug.Log("Paint test sistemi hazır! Mouse ile küpü boyayabilirsiniz.");
    }
}
```

### Test Sahnesini Çalıştırma
1. Yukarıdaki script'i boş GameObject'e ekleyin
2. Play butonuna basın
3. Mouse ile küpü boyamayı deneyin
4. Console'da hata mesajı olup olmadığını kontrol edin

---

## 🔧 Gelişmiş Kurulum

### Çoklu Obje Kurulumu
```csharp
public class MultiObjectPaintSetup : MonoBehaviour
{
    [Header("Paint Objects")]
    public GameObject[] paintableObjects;
    
    [Header("Paint Tools")]
    public GameObject[] paintTools;
    
    void Start()
    {
        SetupMultipleObjects();
        SetupMultipleTools();
    }
    
    void SetupMultipleObjects()
    {
        foreach (var obj in paintableObjects)
        {
            if (obj.GetComponent<CwPaintableMesh>() == null)
            {
                obj.AddComponent<CwPaintableMesh>();
            }
        }
    }
    
    void SetupMultipleTools()
    {
        foreach (var tool in paintTools)
        {
            if (tool.GetComponent<CwPaintSphere>() == null)
            {
                var paintSphere = tool.AddComponent<CwPaintSphere>();
                paintSphere.Color = Random.ColorHSV();
                paintSphere.Radius = 0.05f;
            }
            
            if (tool.GetComponent<CwHitScreen>() == null)
            {
                var hitScreen = tool.AddComponent<CwHitScreen>();
                hitScreen.Camera = Camera.main;
                hitScreen.Layers = LayerMask.GetMask("Default");
            }
        }
    }
}
```

### Layer-Based Kurulum
```csharp
public class LayerBasedPaintSetup : MonoBehaviour
{
    [Header("Layer Settings")]
    public LayerMask paintableLayers = LayerMask.GetMask("Paintable");
    public LayerMask paintToolLayers = LayerMask.GetMask("PaintTool");
    
    void Start()
    {
        SetupLayerBasedSystem();
    }
    
    void SetupLayerBasedSystem()
    {
        // Boyanabilir objeleri bul
        var paintableObjects = FindObjectsOfType<GameObject>()
            .Where(obj => (paintableLayers.value & (1 << obj.layer)) != 0);
            
        foreach (var obj in paintableObjects)
        {
            if (obj.GetComponent<CwPaintableMesh>() == null)
            {
                obj.AddComponent<CwPaintableMesh>();
            }
        }
        
        // Boyama araçlarını bul
        var paintTools = FindObjectsOfType<GameObject>()
            .Where(obj => (paintToolLayers.value & (1 << obj.layer)) != 0);
            
        foreach (var tool in paintTools)
        {
            SetupPaintTool(tool);
        }
    }
    
    void SetupPaintTool(GameObject tool)
    {
        if (tool.GetComponent<CwPaintSphere>() == null)
        {
            var paintSphere = tool.AddComponent<CwPaintSphere>();
            paintSphere.Color = Color.red;
            paintSphere.Radius = 0.1f;
        }
        
        if (tool.GetComponent<CwHitScreen>() == null)
        {
            var hitScreen = tool.AddComponent<CwHitScreen>();
            hitScreen.Camera = Camera.main;
            hitScreen.Layers = paintableLayers;
        }
    }
}
```

---

## 🎯 Sonraki Adımlar

Kurulum tamamlandıktan sonra:

1. **[Hızlı Başlangıç](hizli-baslangic.md)** ile ilk boyamanızı yapın
2. **[Temel Bileşenler](temel-bilesenler/paintcore.md)** ile derinleşin
3. **[Karavan Projesi](karavan-projesi/proje-kurulumu.md)** için özel kurulum yapın

---

*Önceki: [1. Paket Tanıtımı](paket-tanitimi.md) | Sonraki: [3. Hızlı Başlangıç](hizli-baslangic.md)*
