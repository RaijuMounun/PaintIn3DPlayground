using System;
using System.Collections.Generic;

// Hierarchy yapısı için veri modelleri
[Serializable]
public class HierarchyTemplate
{
    public string Name;
    public List<HierarchyNode> RootNodes;
    
    public HierarchyTemplate(string name, List<HierarchyNode> rootNodes)
    {
        Name = name;
        RootNodes = rootNodes;
    }
}

[Serializable]
public class HierarchyNode
{
    public string Name;
    public List<HierarchyNode> Children;
    
    public HierarchyNode(string name, List<HierarchyNode> children = null)
    {
        Name = name;
        Children = children ?? new List<HierarchyNode>();
    }
}

// Folder yapısı için veri modelleri
[Serializable]
public class FolderTemplate
{
    public string Name;
    public List<string> FolderPaths;
    
    public FolderTemplate(string name, List<string> folderPaths)
    {
        Name = name;
        FolderPaths = folderPaths;
    }
} 