using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

public class HierarchyTemplateEditorWindow : EditorWindow
{
    private string templateName = "New Template";
    private List<HierarchyNodeEditor> rootNodes = new List<HierarchyNodeEditor>();
    private Vector2 scrollPosition;
    
    // Window'u açmak için metod
    public static void ShowWindow()
    {
        GetWindow<HierarchyTemplateEditorWindow>("Hierarchy Template Editor");
    }
    
    private void OnEnable()
    {
        // Başlangıçta bir boş node ekle
        if (rootNodes.Count == 0)
        {
            AddRootNode();
        }
    }
    
    private void OnGUI()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Hierarchy Template Editor", EditorStyles.boldLabel);
        EditorGUILayout.Space(10);
        
        // Template adı
        templateName = EditorGUILayout.TextField("Template Name:", templateName);
        
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Root Nodes", EditorStyles.boldLabel);
        
        // Root node'ları çiz
        for (int i = 0; i < rootNodes.Count; i++)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            
            EditorGUILayout.BeginHorizontal();
            rootNodes[i].Name = EditorGUILayout.TextField("Node Name:", rootNodes[i].Name);
            
            if (GUILayout.Button("X", GUILayout.Width(20)))
            {
                rootNodes.RemoveAt(i);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
                break;
            }
            
            EditorGUILayout.EndHorizontal();
            
            // Çocuk node'ları çiz
            DrawChildNodes(rootNodes[i].Children, 1);
            
            // Çocuk node ekleme butonu
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(20);
            if (GUILayout.Button("Add Child Node"))
            {
                rootNodes[i].Children.Add(new HierarchyNodeEditor("New Node"));
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(5);
        }
        
        // Yeni root node ekleme butonu
        if (GUILayout.Button("Add Root Node"))
        {
            AddRootNode();
        }
        
        EditorGUILayout.Space(20);
        
        // Kaydetme butonu
        if (GUILayout.Button("Save Template", GUILayout.Height(30)))
        {
            SaveTemplate();
        }
        
        EditorGUILayout.EndScrollView();
    }
    
    private void DrawChildNodes(List<HierarchyNodeEditor> nodes, int depth)
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(depth * 20);
            nodes[i].Name = EditorGUILayout.TextField("Node Name:", nodes[i].Name);
            
            if (GUILayout.Button("X", GUILayout.Width(20)))
            {
                nodes.RemoveAt(i);
                EditorGUILayout.EndHorizontal();
                break;
            }
            
            EditorGUILayout.EndHorizontal();
            
            // Çocuk node'ları çiz
            DrawChildNodes(nodes[i].Children, depth + 1);
            
            // Çocuk node ekleme butonu
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space((depth + 1) * 20);
            if (GUILayout.Button("Add Child Node"))
            {
                nodes[i].Children.Add(new HierarchyNodeEditor("New Node"));
            }
            EditorGUILayout.EndHorizontal();
        }
    }
    
    private void AddRootNode()
    {
        rootNodes.Add(new HierarchyNodeEditor("New Root Node"));
    }
    
    private void SaveTemplate()
    {
        if (string.IsNullOrEmpty(templateName))
        {
            EditorUtility.DisplayDialog("Error", "Template name cannot be empty!", "OK");
            return;
        }
        
        // Editor node'larını gerçek node'lara dönüştür
        List<HierarchyNode> finalRootNodes = new List<HierarchyNode>();
        foreach (var editorNode in rootNodes)
        {
            finalRootNodes.Add(ConvertToHierarchyNode(editorNode));
        }
        
        // Template'i oluştur
        HierarchyTemplate template = new HierarchyTemplate(templateName, finalRootNodes);
        
        // Template'i kaydet
        SaveTemplateToAsset(template);
        
        // Pencereyi kapat
        Close();
    }
    
    private HierarchyNode ConvertToHierarchyNode(HierarchyNodeEditor editorNode)
    {
        List<HierarchyNode> children = new List<HierarchyNode>();
        
        foreach (var childEditorNode in editorNode.Children)
        {
            children.Add(ConvertToHierarchyNode(childEditorNode));
        }
        
        return new HierarchyNode(editorNode.Name, children);
    }
    
    private void SaveTemplateToAsset(HierarchyTemplate template)
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
        string filePath = $"{folderPath}/{template.Name}_HierarchyTemplate.json";
        System.IO.File.WriteAllText(filePath, json);
        
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Template Saved", $"Template '{template.Name}' has been saved successfully!", "OK");
    }
}

// Hierarchy node editor sınıfı
[Serializable]
public class HierarchyNodeEditor
{
    public string Name;
    public List<HierarchyNodeEditor> Children;
    
    public HierarchyNodeEditor(string name)
    {
        Name = name;
        Children = new List<HierarchyNodeEditor>();
    }
} 