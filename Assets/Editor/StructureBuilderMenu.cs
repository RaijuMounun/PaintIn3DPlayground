using UnityEngine;
using UnityEditor;

public static class StructureBuilderMenu
{
    [MenuItem("Tools/Hierarchy & Folder Structure/Open Builder Tools", priority = 0)]
    public static void OpenBuilderTools()
    {
        // Her iki pencereyi de aç
        HierarchyBuilder.ShowWindow();
        FolderStructureBuilder.ShowWindow();
        
        // Pencereleri düzenle
        EditorWindow hierarchyWindow = EditorWindow.GetWindow<HierarchyBuilder>();
        EditorWindow folderWindow = EditorWindow.GetWindow<FolderStructureBuilder>();
        
        // Pencereleri yan yana yerleştir
        hierarchyWindow.position = new Rect(100, 100, 400, 600);
        folderWindow.position = new Rect(500, 100, 400, 600);
    }
} 