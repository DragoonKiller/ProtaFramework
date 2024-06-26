using UnityEngine;
using UnityEditor;
using Prota.Unity;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.Rendering;
using System.Text.RegularExpressions;
using System;
using Codice.Client.Commands;

namespace Prota.Editor
{
    [CustomEditor(typeof(ProtaMaterialProvider), false)]
    [CanEditMultipleObjects]
    public class ProtaMaterialProviderInspector : UnityEditor.Editor
    {
        struct EnumEntry
        {
            public readonly string name;
            public readonly int value;
            public EnumEntry(string name, int value)
            {
                this.name = name;
                this.value = value;
            }
        }
        
        class ShaderInfoCache
        {
            public Shader shader;
            public Dictionary<string, int> ids = new();
            public Dictionary<int, int> indexes = new();
            public Dictionary<int, string> names = new();
            public Dictionary<int, ShaderPropertyType> types = new();
            public HashMapList<int, string> attributes = new();
            public Dictionary<int, bool> isHDR = new();
            public Dictionary<int, Vector2> ranges = new();
            public HashMapList<int, EnumEntry> enums = new();
            public Dictionary<int, int> stIds = new();
            public Dictionary<int, int> sts = new();
            public ShaderInfoCache(Shader shader)
            {
                this.shader = shader;
                var n = shader.GetPropertyCount();
                for(int i = 0; i < n; i++)
                {
                    var name = shader.GetPropertyName(i);
                    var id = Shader.PropertyToID(name);
                    var type = shader.GetPropertyType(i);
                    
                    ids[name] = id;
                    indexes[id] = i;
                    names[id] = name;
                    types[id] = type;
                    
                    isHDR[id] = 0 != (shader.GetPropertyFlags(i) | ShaderPropertyFlags.HDR);
                    
                    var attrs = shader.GetPropertyAttributes(i);
                    attributes.GetOrCreate(id).AddRange(attrs);
                    
                    foreach(var attr in attrs)
                    {
                        if(GetRangeFromAttribute(attr, out var min, out var max)) ranges[id] = new Vector2(min, max);
                        if(GetEnumFromAttribute(attr, out var enumEntries)) enums.GetOrCreate(id).AddRange(enumEntries);
                    }
                }
                
                // 贴图要加一个 _ST 变量.
                foreach(var id in names.Keys.ToArray())
                {
                    if(types[id] == ShaderPropertyType.Texture)
                    {
                        var stId = Shader.PropertyToID($"{names[id]}_ST");
                        ids[$"{names[id]}_ST"] = stId;
                        indexes[stId] = indexes[id];
                        names[stId] = $"{names[id]}_ST";
                        types[stId] = ShaderPropertyType.Vector;
                        stIds[id] = stId;
                        sts.Add(stId, id);
                    }
                }
            }
        }
        
        ShaderInfoCache info;
        
        public override void OnInspectorGUI()
        {
            Undo.RecordObject(target, "ProtaMaterialProviderInspector");
            
            var obj = serializedObject;
            
            var pUseInstantiatedMaterial = obj.FindProperty("useInstantiatedMaterial");
            EditorGUILayout.PropertyField(pUseInstantiatedMaterial);
            
            var pMaterial = obj.FindProperty("referenceMaterial");
            EditorGUILayout.PropertyField(pMaterial);
            
            var pInsMat = obj.FindProperty("instanceMaterial");
            using(new EditorGUI.DisabledScope(true)) EditorGUILayout.PropertyField(pInsMat);
            
            SyncMaterialProviding(pUseInstantiatedMaterial.boolValue, pInsMat);
            
            var refMat = (Material)pMaterial.objectReferenceValue;
            if(refMat == null)
            {
                EditorGUILayout.HelpBox("Reference material is null", MessageType.Info);
                return;
            }
            
            if(info == null || info.shader != refMat.shader)
            {
                info = new ShaderInfoCache(refMat.shader);
            }
            
            {
                EditorGUILayout.LabelField("Targets", EditorStyles.largeLabel);
                using var _ = new EditorGUI.IndentLevelScope();
                EditorGUILayout.PropertyField(obj.FindProperty("targets"));
                if(GUILayout.Button("Select from self")) SelectFromSelf();
                if(GUILayout.Button("Select from children")) SelectFromChildren();
                if(GUILayout.Button("Select from parent")) SelectFromParent();
                if(GUILayout.Button("Select from all")) SelectFromAll();
                if(GUILayout.Button("Clear targets")) SelectClear();
            }
            
            {
                EditorGUILayout.LabelField("Material Properties", EditorStyles.largeLabel);
                using var _ = new EditorGUI.IndentLevelScope();
                DrawMaterialProperties(obj);
            }
            
            obj.ApplyModifiedProperties();
        }
        
