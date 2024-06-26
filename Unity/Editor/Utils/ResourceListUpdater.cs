using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime;
using System.Text.RegularExpressions;
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
        public static void RefreshAllResourceList()
        {
            listsNeedToUpdate.AddRange(Resources.LoadAll<ResourceList>("/"));
            UpdateAllNeedsToUpdate();
            UpdateProtaResource();
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
                try
                {
                    UpdateAllNeedsToUpdate();
                    UpdateProtaResource();
                }
                catch(Exception e)
                {
                    Debug.LogException(e);
                }
            };
        }
        
        static void UpdateAllNeedsToUpdate()
        {
            foreach(var resList in listsNeedToUpdate) UpdateResourceList(resList);
            listsNeedToUpdate.Clear();
        }
        
        public static void UpdateResourceList(ResourceList resList)
        {    
            var node = AssetTree.instance.root.FindFullPath(AssetDatabase.GetAssetPath(resList));
            UpdateResourcesList(node);
        }
        
        static void UpdateProtaResource()
        {
            var r = Resources.Load<ProtaRes>("ProtaRes");
            if(r == null) throw new Exception("a ProtaRes must put into [Resources] folder and named [ProtaRes].");
            var lists = Resources.LoadAll<ResourceList>("/");
            
            // Debug.Log($"ProtaRes update search: {r.name}");
            // Debug.LogError($"ProtaRes update: {lists.Select(x => x.name).ToStringJoined()}");
            // Debug.LogError($"ProtaRes update: {r.lists.Select(x => x.Value.name).ToStringJoined()}");
            
            if(r.lists.Select(x => x.Value).SameContent(lists)) return;
            
            r.lists.Clear();
            r.lists.AddRange(lists, x => x.name.ToLower(), x => x);
            
            EditorUtility.SetDirty(r);
            AssetDatabase.SaveAssetIfDirty(r);
            Debug.Log($"ProtaRes update: {r.name}");
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
            
            var originList = new ResourceList._Entry();
            foreach(var i in resList.resources)
            {
                originList.Add(i.Key, i.Value);
            }
            resList.resources.Clear();
            FindRecursive(r.parent);
            
            if(originList.SameContent(resList.resources)) return;
            
            EditorUtility.SetDirty(resList);
            AssetDatabase.SaveAssetIfDirty(resList);
            compiledRegex.Clear();
            Debug.Log($"Asset update: {resList.name}");
        }
        
        static readonly Dictionary<string, Regex> compiledRegex = new();
        
        static void CheckAndAddResource(ResourceList r, Node f)
        {
            if(f.extension == ".meta") return;
            
            if(!compiledRegex.TryGetValue(r.regexMatcher, out var regex))
            {
                regex = new Regex(r.regexMatcher, RegexOptions.IgnoreCase);
                compiledRegex[r.regexMatcher] = regex;
            }
            
            if(!regex.IsMatch(f.name))
            {
                return;
            }
            
            var assetPath = f.fullPath.FullPathToAssetPath();
            var assetType = AssetDatabase.GetMainAssetTypeAtPath(assetPath);
            if(assetType == null || assetType == typeof(ProtaRes)) return;
            var mainAsset = AssetDatabase.LoadAssetAtPath(assetPath, assetType);
            if(mainAsset == null) return;
            var allAssets = null as UnityEngine.Object[];
            
            if(r.ignoreSubAsset)
            {
                allAssets = new UnityEngine.Object[] { mainAsset };
            }
            else
            {
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
            }
            
            foreach(var a in allAssets)
            {
                var recordName = a.name;
                var name = null as string;
                
                if(mainAsset == a)
                {
                    name = recordName;
                }
                else
                {
                    name = r.subAssetNamingFormat.Replace("$main", f.nameNoExt).Replace("$sub", recordName);
                }
                
                var lowerName = name.ToLower();
                if(!r.ignoreDuplicateAsset && r.resources.ContainsKey(lowerName)) Debug.LogError($"Duplicate resource name[{name}]");
                r.resources[lowerName] = a;
            }
        }
    }
}
