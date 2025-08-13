using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System;

public class HierarchyBuilder : EditorWindow
{
    // Seçilebilir template'ler
    private List<HierarchyTemplate> hierarchyTemplates = new List<HierarchyTemplate>();
    
    // Seçili template'ler
    private int selectedHierarchyTemplateIndex = 0;
    
    // Scroll position
    private Vector2 scrollPosition;
    
    // Window'u açmak için menü item'ı
    [MenuItem("Tools/Hierarchy & Folder Structure/Hierarchy Builder")]
    public static void ShowWindow()
    {
        GetWindow<HierarchyBuilder>("Hierarchy Builder");
    }
    
    private void OnEnable()
    {
        // Template'leri yükle
        LoadTemplates();
    }
    
    private void LoadTemplates()
    {
        // Template'leri TemplateManager üzerinden yükle
        hierarchyTemplates = TemplateManager.LoadHierarchyTemplates();
    }
    
    private void OnGUI()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Hierarchy Builder", EditorStyles.boldLabel);
        EditorGUILayout.Space(10);
        
        DrawHierarchyTemplateSection();
        
        EditorGUILayout.Space(20);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.Space(10);
        
        DrawTemplateManagementSection();
        
        EditorGUILayout.EndScrollView();
    }
    
    private void DrawHierarchyTemplateSection()
    {
        EditorGUILayout.LabelField("Hierarchy Templates", EditorStyles.boldLabel);
        EditorGUILayout.Space(5);
        
        // Template seçimi
        if (hierarchyTemplates.Count > 0)
        {
            string[] hierarchyTemplateNames = new string[hierarchyTemplates.Count];
            for (int i = 0; i < hierarchyTemplates.Count; i++)
            {
                hierarchyTemplateNames[i] = hierarchyTemplates[i].Name;
            }
            
            selectedHierarchyTemplateIndex = EditorGUILayout.Popup("Select Template:", selectedHierarchyTemplateIndex, hierarchyTemplateNames);
            
            EditorGUILayout.Space(5);
            
            // Seçili template'i görüntüle
            EditorGUILayout.LabelField("Preview:", EditorStyles.boldLabel);
            HierarchyTemplate selectedTemplate = hierarchyTemplates[selectedHierarchyTemplateIndex];
            
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUI.indentLevel++;
            foreach (var node in selectedTemplate.RootNodes)
            {
                DisplayHierarchyNode(node, 0);
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.Space(10);
            
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("Create Hierarchy Structure", GUILayout.Height(30)))
            {
                CreateHierarchyStructure(selectedTemplate);
            }
            
            // Template'i silme butonu (sadece custom template'ler için)
            if (selectedHierarchyTemplateIndex >= 3) // İlk 3 template default
            {
                if (GUILayout.Button("Delete Template", GUILayout.Height(30), GUILayout.Width(120)))
                {
                    if (EditorUtility.DisplayDialog("Delete Template", 
                        $"Are you sure you want to delete the template '{selectedTemplate.Name}'?", 
                        "Yes", "No"))
                    {
                        TemplateManager.DeleteHierarchyTemplate(selectedTemplate.Name);
                        LoadTemplates();
                        selectedHierarchyTemplateIndex = 0;
                        GUIUtility.ExitGUI();
                    }
                }
            }
            
            EditorGUILayout.EndHorizontal();
        }
        else
        {
            EditorGUILayout.HelpBox("No hierarchy templates found.", MessageType.Info);
        }
    }
    
    private void DisplayHierarchyNode(HierarchyNode node, int depth)
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(depth * 20);
        EditorGUILayout.LabelField(node.Name);
        EditorGUILayout.EndHorizontal();
        
        foreach (var child in node.Children)
        {
            DisplayHierarchyNode(child, depth + 1);
        }
    }
    
    private void DrawTemplateManagementSection()
    {
        EditorGUILayout.LabelField("Template Management", EditorStyles.boldLabel);
        EditorGUILayout.Space(5);
        
        if (GUILayout.Button("Create New Hierarchy Template", GUILayout.Height(30)))
        {
            HierarchyTemplateEditorWindow.ShowWindow();
        }
        
        EditorGUILayout.Space(5);
        
        if (GUILayout.Button("Refresh Templates", GUILayout.Height(25)))
        {
            LoadTemplates();
        }
        
        EditorGUILayout.Space(10);
        
        if (GUILayout.Button("Open Folder Structure Builder", GUILayout.Height(30)))
        {
            FolderStructureBuilder.ShowWindow();
        }
    }
    
    private void CreateHierarchyStructure(HierarchyTemplate template)
    {
        foreach (var node in template.RootNodes)
        {
            CreateHierarchyNode(node, null);
        }
        
        EditorUtility.DisplayDialog("Hierarchy Created", "Hierarchy structure has been created successfully!", "OK");
    }
    
    private void CreateHierarchyNode(HierarchyNode node, GameObject parent)
    {
        GameObject newObject = new GameObject(node.Name);
        
        if (parent != null)
        {
            newObject.transform.SetParent(parent.transform);
        }
        
        // Çocuk node'ları oluştur
        foreach (var child in node.Children)
        {
            CreateHierarchyNode(child, newObject);
        }
        
        Undo.RegisterCreatedObjectUndo(newObject, "Create Hierarchy Object");
    }
} 