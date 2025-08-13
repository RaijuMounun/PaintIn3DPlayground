using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System;

public static class TemplateManager
{
    private static readonly string TemplateFolder = "Assets/Editor/Templates";
    
    // Hierarchy template'lerini yükle
    public static List<HierarchyTemplate> LoadHierarchyTemplates()
    {
        List<HierarchyTemplate> templates = new List<HierarchyTemplate>();
        
        // Önce default template'leri ekle
        templates.AddRange(GetDefaultHierarchyTemplates());
        
        // Sonra kayıtlı template'leri yükle
        if (Directory.Exists(TemplateFolder))
        {
            string[] files = Directory.GetFiles(TemplateFolder, "*_HierarchyTemplate.json");
            
            foreach (var file in files)
            {
                try
                {
                    string json = File.ReadAllText(file);
                    HierarchyTemplate template = JsonUtility.FromJson<HierarchyTemplate>(json);
                    templates.Add(template);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error loading hierarchy template from {file}: {e.Message}");
                }
            }
        }
        
        return templates;
    }
    
    // Folder template'lerini yükle
    public static List<FolderTemplate> LoadFolderTemplates()
    {
        List<FolderTemplate> templates = new List<FolderTemplate>();
        
        // Önce default template'leri ekle
        templates.AddRange(GetDefaultFolderTemplates());
        
        // Sonra kayıtlı template'leri yükle
        if (Directory.Exists(TemplateFolder))
        {
            string[] files = Directory.GetFiles(TemplateFolder, "*_FolderTemplate.json");
            
            foreach (var file in files)
            {
                try
                {
                    string json = File.ReadAllText(file);
                    FolderTemplate template = JsonUtility.FromJson<FolderTemplate>(json);
                    templates.Add(template);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error loading folder template from {file}: {e.Message}");
                }
            }
        }
        
        return templates;
    }
    
    // Default hierarchy template'leri
    private static List<HierarchyTemplate> GetDefaultHierarchyTemplates()
    {
        List<HierarchyTemplate> templates = new List<HierarchyTemplate>();
        
        // Boş template
        templates.Add(new HierarchyTemplate("Empty Scene", new List<HierarchyNode>()));
        
        // Basit oyun template'i
        var basicGameTemplate = new HierarchyTemplate("Basic Game Setup", new List<HierarchyNode>
        {
            new HierarchyNode("_GameManagement", new List<HierarchyNode>
            {
                new HierarchyNode("GameManager"),
                new HierarchyNode("UIManager"),
                new HierarchyNode("AudioManager"),
                new HierarchyNode("InputManager")
            }),
            new HierarchyNode("_Environment", new List<HierarchyNode>
            {
                new HierarchyNode("Lighting"),
                new HierarchyNode("Level"),
                new HierarchyNode("Props")
            }),
            new HierarchyNode("_UI", new List<HierarchyNode>
            {
                new HierarchyNode("Canvas", new List<HierarchyNode>
                {
                    new HierarchyNode("MainMenu"),
                    new HierarchyNode("HUD"),
                    new HierarchyNode("Pause")
                }),
                new HierarchyNode("EventSystem")
            }),
            new HierarchyNode("_Cameras", new List<HierarchyNode>
            {
                new HierarchyNode("MainCamera"),
                new HierarchyNode("UICamera")
            }),
            new HierarchyNode("_Characters", new List<HierarchyNode>
            {
                new HierarchyNode("Player"),
                new HierarchyNode("NPCs")
            })
        });
        templates.Add(basicGameTemplate);
        
        // 3D oyun template'i
        var game3DTemplate = new HierarchyTemplate("3D Game Setup", new List<HierarchyNode>
        {
            new HierarchyNode("_Management", new List<HierarchyNode>
            {
                new HierarchyNode("GameManager"),
                new HierarchyNode("LevelManager"),
                new HierarchyNode("UIManager"),
                new HierarchyNode("AudioManager")
            }),
            new HierarchyNode("_Environment", new List<HierarchyNode>
            {
                new HierarchyNode("Terrain"),
                new HierarchyNode("Lighting"),
                new HierarchyNode("Props"),
                new HierarchyNode("Vegetation"),
                new HierarchyNode("Buildings")
            }),
            new HierarchyNode("_Characters", new List<HierarchyNode>
            {
                new HierarchyNode("Player"),
                new HierarchyNode("NPCs"),
                new HierarchyNode("Enemies")
            }),
            new HierarchyNode("_UI", new List<HierarchyNode>
            {
                new HierarchyNode("MainCanvas", new List<HierarchyNode>
                {
                    new HierarchyNode("MainMenu"),
                    new HierarchyNode("HUD"),
                    new HierarchyNode("PauseMenu"),
                    new HierarchyNode("InventoryUI")
                }),
                new HierarchyNode("EventSystem")
            }),
            new HierarchyNode("_Cameras", new List<HierarchyNode>
            {
                new HierarchyNode("MainCamera"),
                new HierarchyNode("CutsceneCamera")
            }),
            new HierarchyNode("_Effects", new List<HierarchyNode>
            {
                new HierarchyNode("ParticleEffects"),
                new HierarchyNode("PostProcessing")
            })
        });
        templates.Add(game3DTemplate);
        
        // 2D oyun template'i
        var game2DTemplate = new HierarchyTemplate("2D Game Setup", new List<HierarchyNode>
        {
            new HierarchyNode("_Management", new List<HierarchyNode>
            {
                new HierarchyNode("GameManager"),
                new HierarchyNode("LevelManager"),
                new HierarchyNode("UIManager"),
                new HierarchyNode("AudioManager")
            }),
            new HierarchyNode("_Environment", new List<HierarchyNode>
            {
                new HierarchyNode("Background"),
                new HierarchyNode("Midground"),
                new HierarchyNode("Foreground"),
                new HierarchyNode("Platforms"),
                new HierarchyNode("Collectibles")
            }),
            new HierarchyNode("_Characters", new List<HierarchyNode>
            {
                new HierarchyNode("Player"),
                new HierarchyNode("Enemies"),
                new HierarchyNode("NPCs")
            }),
            new HierarchyNode("_UI", new List<HierarchyNode>
            {
                new HierarchyNode("Canvas", new List<HierarchyNode>
                {
                    new HierarchyNode("MainMenu"),
                    new HierarchyNode("HUD"),
                    new HierarchyNode("PauseMenu")
                }),
                new HierarchyNode("EventSystem")
            }),
            new HierarchyNode("_Cameras", new List<HierarchyNode>
            {
                new HierarchyNode("MainCamera")
            })
        });
        templates.Add(game2DTemplate);
        
        return templates;
    }
    
