using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime;
using Prota.Unity;
using UnityEditor;
using UnityEngine;

using Node = Prota.FileTree.Node;

namespace Prota.Editor
{
    public static class ResourceListUpdater
    {
        static readonly List<ResourceList> listsNeedToUpdate = new List<ResourceList>();
        
        [MenuItem("ProtaFramework/Resources/Refresh All Resource List")]
        static void RefreshAllResourceList()
        {
            listsNeedToUpdate.AddRange(Resources.LoadAll<ResourceList>("/"));
            UpdateAllNeedsToUpdate();
            UpdateResourceCollection();
        }
        
        [InitializeOnLoadMethod]
        static void Init()
        {
            AssetTree.instance.root.onUpdate += (_, e) => {
                var node = e.node;
                while(node != null)
                {
                    if(Directory.Exists(node.fullPath))
                    {
                        foreach(var f in node.files)
                        {
                            if(!File.Exists(f.fullPath)) continue;
                            if(!IsResourceList(f, out var r)) continue;
                            listsNeedToUpdate.Add(r);
                        }
                    }
                    else if(File.Exists(node.fullPath))
                    {
                        if(IsResourceList(node, out var r))
                            listsNeedToUpdate.Add(r);
                    }
                    
                    node = node.parent;
                }
            };
            
            AssetTree.instance.afterRefresh += () => {
                UpdateAllNeedsToUpdate();
                UpdateResourceCollection();
            };
        }
        
        static void UpdateAllNeedsToUpdate()
        {
            foreach(var r in listsNeedToUpdate)
            {
                var n = AssetTree.instance.root.FindFullPath(AssetDatabase.GetAssetPath(r));
                UpdateResourcesList(n);
            }
            listsNeedToUpdate.Clear();
        }
        
        static void UpdateResourceCollection()
        {
            var r = Resources.Load<ResourceCollection>("ResourceCollection");
            if(r == null) throw new Exception("a ResourceCollection must put into Resources folder.");
            var lists = Resources.LoadAll<ResourceList>("/");
            var originList = r.lists;
            r.lists = lists;
            if(originList.SameContent(r.lists)) return;
            EditorUtility.SetDirty(r);
            AssetDatabase.SaveAssetIfDirty(r);
        }
        
        static ResourceList FindResourcesListObject(Node d, out Node file)
        {
            file = null;
            var res = null as ResourceList;
            foreach(var f in d.files)
            {
                if(!IsResourceList(f, out var r)) continue;
                if(res != null) Debug.LogError($"Multiple ResourcesList found in {d.fullPath}");
                res = r;
                file = f;
            }
            return res;
        }
        
        static bool IsResourceList(Node f, out ResourceList res)
        {
            res = null;
            if(f.extension != ".asset") return false;
            var assetPath = f.fullPath.FullPathToAssetPath();
            res = AssetDatabase.LoadAssetAtPath<ResourceList>(assetPath);
            return res != null;
        }
        
        static void UpdateResourcesList(Node r)
        {
            var resList = AssetDatabase.LoadAssetAtPath<ResourceList>(r.fullPath.FullPathToAssetPath());
            
            void FindRecursive(Node d)
            {
                var rs = FindResourcesListObject(d, out var rsf);
                if(rs != null && rsf != r)
                {
                    CheckAndAddResource(resList, rsf);
                    return;
                }
                
                foreach(var sub in d.dirs)
                {
                    FindRecursive(sub);
                }
                
                foreach(var f in d.files)
                {
                    if(f == r) continue;
                    CheckAndAddResource(resList, f);
                }
            }
            
            var originList = resList.resources;
            resList.resources = new List<ResourceList.Entry>();
            FindRecursive(r.parent);
            
            if(originList.SameContent(resList.resources)) return;
            EditorUtility.SetDirty(resList);
            AssetDatabase.SaveAssetIfDirty(resList);
        }
        
        static void CheckAndAddResource(ResourceList r, Node f)
        {
            if(f.extension == ".meta") return;
            var assetPath = f.fullPath.FullPathToAssetPath();
            var assetType = AssetDatabase.GetMainAssetTypeAtPath(assetPath);
            if(assetType == null || assetType == typeof(ResourceCollection)) return;
            var mainAsset = AssetDatabase.LoadAssetAtPath(assetPath, assetType);
            if(mainAsset == null) return;
            var allAssets = null as UnityEngine.Object[];
            
            
            if(assetType == typeof(SceneAsset))
            {
                allAssets = new UnityEngine.Object[] { mainAsset };
            }
            else if(assetType == typeof(GameObject))
            {
                allAssets = new UnityEngine.Object[] { mainAsset };
            }
            else if(assetType == typeof(TextAsset))
            {
                allAssets = new UnityEngine.Object[] { mainAsset };
            }
            else
            {
                allAssets = AssetDatabase.LoadAllAssetsAtPath(assetPath)
                .Where(x => x.hideFlags == HideFlags.None || x.hideFlags == HideFlags.NotEditable)
                .ToArray();
            }
            
            
            foreach(var a in allAssets)
            {
                var entry = new ResourceList.Entry();
                var pathToAsset = a.name;
                
                if(mainAsset == a)
                {
                    entry.name = pathToAsset;
                }
                else
                {
                    if(a.name == pathToAsset)
                    {
                        entry.name = ":" + pathToAsset;
                    }
                    else
                    {
                        entry.name = pathToAsset + ":" + a.name;
                    }
                }
                entry.target = a;
                r.resources.Add(entry);
            }
        }
    }
}
