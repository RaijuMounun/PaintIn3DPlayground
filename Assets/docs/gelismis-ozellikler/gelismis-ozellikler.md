# Advanced Features (Gelişmiş Özellikler)

Paint in 3D paketi, gelişmiş kullanıcılar için çeşitli ileri seviye özellikler sunar. Bu bölüm, paketin en güçlü yeteneklerini detaylandırır.

## 1. Genel Bakış

Gelişmiş özellikler, standart boyama işlemlerinin ötesine geçen karmaşık ve özelleştirilebilir sistemlerdir. Bu özellikler, profesyonel projeler ve özel gereksinimler için tasarlanmıştır.

### Gelişmiş Özellik Kategorileri
- **Custom Shaders**: Özel shader entegrasyonu
- **Multi-Layer Systems**: Çoklu katman sistemleri
- **Procedural Painting**: Prosedürel boyama
- **Advanced Blending**: Gelişmiş karışım teknikleri
- **Real-Time Effects**: Gerçek zamanlı efektler

## 2. Custom Shaders (Özel Shader'lar)

### Genel Bakış
Kendi shader'larınızı entegre ederek tamamen özelleştirilebilir boyama efektleri oluşturabilirsiniz.

### Shader Entegrasyonu

#### Temel Shader Yapısı
```hlsl
Shader "Custom/PaintShader"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "white" {}
        _PaintTex ("Paint Texture", 2D) = "white" {}
        _BlendStrength ("Blend Strength", Range(0, 1)) = 1
        _PaintColor ("Paint Color", Color) = (1,1,1,1)
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
            
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };
            
            sampler2D _MainTex;
            sampler2D _PaintTex;
            float4 _MainTex_ST;
            float _BlendStrength;
            float4 _PaintColor;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 baseColor = tex2D(_MainTex, i.uv);
                fixed4 paintColor = tex2D(_PaintTex, i.uv);
                
                // Özel karışım mantığı
                fixed4 finalColor = lerp(baseColor, paintColor * _PaintColor, paintColor.a * _BlendStrength);
                
                return finalColor;
            }
            ENDCG
        }
    }
}
```

#### Shader Entegrasyon Kodu
```csharp
// Özel shader entegrasyonu
public class CustomShaderPainter : MonoBehaviour
{
    public Shader CustomPaintShader;
    public Material CustomPaintMaterial;
    
    private void Start()
    {
        if (CustomPaintShader != null)
        {
            CustomPaintMaterial = new Material(CustomPaintShader);
        }
    }
    
    public void PaintWithCustomShader(CwHit hit, Color paintColor, float blendStrength)
    {
        if (CustomPaintMaterial != null)
        {
            CustomPaintMaterial.SetColor("_PaintColor", paintColor);
            CustomPaintMaterial.SetFloat("_BlendStrength", blendStrength);
            
            // Boyama işlemi
            PaintTexture(hit, CustomPaintMaterial);
        }
    }
    
    private void PaintTexture(CwHit hit, Material material)
    {
        // Texture boyama işlemi
        var renderTexture = RenderTexture.GetTemporary(1024, 1024);
        Graphics.Blit(null, renderTexture, material);
        
        // Sonucu texture'a uygula
        ApplyToTexture(renderTexture);
        
        RenderTexture.ReleaseTemporary(renderTexture);
    }
}
```

### Gelişmiş Shader Efektleri