        void SelectFromSelf()
        {
            var provider = target as ProtaMaterialProvider;
            var res = provider.gameObject.GetComponents<Renderer>();
            if(!provider.targets.IsNullOrEmpty()) res = res.Concat(provider.targets).Distinct().ToArray();
            provider.targets = res;
            serializedObject.Update();
        }
        
        void SelectFromChildren()
        {
            var provider = target as ProtaMaterialProvider;
            var res = provider.gameObject.GetComponentsInChildren<Renderer>();
            if(!provider.targets.IsNullOrEmpty()) res = res.Concat(provider.targets).Distinct().ToArray();
            provider.targets = res;
            serializedObject.Update();
        }
        
        void SelectFromParent()
        {
            var provider = target as ProtaMaterialProvider;
            var res = provider.gameObject.GetComponentsInParent<Renderer>();
            if(!provider.targets.IsNullOrEmpty()) res = res.Concat(provider.targets).Distinct().ToArray();
            provider.targets = res;
            serializedObject.Update();
        }
        
        void SelectFromAll()
        {
            var provider = target as ProtaMaterialProvider;
            var res = FindObjectsOfType<Renderer>();
            if(!provider.targets.IsNullOrEmpty()) res = res.Concat(provider.targets).Distinct().ToArray();
            provider.targets = res;
            serializedObject.Update();
        }
        
        void SelectClear()
        {
            var provider = target as ProtaMaterialProvider;
            provider.targets = Array.Empty<Renderer>();
            serializedObject.Update();
        }
        
        void DrawMaterialProperties(SerializedObject obj)
        {
            var vecList = obj.FindProperty("vectorEntries");
            var floatList = obj.FindProperty("floatEntries");
            var intList = obj.FindProperty("intEntries");
            var texList = obj.FindProperty("textureEntries");
            // var matList = obj.FindProperty("matrixEntries");     // 填不了?
            
            // 确保所有材质中存在的属性都被记录.
            foreach(var name in info.names.Values)
            {
                var id = info.ids[name];
                var type = info.types[id];
                switch(type)
                {
                    case ShaderPropertyType.Int: EnsurePropertyExists(intList, id); break;
                    case ShaderPropertyType.Float: EnsurePropertyExists(floatList, id); break;
                    case ShaderPropertyType.Range: EnsurePropertyExists(floatList, id); break;
                    case ShaderPropertyType.Color: EnsurePropertyExists(vecList, id); break;
                    case ShaderPropertyType.Vector: EnsurePropertyExists(vecList, id); break;
                    case ShaderPropertyType.Texture:
                        EnsurePropertyExists(texList, id);
                        EnsurePropertyExists(vecList, info.stIds[id], false);
                    break;
                    default:
                    {
                        EditorGUILayout.HelpBox($"Unsupported shader property type: {type}", MessageType.Warning);
                        break;
                    }
                }
            }
            
            // 删除不存在于材质中, 但是之前被记录的属性.
            vecList.RemoveAllAsList(x => !info.names.ContainsKey(IdFromProp(x)));
            floatList.RemoveAllAsList(x => !info.names.ContainsKey(IdFromProp(x)));
            intList.RemoveAllAsList(x => !info.names.ContainsKey(IdFromProp(x)));
            texList.RemoveAllAsList(x => !info.names.ContainsKey(IdFromProp(x)));
            
            // 重新设置激活数组.
            var vecValid = obj.FindProperty("vectorValid");
            var floatValid = obj.FindProperty("floatValid");
            var intValid = obj.FindProperty("intValid");
            var texValid = obj.FindProperty("textureValid");
            // var matValid = obj.FindProperty("matrixValid");
            
            MatchValidArray(vecList, vecValid);
            MatchValidArray(floatList, floatValid);
            MatchValidArray(intList, intValid);
            MatchValidArray(texList, texValid);
            
            for(int i = 0; i < vecList.arraySize; i++)
            {
                var p = vecList.GetArrayElementAtIndex(i);
                var v = vecValid.GetArrayElementAtIndex(i);
                if(!IsSTProperty(p)) DrawEntry(p, v);
            }
            
            for(int i = 0; i < floatList.arraySize; i++)
            {
                var p = floatList.GetArrayElementAtIndex(i);
                var v = floatValid.GetArrayElementAtIndex(i);
                DrawEntry(p, v);
            }
            
            for(int i = 0; i < intList.arraySize; i++)
            {
                var p = intList.GetArrayElementAtIndex(i);
                var v = intValid.GetArrayElementAtIndex(i);
                DrawEntry(p, v);
            }
            
            for(int i = 0; i < texList.arraySize; i++)
            {
                var p = texList.GetArrayElementAtIndex(i);
                var v = texValid.GetArrayElementAtIndex(i);
                DrawEntry(p, v);
                var stid = info.stIds[IdFromProp(p)];
                foreach(var q in vecList.EnumerateAsList())
                    if(IsSTProperty(q) && IdFromProp(q) == stid)
                        DrawEntry(q);
            }
            
            if(GUILayout.Button("Reset"))
            {
                info = null;
                vecList.ClearArray();
                floatList.ClearArray();
                intList.ClearArray();
                texList.ClearArray();
                // matList.ClearArray();
                vecValid.ClearArray();
                floatValid.ClearArray();
                intValid.ClearArray();
                texValid.ClearArray();
                // matValid.ClearArray();
                return;
            }
        }
        
