


using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;



// ProtaFramework 专用的资源列表, 搭配 ResourcesList 一起使用.
// ProtaRes 是一个放置在 ProtaFramework 包里的 ScriptableObject, 会记录项目里所有的 ResourcesList.
// ResourceList 记录它所在文件夹和子文件夹里的所有资源, 比如 Sprite, TextAsset, Prefab等.
// ResourceList 也会记录子文件夹里的其它 ResourceList, 这样可以对 ResourceList 分级.
// 可以在一个目录中点击右键, 选择 Create/Prota Framework/ResourcesList 来创建 ResourceList 对象,
//     资源会被自动记录在该 ResourceList 对象里, 而该 ResourceList 对象也会被 ProtaRes 自动记录.
// 可以通过 ProtaRes.Load<T>(listName, resName) 和 ProtaRes.TryLoad<T>(listName, resName, out res) 来加载资源.
// 资源文件必须放置在 Resource 文件夹内. 未来会考虑扩展至 StreamingAssets 和自定义打包结构(assetBundle 和 其他文件).
// 其中 resName 是资源自己的名称, 可以在 ResourceList 对象中查看记录的资源的名称.
// 如果 Sprite 和所属的 Texture 文件不重名则以冒号开头, 否则以 Texture 文件名+冒号开头.
// 其他资源名称即为文件名.



namespace Prota.Unity
{
    [CreateAssetMenu(fileName = "ResourcesList", menuName = "Prota Framework/ResourcesCollection", order = 1)]
    public class ProtaRes : ScriptableObject
    {
        [Serializable]
        public class _ListSet : SerializableDictionary<string, UnityEngine.Object> { }
        [SerializeField] public _ListSet lists = new _ListSet();
        
        public ResourceList this[string name]
        {
            get
            {
                // lists.Select(x => $"{x.Key} :: {x.Value}").ToStringJoined().LogError();
                // Debug.LogError(name);
                return lists[name].AssertNotNull() as ResourceList;
            }
        }
        
        static ProtaRes _instance;
        public static ProtaRes instance
        {
            get
            {
                if(_instance == null) _instance = Resources.Load<ProtaRes>("ProtaRes");
                _instance.AssertNotNull();
                return _instance;
            }
        }
        
        public static T Load<T>(string list, string name) where T : UnityEngine.Object
        {
            return instance[list].Get<T>(name);
        }
        
        public static bool TryLoad<T>(string list, string name, out T res) where T : UnityEngine.Object
        {
            return instance[list].TryGet<T>(name, out res);
        }
    }
    
}