#### Noise-Based Painting
```hlsl
// Noise tabanlı boyama
float2 random2(float2 st)
{
    st = float2(dot(st,float2(127.1,311.7)),
                dot(st,float2(269.5,183.3)));
    return -1.0 + 2.0 * frac(sin(st) * 43758.5453123);
}

float noise(float2 st)
{
    float2 i = floor(st);
    float2 f = frac(st);
    
    float2 u = f * f * (3.0 - 2.0 * f);
    
    return lerp(lerp(dot(random2(i + float2(0.0,0.0)), f - float2(0.0,0.0)),
                     dot(random2(i + float2(1.0,0.0)), f - float2(1.0,0.0)), u.x),
                lerp(dot(random2(i + float2(0.0,1.0)), f - float2(0.0,1.0)),
                     dot(random2(i + float2(1.0,1.0)), f - float2(1.0,1.0)), u.x), u.y);
}

fixed4 frag (v2f i) : SV_Target
{
    float noiseValue = noise(i.uv * 10.0);
    fixed4 paintColor = _PaintColor * noiseValue;
    
    return paintColor;
}
```

#### Gradient-Based Painting
```hlsl
// Gradyan tabanlı boyama
fixed4 frag (v2f i) : SV_Target
{
    float2 uv = i.uv;
    
    // Radial gradient
    float distance = length(uv - float2(0.5, 0.5));
    float gradient = 1.0 - smoothstep(0.0, 0.5, distance);
    
    // Linear gradient
    float linearGradient = uv.x;
    
    // Combined gradient
    float combinedGradient = gradient * linearGradient;
    
    return _PaintColor * combinedGradient;
}
```

## 3. Multi-Layer Systems (Çoklu Katman Sistemleri)

### Genel Bakış
Birden fazla boyama katmanını yöneterek karmaşık boyama sistemleri oluşturabilirsiniz.

### Katman Yönetimi

#### Temel Katman Sistemi
```csharp
// Çoklu katman sistemi
public class MultiLayerPainter : MonoBehaviour
{
    [System.Serializable]
    public class PaintLayer
    {
        public string Name;
        public CwPaintableTexture Texture;
        public bool IsVisible = true;
        public bool IsLocked = false;
        public float Opacity = 1.0f;
        public CwBlendMode BlendMode = CwBlendMode.AlphaBlend;
    }
    
    public List<PaintLayer> Layers = new List<PaintLayer>();
    public int ActiveLayerIndex = 0;
    
    public PaintLayer ActiveLayer
    {
        get
        {
            if (ActiveLayerIndex >= 0 && ActiveLayerIndex < Layers.Count)
                return Layers[ActiveLayerIndex];
            return null;
        }
    }
    
    public void AddLayer(string name)
    {
        var newLayer = new PaintLayer
        {
            Name = name,
            Texture = CreatePaintableTexture(),
            IsVisible = true,
            IsLocked = false,
            Opacity = 1.0f,
            BlendMode = CwBlendMode.AlphaBlend
        };
        
        Layers.Add(newLayer);
    }
    
    public void RemoveLayer(int index)
    {
        if (index >= 0 && index < Layers.Count)
        {
            if (Layers[index].Texture != null)
            {
                DestroyImmediate(Layers[index].Texture);
            }
            Layers.RemoveAt(index);
            
            if (ActiveLayerIndex >= Layers.Count)
            {
                ActiveLayerIndex = Layers.Count - 1;
            }
        }
    }
    
    public void PaintOnLayer(int layerIndex, CwHit hit, Color color, float radius)
    {
        if (layerIndex >= 0 && layerIndex < Layers.Count)
        {
            var layer = Layers[layerIndex];
            if (!layer.IsLocked)
            {
                PaintOnTexture(layer.Texture, hit, color, radius);
            }
        }
    }
}
```

