using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prota.UI
{
    // 基础 UI 组件. 
    public class UIBinding : MonoBehaviour, ISerializationCallbackReceiver
    {
        Dictionary<string, UIContent> contents = new Dictionary<string, UIContent>();
        
        public IReadOnlyDictionary<string, UIContent> rawContent => contents;
        
        public UIContent this[string name]
        {
            get
            {
                if(!contents.TryGetValue(name, out var res)) return null;
                return res;
            }
            
            set
            {
                // UnityEngine.Debug.Log($"set { name } { value }");
                contents[name] = value;
            }
        }
        
        public T Get<T>(string name) where T: Component => this[name].GetComponent<T>();
        
        public UIBinding ClearBindings()
        {
            contents.Clear();
            elementNames.Clear();
            elements.Clear();
            return this;
        }
        
        // ========================================================================================
        // Serialization
        // ========================================================================================
        
        [SerializeField] List<UIContent> elements = new List<UIContent>();
        
        [SerializeField] List<string> elementNames = new List<string>();
           
        public void OnBeforeSerialize()
        {
            contents.SerializeToList(elementNames, elements);
        }

        public void OnAfterDeserialize()
        {
            contents.DeserializeFromList(elementNames, elements);
        }
    }
}