        void MatchValidArray(SerializedProperty arr, SerializedProperty valid)
        {
            var originalSize = valid.arraySize;
            valid.arraySize = arr.arraySize;
            for(int i = originalSize; i < arr.arraySize; i++)
            {
                valid.GetArrayElementAtIndex(i).boolValue = true;
            }
        }
        
        void EnsurePropertyExists(SerializedProperty list, int id, bool init = true)
        {
            var prop = list.FindAsList(x => IdFromProp(x) == id);
            if(prop == null)
            {
                prop = list.AddAsList();
                prop.FindPropertyRelative("id").intValue = id;
                if(init) AssignDefaultValue(prop, id);
            }
        }
        
        void AssignDefaultValue(SerializedProperty p, int id)
        {
            var type = info.types[id];
            var index = info.indexes[id];
            var valueProp = p.FindPropertyRelative("value");
            switch(type)
            {
                case ShaderPropertyType.Int: valueProp.intValue = info.shader.GetPropertyDefaultIntValue(index); break;
                case ShaderPropertyType.Float: valueProp.floatValue = info.shader.GetPropertyDefaultFloatValue(index); break;
                case ShaderPropertyType.Range: valueProp.floatValue = info.shader.GetPropertyDefaultFloatValue(index); break;
                case ShaderPropertyType.Color: valueProp.vector4Value = info.shader.GetPropertyDefaultVectorValue(index); break;
                case ShaderPropertyType.Vector:
                    if(info.stIds.Values.Contains(id))
                    {
                        valueProp.vector4Value = new Vector4(1, 1, 0, 0);
                    }
                    else
                    {
                        valueProp.vector4Value = info.shader.GetPropertyDefaultVectorValue(index);
                    }
                break;
                case ShaderPropertyType.Texture: valueProp.objectReferenceValue = Resources.Load<Texture>(info.shader.GetPropertyTextureDefaultName(index)); break;
                default: break; // no default value.
            }
        }
        