#### Katman Blend Sistemi
```csharp
// Katman karışım sistemi
public class LayerBlendSystem : MonoBehaviour
{
    public RenderTexture FinalTexture;
    public Material BlendMaterial;
    
    public void BlendAllLayers(List<PaintLayer> layers)
    {
        if (FinalTexture == null)
        {
            FinalTexture = new RenderTexture(1024, 1024, 0);
        }
        
        // İlk katmanı kopyala
        if (layers.Count > 0 && layers[0].IsVisible)
        {
            Graphics.Blit(layers[0].Texture, FinalTexture);
        }
        
        // Diğer katmanları karıştır
        for (int i = 1; i < layers.Count; i++)
        {
            if (layers[i].IsVisible)
            {
                BlendLayer(layers[i]);
            }
        }
    }
    
    private void BlendLayer(PaintLayer layer)
    {
        if (BlendMaterial != null)
        {
            BlendMaterial.SetTexture("_BlendTexture", layer.Texture);
            BlendMaterial.SetFloat("_Opacity", layer.Opacity);
            BlendMaterial.SetInt("_BlendMode", (int)layer.BlendMode);
            
            var tempTexture = RenderTexture.GetTemporary(FinalTexture.width, FinalTexture.height);
            Graphics.Blit(FinalTexture, tempTexture, BlendMaterial);
            Graphics.Blit(tempTexture, FinalTexture);
            RenderTexture.ReleaseTemporary(tempTexture);
        }
    }
}
```

### Katman Efektleri

#### Layer Masks
```csharp
// Katman maske sistemi
public class LayerMaskSystem : MonoBehaviour
{
    public Texture2D LayerMask;
    public float MaskThreshold = 0.5f;
    
    public bool IsMasked(Vector2 uv)
    {
        if (LayerMask != null)
        {
            Color maskColor = LayerMask.GetPixelBilinear(uv.x, uv.y);
            return maskColor.r > MaskThreshold;
        }
        return true;
    }
    
    public void PaintWithMask(CwPaintableTexture texture, CwHit hit, Color color, float radius)
    {
        if (IsMasked(hit.UV))
        {
            // Normal boyama
            PaintOnTexture(texture, hit, color, radius);
        }
    }
}
```

#### Layer Groups
```csharp
// Katman grup sistemi
public class LayerGroup : MonoBehaviour
{
    [System.Serializable]
    public class Group
    {
        public string Name;
        public List<int> LayerIndices = new List<int>();
        public bool IsCollapsed = false;
        public bool IsVisible = true;
    }
    
    public List<Group> Groups = new List<Group>();
    
    public void CreateGroup(string name, List<int> layerIndices)
    {
        var newGroup = new Group
        {
            Name = name,
            LayerIndices = new List<int>(layerIndices),
            IsCollapsed = false,
            IsVisible = true
        };
        
        Groups.Add(newGroup);
    }
    
    public void SetGroupVisibility(string groupName, bool isVisible)
    {
        var group = Groups.Find(g => g.Name == groupName);
        if (group != null)
        {
            group.IsVisible = isVisible;
            UpdateLayerVisibility(group);
        }
    }
    
    private void UpdateLayerVisibility(Group group)
    {
        foreach (int layerIndex in group.LayerIndices)
        {
            // Katman görünürlüğünü güncelle
        }
    }
}
```

## 4. Procedural Painting (Prosedürel Boyama)

### Genel Bakış
Matematiksel algoritmalar kullanarak otomatik boyama desenleri oluşturabilirsiniz.

### Noise-Based Procedural Painting

#### Perlin Noise Boyama
```csharp
// Perlin noise tabanlı boyama
public class PerlinNoisePainter : MonoBehaviour
{
    public float NoiseScale = 10f;
    public float NoiseStrength = 1f;
    public Color NoiseColor = Color.white;
    
    public void PaintWithPerlinNoise(CwPaintableTexture texture, Vector2 center, float radius)
    {
        int textureSize = texture.Width;
        
        for (int x = 0; x < textureSize; x++)
        {
            for (int y = 0; y < textureSize; y++)
            {
                Vector2 uv = new Vector2((float)x / textureSize, (float)y / textureSize);
                float distance = Vector2.Distance(uv, center);
                
                if (distance <= radius)
                {
                    // Perlin noise hesapla
                    float noiseX = uv.x * NoiseScale;
                    float noiseY = uv.y * NoiseScale;
                    float noiseValue = Mathf.PerlinNoise(noiseX, noiseY);
                    
                    // Mesafe faktörü
                    float distanceFactor = 1f - (distance / radius);
                    
                    // Final değer
                    float finalValue = noiseValue * distanceFactor * NoiseStrength;
                    
                    // Texture'a uygula
                    Color currentColor = texture.GetPixel(x, y);
                    Color newColor = Color.Lerp(currentColor, NoiseColor, finalValue);
                    texture.SetPixel(x, y, newColor);
                }
            }
        }
        
        texture.Apply();
    }
}
```