    // Default folder template'leri
    private static List<FolderTemplate> GetDefaultFolderTemplates()
    {
        List<FolderTemplate> templates = new List<FolderTemplate>();
        
        // Boş template
        templates.Add(new FolderTemplate("Empty Project", new List<string>()));
        
        // Basit proje template'i
        var basicProjectTemplate = new FolderTemplate("Basic Project Structure", new List<string>
        {
            "Scripts",
            "Scripts/Managers",
            "Scripts/Player",
            "Scripts/UI",
            "Scripts/Utils",
            "Art",
            "Art/Models",
            "Art/Textures",
            "Art/Materials",
            "Art/Animations",
            "Prefabs",
            "Prefabs/UI",
            "Prefabs/Environment",
            "Prefabs/Characters",
            "Scenes",
            "Audio",
            "Audio/Music",
            "Audio/SFX",
            "Resources",
            "Plugins",
            "Editor"
        });
        templates.Add(basicProjectTemplate);
        
        // 3D oyun projesi template'i
        var game3DProjectTemplate = new FolderTemplate("3D Game Project", new List<string>
        {
            "Scripts",
            "Scripts/Core",
            "Scripts/Managers",
            "Scripts/Player",
            "Scripts/Enemy",
            "Scripts/NPC",
            "Scripts/UI",
            "Scripts/Utils",
            "Scripts/AI",
            "Scripts/Gameplay",
            "Art",
            "Art/Models",
            "Art/Models/Characters",
            "Art/Models/Environment",
            "Art/Models/Props",
            "Art/Textures",
            "Art/Materials",
            "Art/Shaders",
            "Art/Animations",
            "Art/UI",
            "Prefabs",
            "Prefabs/Characters",
            "Prefabs/Environment",
            "Prefabs/UI",
            "Prefabs/Effects",
            "Prefabs/Weapons",
            "Scenes",
            "Scenes/Levels",
            "Scenes/MainMenu",
            "Audio",
            "Audio/Music",
            "Audio/SFX",
            "Audio/Voice",
            "Audio/Ambient",
            "Resources",
            "Resources/Data",
            "Resources/Configs",
            "Editor",
            "Editor/Tools",
            "Plugins",
            "StreamingAssets"
        });
        templates.Add(game3DProjectTemplate);
        
        // 2D oyun projesi template'i
        var game2DProjectTemplate = new FolderTemplate("2D Game Project", new List<string>
        {
            "Scripts",
            "Scripts/Core",
            "Scripts/Managers",
            "Scripts/Player",
            "Scripts/Enemy",
            "Scripts/UI",
            "Scripts/Utils",
            "Art",
            "Art/Sprites",
            "Art/Sprites/Characters",
            "Art/Sprites/Environment",
            "Art/Sprites/UI",
            "Art/Sprites/Items",
            "Art/Sprites/Effects",
            "Art/Animations",
            "Art/Tilemaps",
            "Art/Materials",
            "Prefabs",
            "Prefabs/Characters",
            "Prefabs/Environment",
            "Prefabs/UI",
            "Prefabs/Items",
            "Prefabs/Effects",
            "Scenes",
            "Scenes/Levels",
            "Scenes/MainMenu",
            "Audio",
            "Audio/Music",
            "Audio/SFX",
            "Resources",
            "Resources/Data",
            "Editor",
            "Editor/Tools"
        });
        templates.Add(game2DProjectTemplate);
        
        return templates;
    }
    
    // Template'i sil
    public static void DeleteHierarchyTemplate(string templateName)
    {
        string filePath = $"{TemplateFolder}/{templateName}_HierarchyTemplate.json";
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            AssetDatabase.Refresh();
        }
    }
    
    public static void DeleteFolderTemplate(string templateName)
    {
        string filePath = $"{TemplateFolder}/{templateName}_FolderTemplate.json";
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            AssetDatabase.Refresh();
        }
    }
} 