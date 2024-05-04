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
        public string name;
        
        // 边界, 左闭右闭区间.
        public Rect range;
        public int[] adjacentScenes;
        
        // true: 激活(需要加载), false: 不激活(需要卸载)
        [field: Header("runtime"), SerializeField]
        public bool targetState { get; private set; }
        
        [field: SerializeField]
        public SceneLoadingState state { get; private set; } = SceneLoadingState.None;
        
        [field: SerializeField]
        public Scene runtimeScene { get; private set; }
        
        public IEnumerable<SceneEntry> GetAdjacent(SceneEntry[] entries)
        {
            return adjacentScenes.Select(x => entries[x]);
        }
        
        public bool ContainsPoint(Vector2 p)
        {
            return range.ContainsInclusive(p);
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
        
        public SceneEntry[] entries = Array.Empty<SceneEntry>();
        
        [Header("Loading Config")]
        public int checkPerFrame = 1;
        
    }
}