#### Fractal Noise Boyama
```csharp
// Fractal noise boyama
public class FractalNoisePainter : MonoBehaviour
{
    public int Octaves = 4;
    public float Lacunarity = 2f;
    public float Persistence = 0.5f;
    
    public float FractalNoise(Vector2 position)
    {
        float value = 0f;
        float amplitude = 1f;
        float frequency = 1f;
        
        for (int i = 0; i < Octaves; i++)
        {
            value += Mathf.PerlinNoise(position.x * frequency, position.y * frequency) * amplitude;
            amplitude *= Persistence;
            frequency *= Lacunarity;
        }
        
        return value;
    }
    
    public void PaintWithFractalNoise(CwPaintableTexture texture, Vector2 center, float radius)
    {
        int textureSize = texture.Width;
        
        for (int x = 0; x < textureSize; x++)
        {
            for (int y = 0; y < textureSize; y++)
            {
                Vector2 uv = new Vector2((float)x / textureSize, (float)y / textureSize);
                float distance = Vector2.Distance(uv, center);
                
                if (distance <= radius)
                {
                    float noiseValue = FractalNoise(uv * 10f);
                    float distanceFactor = 1f - (distance / radius);
                    float finalValue = noiseValue * distanceFactor;
                    
                    Color currentColor = texture.GetPixel(x, y);
                    Color newColor = Color.Lerp(currentColor, Color.white, finalValue);
                    texture.SetPixel(x, y, newColor);
                }
            }
        }
        
        texture.Apply();
    }
}
```

### Pattern-Based Procedural Painting

#### Geometric Patterns
```csharp
// Geometrik desen boyama
public class GeometricPatternPainter : MonoBehaviour
{
    public enum PatternType
    {
        Circles,
        Squares,
        Triangles,
        Hexagons,
        Spiral
    }
    
    public PatternType Pattern = PatternType.Circles;
    public float PatternScale = 1f;
    public Color PatternColor = Color.red;
    
    public void PaintGeometricPattern(CwPaintableTexture texture, Vector2 center, float radius)
    {
        switch (Pattern)
        {
            case PatternType.Circles:
                PaintCircles(texture, center, radius);
                break;
            case PatternType.Squares:
                PaintSquares(texture, center, radius);
                break;
            case PatternType.Triangles:
                PaintTriangles(texture, center, radius);
                break;
            case PatternType.Hexagons:
                PaintHexagons(texture, center, radius);
                break;
            case PatternType.Spiral:
                PaintSpiral(texture, center, radius);
                break;
        }
    }
    
    private void PaintCircles(CwPaintableTexture texture, Vector2 center, float radius)
    {
        int textureSize = texture.Width;
        
        for (int x = 0; x < textureSize; x++)
        {
            for (int y = 0; y < textureSize; y++)
            {
                Vector2 uv = new Vector2((float)x / textureSize, (float)y / textureSize);
                float distance = Vector2.Distance(uv, center);
                
                if (distance <= radius)
                {
                    // Dairesel desen
                    float circleValue = Mathf.Sin(distance * PatternScale * Mathf.PI * 2f) * 0.5f + 0.5f;
                    float distanceFactor = 1f - (distance / radius);
                    float finalValue = circleValue * distanceFactor;
                    
                    Color currentColor = texture.GetPixel(x, y);
                    Color newColor = Color.Lerp(currentColor, PatternColor, finalValue);
                    texture.SetPixel(x, y, newColor);
                }
            }
        }
        
        texture.Apply();
    }
    
    private void PaintSpiral(CwPaintableTexture texture, Vector2 center, float radius)
    {
        int textureSize = texture.Width;
        
        for (int x = 0; x < textureSize; x++)
        {
            for (int y = 0; y < textureSize; y++)
            {
                Vector2 uv = new Vector2((float)x / textureSize, (float)y / textureSize);
                Vector2 offset = uv - center;
                float distance = offset.magnitude;
                
                if (distance <= radius)
                {
                    // Spiral desen
                    float angle = Mathf.Atan2(offset.y, offset.x);
                    float spiralValue = Mathf.Sin(angle * PatternScale + distance * PatternScale) * 0.5f + 0.5f;
                    float distanceFactor = 1f - (distance / radius);
                    float finalValue = spiralValue * distanceFactor;
                    
                    Color currentColor = texture.GetPixel(x, y);
                    Color newColor = Color.Lerp(currentColor, PatternColor, finalValue);
                    texture.SetPixel(x, y, newColor);
                }
            }
        }
        
        texture.Apply();
    }
}
```

