using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Loading;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Prota;
using System.Linq;

namespace Prota.Unity
{
    [Serializable]
    public enum SceneLoadingState
    {
        None = 0,
        Loading,
        Loaded,
        Unloading,
    }
    
    [Serializable]
    public class SceneEntry
    {
        [SerializeField] public OverworldSceneInfo info;
        
        public string name;
        
        // 边界, 左闭右闭区间.
        public Rect range;
        public int[] adjacentScenes;
        
        // true: 激活(需要加载), false: 不激活(需要卸载)
        [field: Header("runtime"), NonSerialized]
        public bool targetState { get; private set; }
        
        [field: NonSerialized]
        public SceneLoadingState state { get; private set; } = SceneLoadingState.None;
        
        [field: NonSerialized]
        public Scene runtimeScene { get; private set; }
        
        public string assetPath => $"Assets/Resources/{info.scenePath}/{name}.unity";
        
        public string resourcePath => $"{info.scenePath}/{name}";
        
        public string assetFileName => $"{name}.unity";
        
        
        public SceneEntry(string name, OverworldSceneInfo overworld)
        {
            this.name = name;
            this.info = overworld;
        }
        
        public IEnumerable<SceneEntry> GetAdjacent(SceneEntry[] entries)
        {
            return adjacentScenes.Select(x => entries[x]);
        }
        
        public bool ContainsPoint(Vector2 p)
        {
            return range.ContainsInclusive(p);
        }
        
        public bool ContainsAny(IEnumerable<Transform> transforms)
        {
            return transforms.Any(x => ContainsPoint(x.position));
        }
        
        public bool ContainsAny(IEnumerable<Vector2> points)
        {
            return points.Any(x => ContainsPoint(x));
        }
        
        
        public void SetTargetState(bool targetState)
        {
            this.targetState = targetState;
            if(targetState == true)
            {
                if(state == SceneLoadingState.None) Load();
            }
            else
            {
                if(state == SceneLoadingState.Loaded) Unload();
            }
        }
        
        public void Load()
        {
            if(state != SceneLoadingState.None) return;
            state = SceneLoadingState.Loading;
            var asop = SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);
            asop.completed += op =>
            {
                state = SceneLoadingState.Loaded;
                runtimeScene = SceneManager.GetSceneByName(name);
                asop = null;
                
                // 加载完了发现需要卸载.
                if(targetState == false) Unload();
            };
        }
        
        public void Unload()
        {
            if(state != SceneLoadingState.Loaded) return;
            state = SceneLoadingState.Unloading;
            var asop = SceneManager.UnloadSceneAsync(runtimeScene);
            asop.completed += op =>
            {
                state = SceneLoadingState.None;
                runtimeScene = default;
                asop = null;
                
                // 卸载完了发现需要加载.
                if(targetState == true) Load();
            };
        }
    }
        
    // 存储所有的 scene 信息; 包括每个 scene 的包围盒, 以及 scene 资源名称.
    // 注意不能存 SceneAsset, 这是编辑器的内容.
    // 也不能存 Scene, 因为这是场景加载过后的对象.
    [CreateAssetMenu(menuName = "Prota Framework/Overworld Scenes Info", fileName = "OverworldScenesInfo")]
    public class OverworldSceneInfo : ScriptableObject
    {
        // Resources 相对路径.
        public string scenePath;
        
        public string scenePathRelativeToAssets => $"Resources/{scenePath}";
        public string scenePathRelativeToRoot => $"Assets/Resources/{scenePath}";
        
        public SceneEntry[] entries = Array.Empty<SceneEntry>();
        
        [Header("Loading Config")]
        public int checkPerFrame = 1;
        public int adjacentStep = 1;
        
        
        public string GetAssetPathOfName(string name)
        {
            return $"Assets/Resources/{scenePath}/{name}.unity";
        }
    }
}