        void DrawEntry(SerializedProperty p, SerializedProperty ac = null)
        {
            var id = IdFromProp(p);
            var name = info.names[id];
            var value = p.FindPropertyRelative("value");
            var type = info.types[id];
            
            using var _ = new EditorGUILayout.HorizontalScope();
            
            if(ac != null) ac.boolValue = EditorGUILayout.Toggle(GUIContent.none, ac.boolValue, GUILayout.Width(20));
            else EditorGUILayout.LabelField("", GUILayout.Width(20));
            
            switch (type)
            {
                case ShaderPropertyType.Int:
                {
                    var enumed = info.enums.TryGetValue(id, out var enumEntries);
                    var ranged = info.ranges.TryGetValue(id, out var range);
                    if(enumed)
                    {
                        var enumIndex = enumEntries.FindIndex(x => x.value == value.intValue);
                        var newIndex = EditorGUILayout.Popup(name, enumIndex, enumEntries.Select(x => x.name).ToArray());
                        if(newIndex != enumIndex) value.intValue = enumEntries[newIndex].value;
                    }
                    else if(ranged)
                    {
                        value.intValue = EditorGUILayout.IntSlider(name, value.intValue, (int)range.x, (int)range.y);
                    }
                    else
                    {
                        value.intValue = EditorGUILayout.IntField(name, value.intValue);
                    }
                    break;
                }
                
                case ShaderPropertyType.Color:
                {
                    var isHDR = info.isHDR[id];
                    value.vector4Value = EditorGUILayout.ColorField(new GUIContent(name), value.vector4Value, true, true, isHDR);
                    break;
                }
                
                case ShaderPropertyType.Vector:
                {
                    value.vector4Value = EditorGUILayout.Vector4Field(name, value.vector4Value);
                    break;
                }
                
                case ShaderPropertyType.Float:
                case ShaderPropertyType.Range:
                {
                    var enumed = info.enums.TryGetValue(id, out var enumEntries);
                    var ranged = info.ranges.TryGetValue(id, out var range);
                    if(enumed)
                    {
                        var enumIndex = enumEntries.FindIndex(x => x.value == value.floatValue);
                        var newIndex = EditorGUILayout.Popup(name, enumIndex, enumEntries.Select(x => x.name).ToArray());
                        if(newIndex != enumIndex) value.floatValue = enumEntries[newIndex].value;
                    }
                    else if(ranged)
                    {
                        value.floatValue = EditorGUILayout.Slider(name, value.floatValue, range.x, range.y);
                    }
                    else
                    {
                        value.floatValue = EditorGUILayout.FloatField(name, value.floatValue);
                    }
                    break;
                }
                
                case ShaderPropertyType.Texture:
                {
                    var tex = (Texture)value.objectReferenceValue;
                    tex = (Texture)EditorGUILayout.ObjectField(name, tex, typeof(Texture), false, th);
                    value.objectReferenceValue = tex;
                    break;
                }
                
                default:
                {
                    EditorGUILayout.HelpBox($"Unsupported shader property type: [{type}]", MessageType.Warning);
                    break;
                }
            }
        }
        
        void SyncMaterialProviding(bool useInstantiatedMaterial, SerializedProperty p)
        {
            var provider = target as ProtaMaterialProvider;
            if(useInstantiatedMaterial && p.objectReferenceValue == null) provider.OnEnable();
            if(!useInstantiatedMaterial && p.objectReferenceValue != null) provider.OnDisable();
        }
        
        
        static GUILayoutOption th = GUILayout.Height(EditorGUIUtility.singleLineHeight);
        
        // ====================================================================================================
        // ====================================================================================================
        
        bool IsSTProperty(SerializedProperty p) => info.sts.ContainsKey(IdFromProp(p));
        
        int IdFromProp(SerializedProperty p) => p.FindPropertyRelative("id").intValue;
        
        
        static Regex rangeRegex = new Regex(@"Range\((.*?),(.*?)\)", RegexOptions.Compiled);
        
        public static bool GetRangeFromAttribute(string attr, out float min, out float max)
        {
            var match = rangeRegex.Match(attr);
            if(match.Success)
            {
                min = float.Parse(match.Groups[1].Value);
                max = float.Parse(match.Groups[2].Value);
                return true;
            }
            
            min = max = 0;
            return false;
        }
        
        
        static Regex namedRegex = new Regex(@"Enum\((.+?)(?:,[\s]*(.+?))+\)", RegexOptions.Compiled);
        static Regex enumRegex = new Regex(@"Enum\((.*?)\)", RegexOptions.Compiled);
        static bool GetEnumFromAttribute(string attr, out EnumEntry[] enumEntries)
        {
            bool MatchEnumTypeName(string attr, ref EnumEntry[] enumEntries)
            {
                var match = enumRegex.Match(attr);
                if(!match.Success) return false;
                var enumName = match.Groups[1].Value;
                var type = AppDomain.CurrentDomain.GetAssemblies().Select(x => x.GetType(enumName)).FirstOrDefault(x => x != null);
                if(type == null || !type.IsEnum)
                {
                    Debug.LogError($"Enum type not found: {enumName}");
                    return false;
                }
                enumEntries = Enum.GetValues(type)
                    .Cast<Enum>()
                    .Select(x => new EnumEntry(Enum.GetName(type, x), Convert.ToInt32(x)))
                    .ToArray();
                return true;
            }
            
            bool MatchNamed(string attr, ref EnumEntry[] enumEntries)
            {
                var match = namedRegex.Match(attr);
                if(!match.Success) return false;
                enumEntries = match.Groups[1].Captures
                    .Concat(match.Groups[2].Captures)
                    .Select((x, i) => (x, i))
                    .GroupBy(x => x.i / 2)
                    .Select(x => new EnumEntry(x.First().x.Value, int.Parse(x.Last().x.Value)))
                    .ToArray();
                return true;
            }
            
            enumEntries = null;
            if(MatchNamed(attr, ref enumEntries)) return true;
            if(MatchEnumTypeName(attr, ref enumEntries)) return true;
            return false;
        }
    }
}