## 5. Real-Time Effects (Gerçek Zamanlı Efektler)

### Genel Bakış
Gerçek zamanlı olarak değişen ve etkileşimli boyama efektleri oluşturabilirsiniz.

### Dynamic Effects

#### Time-Based Effects
```csharp
// Zaman tabanlı efektler
public class TimeBasedPainter : MonoBehaviour
{
    public float AnimationSpeed = 1f;
    public Color Color1 = Color.red;
    public Color Color2 = Color.blue;
    public float EffectRadius = 0.5f;
    
    private void Update()
    {
        float time = Time.time * AnimationSpeed;
        
        // Zamanla değişen renk
        Color currentColor = Color.Lerp(Color1, Color2, Mathf.Sin(time) * 0.5f + 0.5f);
        
        // Zamanla değişen pozisyon
        Vector2 center = new Vector2(
            Mathf.Sin(time * 0.5f) * 0.3f + 0.5f,
            Mathf.Cos(time * 0.3f) * 0.3f + 0.5f
        );
        
        // Efekti uygula
        ApplyTimeEffect(center, currentColor, EffectRadius);
    }
    
    private void ApplyTimeEffect(Vector2 center, Color color, float radius)
    {
        var paintableTextures = FindObjectsOfType<CwPaintableTexture>();
        
        foreach (var texture in paintableTextures)
        {
            PaintCircle(texture, center, radius, color);
        }
    }
    
    private void PaintCircle(CwPaintableTexture texture, Vector2 center, float radius, Color color)
    {
        int textureSize = texture.Width;
        
        for (int x = 0; x < textureSize; x++)
        {
            for (int y = 0; y < textureSize; y++)
            {
                Vector2 uv = new Vector2((float)x / textureSize, (float)y / textureSize);
                float distance = Vector2.Distance(uv, center);
                
                if (distance <= radius)
                {
                    float alpha = 1f - (distance / radius);
                    Color currentColor = texture.GetPixel(x, y);
                    Color newColor = Color.Lerp(currentColor, color, alpha * 0.1f);
                    texture.SetPixel(x, y, newColor);
                }
            }
        }
        
        texture.Apply();
    }
}
```

