using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System;

public class FolderStructureBuilder : EditorWindow
{
    // Seçilebilir template'ler
    private List<FolderTemplate> folderTemplates = new List<FolderTemplate>();
    
    // Seçili template
    private int selectedFolderTemplateIndex = 0;
    
    // Scroll position
    private Vector2 scrollPosition;
    
    // Tool penceresini açmak için menü item'ı
    [MenuItem("Tools/Hierarchy & Folder Structure/Folder Structure Builder")]
    public static void ShowWindow()
    {
        GetWindow<FolderStructureBuilder>("Folder Structure Builder");
    }
    
    private void OnEnable()
    {
        // Template'leri yükle
        LoadTemplates();
    }
    
    private void LoadTemplates()
    {
        // Template'leri TemplateManager üzerinden yükle
        folderTemplates = TemplateManager.LoadFolderTemplates();
    }
    
    private void OnGUI()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Folder Structure Builder", EditorStyles.boldLabel);
        EditorGUILayout.Space(10);
        
        DrawFolderTemplateSection();
        
        EditorGUILayout.Space(20);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.Space(10);
        
        DrawTemplateManagementSection();
        
        EditorGUILayout.EndScrollView();
    }
    
    private void DrawFolderTemplateSection()
    {
        EditorGUILayout.LabelField("Folder Structure Templates", EditorStyles.boldLabel);
        EditorGUILayout.Space(5);
        
        // Template seçimi
        if (folderTemplates.Count <= 0) {
            EditorGUILayout.HelpBox("No folder templates found.", MessageType.Info);
            return;
        }

        // Template isimlerini al
        string[] folderTemplateNames = new string[folderTemplates.Count];
        for (int i = 0; i < folderTemplates.Count; i++)
        {
            folderTemplateNames[i] = folderTemplates[i].Name;
        }
        
        selectedFolderTemplateIndex = EditorGUILayout.Popup("Select Template:", selectedFolderTemplateIndex, folderTemplateNames);
        
        EditorGUILayout.Space(5);
        
        // Seçili template'i görüntüle
        EditorGUILayout.LabelField("Preview:", EditorStyles.boldLabel);
        FolderTemplate selectedTemplate = folderTemplates[selectedFolderTemplateIndex];
        
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUI.indentLevel++;
        foreach (var folder in selectedTemplate.FolderPaths)
        {
            EditorGUILayout.LabelField(folder);
        }
        EditorGUI.indentLevel--;
        EditorGUILayout.EndVertical();
        
        EditorGUILayout.Space(10);
        
        EditorGUILayout.BeginHorizontal();
        
        if (GUILayout.Button("Create Folder Structure", GUILayout.Height(30)))
        {
            CreateFolderStructure(selectedTemplate);
        }
        
        // Template'i silme butonu (sadece custom template'ler için)
        if (selectedFolderTemplateIndex >= 3) // İlk 3 template default
        {
            if (GUILayout.Button("Delete Template", GUILayout.Height(30), GUILayout.Width(120)))
            {
                if (EditorUtility.DisplayDialog("Delete Template", 
                    $"Are you sure you want to delete the template '{selectedTemplate.Name}'?", 
                    "Yes", "No"))
                {
                    TemplateManager.DeleteFolderTemplate(selectedTemplate.Name);
                    LoadTemplates();
                    selectedFolderTemplateIndex = 0;
                    GUIUtility.ExitGUI();
                }
            }
        }
        
        EditorGUILayout.EndHorizontal();
        
    }
    
    private void DrawTemplateManagementSection()
    {
        EditorGUILayout.LabelField("Template Management", EditorStyles.boldLabel);
        EditorGUILayout.Space(5);
        
        if (GUILayout.Button("Create New Folder Template", GUILayout.Height(30)))
        {
            FolderTemplateEditorWindow.ShowWindow();
        }
        
        EditorGUILayout.Space(5);
        
        if (GUILayout.Button("Refresh Templates", GUILayout.Height(25)))
        {
            LoadTemplates();
        }
        
        EditorGUILayout.Space(10);
        
        if (GUILayout.Button("Open Hierarchy Builder", GUILayout.Height(30)))
        {
            HierarchyBuilder.ShowWindow();
        }
    }
    
    private void CreateFolderStructure(FolderTemplate template)
    {
        foreach (var folderPath in template.FolderPaths)
        {
            string fullPath = Path.Combine(Application.dataPath, folderPath);
            
            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
            }
        }
        
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Folder Structure Created", "Folder structure has been created successfully!", "OK");
    }
} 