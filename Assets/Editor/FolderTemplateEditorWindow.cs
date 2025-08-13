using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

public class FolderTemplateEditorWindow : EditorWindow
{
    private string templateName = "New Folder Template";
    private List<string> folderPaths = new List<string>();
    private string newFolderPath = "";
    private Vector2 scrollPosition;
    
    // Window'u açmak için metod
    public static void ShowWindow()
    {
        GetWindow<FolderTemplateEditorWindow>("Folder Template Editor");
    }
    
    private void OnGUI()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Folder Template Editor", EditorStyles.boldLabel);
        EditorGUILayout.Space(10);
        
        // Template adı
        templateName = EditorGUILayout.TextField("Template Name:", templateName);
        
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Folder Paths", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("Enter folder paths relative to Assets folder (e.g. 'Scripts/Managers')", MessageType.Info);
        
        // Mevcut klasör yollarını listele
        for (int i = 0; i < folderPaths.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            
            folderPaths[i] = EditorGUILayout.TextField(folderPaths[i]);
            
            if (GUILayout.Button("X", GUILayout.Width(20)))
            {
                folderPaths.RemoveAt(i);
                EditorGUILayout.EndHorizontal();
                break;
            }
            
            EditorGUILayout.EndHorizontal();
        }
        
        // Yeni klasör yolu ekleme
        EditorGUILayout.Space(5);
        EditorGUILayout.BeginHorizontal();
        
        newFolderPath = EditorGUILayout.TextField("New Folder:", newFolderPath);
        
        if (GUILayout.Button("Add", GUILayout.Width(50)))
        {
            if (!string.IsNullOrEmpty(newFolderPath))
            {
                folderPaths.Add(newFolderPath);
                newFolderPath = "";
                GUI.FocusControl(null);
            }
        }
        
        EditorGUILayout.EndHorizontal();
        
        // Yaygın klasör yapılarını ekleme butonları
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Quick Templates", EditorStyles.boldLabel);
        
        EditorGUILayout.BeginHorizontal();
        
        if (GUILayout.Button("Add Basic Structure"))
        {
            AddBasicStructure();
        }
        
        if (GUILayout.Button("Add MVC Structure"))
        {
            AddMVCStructure();
        }
        
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal();
        
        if (GUILayout.Button("Add Art Structure"))
        {
            AddArtStructure();
        }
        
        if (GUILayout.Button("Add Audio Structure"))
        {
            AddAudioStructure();
        }
        
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space(20);
        
        // Kaydetme butonu
        if (GUILayout.Button("Save Template", GUILayout.Height(30)))
        {
            SaveTemplate();
        }
        
        EditorGUILayout.EndScrollView();
    }
    
    private void AddBasicStructure()
    {
        folderPaths.AddRange(new List<string>
        {
            "Scripts",
            "Scripts/Managers",
            "Scripts/Player",
            "Scripts/UI",
            "Scripts/Utils",
            "Prefabs",
            "Scenes",
            "Resources"
        });
    }
    
    private void AddMVCStructure()
    {
        folderPaths.AddRange(new List<string>
        {
            "Scripts/Models",
            "Scripts/Views",
            "Scripts/Controllers",
            "Scripts/Services",
            "Scripts/Utils"
        });
    }
    
    private void AddArtStructure()
    {
        folderPaths.AddRange(new List<string>
        {
            "Art",
            "Art/Models",
            "Art/Textures",
            "Art/Materials",
            "Art/Animations",
            "Art/Shaders",
            "Art/UI"
        });
    }
    
    private void AddAudioStructure()
    {
        folderPaths.AddRange(new List<string>
        {
            "Audio",
            "Audio/Music",
            "Audio/SFX",
            "Audio/Ambient",
            "Audio/Voice"
        });
    }
    
    private void SaveTemplate()
    {
        if (string.IsNullOrEmpty(templateName))
        {
            EditorUtility.DisplayDialog("Error", "Template name cannot be empty!", "OK");
            return;
        }
        
        if (folderPaths.Count == 0)
        {
            EditorUtility.DisplayDialog("Error", "Template must contain at least one folder path!", "OK");
            return;
        }
        
        // Template'i oluştur
        FolderTemplate template = new FolderTemplate(templateName, folderPaths);
        
        // Template'i kaydet
        SaveTemplateToAsset(template);
        
        // Pencereyi kapat
        Close();
    }
    
    private void SaveTemplateToAsset(FolderTemplate template)
    {
        // Template'leri saklayacak klasörü oluştur
        string folderPath = "Assets/Editor/Templates";
        if (!System.IO.Directory.Exists(folderPath))
        {
            System.IO.Directory.CreateDirectory(folderPath);
            AssetDatabase.Refresh();
        }
        
        // Template'i JSON olarak kaydet
        string json = JsonUtility.ToJson(template, true);
        string filePath = $"{folderPath}/{template.Name}_FolderTemplate.json";
        System.IO.File.WriteAllText(filePath, json);
        
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Template Saved", $"Template '{template.Name}' has been saved successfully!", "OK");
    }
} 