#### Physics-Based Effects
```csharp
// Fizik tabanlı efektler
public class PhysicsBasedPainter : MonoBehaviour
{
    public float Gravity = 9.81f;
    public float Viscosity = 0.1f;
    public int ParticleCount = 100;
    
    private List<PaintParticle> particles = new List<PaintParticle>();
    
    [System.Serializable]
    public class PaintParticle
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public Color Color;
        public float Life;
        public float MaxLife;
    }
    
    private void Start()
    {
        InitializeParticles();
    }
    
    private void Update()
    {
        UpdateParticles();
        ApplyParticleEffects();
    }
    
    private void InitializeParticles()
    {
        for (int i = 0; i < ParticleCount; i++)
        {
            var particle = new PaintParticle
            {
                Position = new Vector2(Random.Range(0f, 1f), Random.Range(0f, 1f)),
                Velocity = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)),
                Color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)),
                Life = Random.Range(0f, 1f),
                MaxLife = Random.Range(1f, 3f)
            };
            
            particles.Add(particle);
        }
    }
    
    private void UpdateParticles()
    {
        for (int i = particles.Count - 1; i >= 0; i--)
        {
            var particle = particles[i];
            
            // Fizik güncellemesi
            particle.Velocity.y -= Gravity * Time.deltaTime;
            particle.Velocity *= (1f - Viscosity * Time.deltaTime);
            particle.Position += particle.Velocity * Time.deltaTime;
            
            // Yaşam güncellemesi
            particle.Life += Time.deltaTime;
            
            // Sınırlar
            if (particle.Position.x < 0f || particle.Position.x > 1f)
            {
                particle.Velocity.x *= -0.5f;
                particle.Position.x = Mathf.Clamp01(particle.Position.x);
            }
            
            if (particle.Position.y < 0f || particle.Position.y > 1f)
            {
                particle.Velocity.y *= -0.5f;
                particle.Position.y = Mathf.Clamp01(particle.Position.y);
            }
            
            // Ölüm kontrolü
            if (particle.Life >= particle.MaxLife)
            {
                particles.RemoveAt(i);
            }
        }
        
        // Yeni parçacıklar ekle
        while (particles.Count < ParticleCount)
        {
            InitializeParticles();
        }
    }
    
    private void ApplyParticleEffects()
    {
        var paintableTextures = FindObjectsOfType<CwPaintableTexture>();
        
        foreach (var texture in paintableTextures)
        {
            foreach (var particle in particles)
            {
                PaintParticle(texture, particle);
            }
        }
    }
    
    private void PaintParticle(CwPaintableTexture texture, PaintParticle particle)
    {
        float alpha = 1f - (particle.Life / particle.MaxLife);
        Color particleColor = particle.Color * alpha;
        
        PaintCircle(texture, particle.Position, 0.02f, particleColor);
    }
    
    private void PaintCircle(CwPaintableTexture texture, Vector2 center, float radius, Color color)
    {
        int textureSize = texture.Width;
        
        for (int x = 0; x < textureSize; x++)
        {
            for (int y = 0; y < textureSize; y++)
            {
                Vector2 uv = new Vector2((float)x / textureSize, (float)y / textureSize);
                float distance = Vector2.Distance(uv, center);
                
                if (distance <= radius)
                {
                    float alpha = 1f - (distance / radius);
                    Color currentColor = texture.GetPixel(x, y);
                    Color newColor = Color.Lerp(currentColor, color, alpha * 0.1f);
                    texture.SetPixel(x, y, newColor);
                }
            }
        }
        
        texture.Apply();
    }
}
```

## 6. Advanced Blending (Gelişmiş Karışım)

### Genel Bakış
Karmaşık karışım algoritmaları kullanarak gelişmiş görsel efektler oluşturabilirsiniz.

### Custom Blend Modes

#### Advanced Blend Shader
```hlsl
// Gelişmiş karışım shader
Shader "Custom/AdvancedBlend"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "white" {}
        _BlendTex ("Blend Texture", 2D) = "white" {}
        _BlendMode ("Blend Mode", Int) = 0
        _BlendStrength ("Blend Strength", Range(0, 1)) = 1
    }
    
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            sampler2D _MainTex;
            sampler2D _BlendTex;
            int _BlendMode;
            float _BlendStrength;
            
            float4 BlendModes(float4 base, float4 blend, int mode)
            {
                float4 result = base;
                
                switch (mode)
                {
                    case 0: // Normal
                        result = blend;
                        break;
                    case 1: // Multiply
                        result = base * blend;
                        break;
                    case 2: // Screen
                        result = 1 - (1 - base) * (1 - blend);
                        break;
                    case 3: // Overlay
                        result = base < 0.5 ? 2 * base * blend : 1 - 2 * (1 - base) * (1 - blend);
                        break;
                    case 4: // Soft Light
                        result = blend < 0.5 ? 2 * base * blend + base * base * (1 - 2 * blend) : sqrt(base) * (2 * blend - 1) + 2 * base * (1 - blend);
                        break;
                    case 5: // Hard Light
                        result = blend < 0.5 ? 2 * base * blend : 1 - 2 * (1 - base) * (1 - blend);
                        break;
                    case 6: // Color Dodge
                        result = base / (1 - blend);
                        break;
                    case 7: // Color Burn
                        result = 1 - (1 - base) / blend;
                        break;
                    case 8: // Linear Burn
                        result = base + blend - 1;
                        break;
                    case 9: // Linear Dodge
                        result = base + blend;
                        break;
                }
                
                return result;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 base = tex2D(_MainTex, i.uv);
                fixed4 blend = tex2D(_BlendTex, i.uv);
                
                fixed4 result = BlendModes(base, blend, _BlendMode);
                result = lerp(base, result, _BlendStrength);
                
                return result;
            }
            ENDCG
        }
    }
}
```

#### Blend Mode Manager
```csharp
// Karışım modu yöneticisi
public class AdvancedBlendManager : MonoBehaviour
{
    public enum AdvancedBlendMode
    {
        Normal,
        Multiply,
        Screen,
        Overlay,
        SoftLight,
        HardLight,
        ColorDodge,
        ColorBurn,
        LinearBurn,
        LinearDodge
    }
    
    public AdvancedBlendMode BlendMode = AdvancedBlendMode.Normal;
    public float BlendStrength = 1f;
    public Material BlendMaterial;
    
    public void ApplyAdvancedBlend(CwPaintableTexture baseTexture, CwPaintableTexture blendTexture, CwPaintableTexture resultTexture)
    {
        if (BlendMaterial != null)
        {
            BlendMaterial.SetTexture("_MainTex", baseTexture);
            BlendMaterial.SetTexture("_BlendTex", blendTexture);
            BlendMaterial.SetInt("_BlendMode", (int)BlendMode);
            BlendMaterial.SetFloat("_BlendStrength", BlendStrength);
            
            Graphics.Blit(null, resultTexture, BlendMaterial);
        }
    }
    
    public void CreateBlendEffect(CwHit hit, Color color, float radius)
    {
        // Geçici texture'lar oluştur
        var tempTexture1 = RenderTexture.GetTemporary(1024, 1024);
        var tempTexture2 = RenderTexture.GetTemporary(1024, 1024);
        
        // Boyama işlemi
        PaintOnTexture(tempTexture1, hit, color, radius);
        
        // Karışım uygula
        ApplyAdvancedBlend(tempTexture1, tempTexture2, tempTexture2);
        
        // Temizlik
        RenderTexture.ReleaseTemporary(tempTexture1);
        RenderTexture.ReleaseTemporary(tempTexture2);
    }
}
```

## 7. Integration Examples (Entegrasyon Örnekleri)

### Unity UI Integration
```csharp
// Unity UI entegrasyonu
public class UIPaintIntegration : MonoBehaviour
{
    public RawImage PaintDisplay;
    public Slider BrushSizeSlider;
    public Slider OpacitySlider;
    public Dropdown BlendModeDropdown;
    public ColorPicker ColorPicker;
    
    private CwPaintableTexture paintTexture;
    private CwPaintSphere paintSphere;
    
    private void Start()
    {
        InitializePaintSystem();
        SetupUI();
    }
    
    private void InitializePaintSystem()
    {
        // Paint texture oluştur
        paintTexture = new CwPaintableTexture();
        paintTexture.SetResolution(1024, 1024);
        
        // Paint sphere oluştur
        paintSphere = gameObject.AddComponent<CwPaintSphere>();
        paintSphere.Texture = paintTexture;
        
        // UI'da göster
        PaintDisplay.texture = paintTexture;
    }
    
    private void SetupUI()
    {
        // Slider event'leri
        BrushSizeSlider.onValueChanged.AddListener(OnBrushSizeChanged);
        OpacitySlider.onValueChanged.AddListener(OnOpacityChanged);
        BlendModeDropdown.onValueChanged.AddListener(OnBlendModeChanged);
        ColorPicker.onColorChanged.AddListener(OnColorChanged);
    }
    
    private void OnBrushSizeChanged(float value)
    {
        paintSphere.Radius = value;
    }
    
    private void OnOpacityChanged(float value)
    {
        paintSphere.Opacity = value;
    }
    
    private void OnBlendModeChanged(int index)
    {
        paintSphere.BlendMode = (CwBlendMode)index;
    }
    
    private void OnColorChanged(Color color)
    {
        paintSphere.Color = color;
    }
}
```

### VR Integration
```csharp
// VR entegrasyonu
public class VRPaintIntegration : MonoBehaviour
{
    public Transform LeftController;
    public Transform RightController;
    public LayerMask PaintableLayers;
    
    private CwPaintSphere leftPaintSphere;
    private CwPaintSphere rightPaintSphere;
    
    private void Start()
    {
        InitializeVRPaintSystem();
    }
    
    private void InitializeVRPaintSystem()
    {
        // Sol kontrolcü boyama sistemi
        leftPaintSphere = LeftController.gameObject.AddComponent<CwPaintSphere>();
        leftPaintSphere.LayerMask = PaintableLayers;
        leftPaintSphere.Radius = 0.05f;
        leftPaintSphere.Color = Color.red;
        
        // Sağ kontrolcü boyama sistemi
        rightPaintSphere = RightController.gameObject.AddComponent<CwPaintSphere>();
        rightPaintSphere.LayerMask = PaintableLayers;
        rightPaintSphere.Radius = 0.05f;
        rightPaintSphere.Color = Color.blue;
    }
    
    private void Update()
    {
        // VR input kontrolü
        CheckVRInput();
    }
    
    private void CheckVRInput()
    {
        // Sol kontrolcü trigger
        if (Input.GetAxis("LeftTrigger") > 0.5f)
        {
            leftPaintSphere.enabled = true;
        }
        else
        {
            leftPaintSphere.enabled = false;
        }
        
        // Sağ kontrolcü trigger
        if (Input.GetAxis("RightTrigger") > 0.5f)
        {
            rightPaintSphere.enabled = true;
        }
        else
        {
            rightPaintSphere.enabled = false;
        }
    }
}
```

## 8. Best Practices

### Performance Considerations
- **Shader Complexity**: Karmaşık shader'lar performansı etkiler
- **Texture Resolution**: Yüksek çözünürlük bellek kullanımını artırır
- **Real-Time Effects**: Gerçek zamanlı efektler CPU yoğun olabilir
- **Multi-Layer Management**: Çoklu katmanlar bellek kullanımını artırır

### Memory Management
- **Texture Pooling**: Texture'ları havuzda tutun
- **RenderTexture Management**: RenderTexture'ları düzgün temizleyin
- **Material Caching**: Materyalleri cache'leyin
- **Garbage Collection**: GC baskısını minimize edin

### Code Organization
- **Modular Design**: Modüler tasarım kullanın
- **Event-Driven Architecture**: Event tabanlı mimari
- **Configuration System**: Yapılandırma sistemi
- **Error Handling**: Hata yönetimi

### Testing and Validation
- **Unit Testing**: Birim testleri yazın
- **Performance Testing**: Performans testleri
- **Cross-Platform Testing**: Platformlar arası test
- **User Testing**: Kullanıcı testleri
