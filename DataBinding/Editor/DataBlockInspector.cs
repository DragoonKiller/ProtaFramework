using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;
using System;
using System.Linq;
using Prota.Unity;

namespace Prota.DataBinding
{
    [CustomEditor(typeof(DataBlock))]
    public class DataBlockInspector : UnityEditor.Editor
    {
        static List<Type> bindingTypeCache = new List<Type>();
        
        static DataBlockInspector()
        {
            foreach(var t in typeof(DataBinding).GetNestedTypes())
            {
                if(typeof(DataBinding).IsAssignableFrom(t))
                {
                    bindingTypeCache.Add(t);
                }
            }
            
            bindingTypeCache.Sort((a, b) => a.Name.CompareTo(b.Name));
        }
        
        DataBlock dataBlock => serializedObject.targetObject as DataBlock;
        
        SerializedProperty data => serializedObject.FindProperty("data");
        
        IEnumerable<SerializedProperty> GetRecords()
        {
            for(int i = 0; i < data.arraySize; i++)
            {
                yield return data.GetArrayElementAtIndex(i);
            }
        }
        
        
        bool useOriginal;
        public override void OnInspectorGUI()
        {
            useOriginal = EditorGUILayout.ToggleLeft("使用原始视图", useOriginal);
            if(useOriginal) OriginalGUI();
            else CustomGUI();
        }
        
        void OriginalGUI() => base.OnInspectorGUI();
        
        void CustomGUI()
        {
            Undo.RecordObject(dataBlock, "DataBlockInspector");
            var changed = false;
            changed |= BlockName();
            changed |= Data();
            changed |= AddDataBlock();
            if(changed) dataBlock.WriteData();
            var source = PrefabUtility.GetCorrespondingObjectFromSource(dataBlock);
            var path = source == null ? null : AssetDatabase.GetAssetPath(source);
            serializedObject.ApplyModifiedProperties();
            serializedObject.UpdateIfRequiredOrScript();
            if(source != null && GUILayout.Button("保存数据"))
            {
                PrefabUtility.ApplyObjectOverride(dataBlock, path, InteractionMode.AutomatedAction);
            }
        }
        
        bool BlockName()
        {
            var originalName = dataBlock.blockName;
            dataBlock.blockName = EditorGUILayout.TextField("数据块名称:", dataBlock.blockName);
            EditorGUILayout.Space(5);
            return originalName != dataBlock.blockName;
        }
        
        
        
        bool Data()
        {
            var changed = false;
            
            // 名称排序.
            var dataList = GetRecords().ToList();
            dataList.Sort((p, q) => p.FindPropertyRelative("name").stringValue.CompareTo(q.FindPropertyRelative("name").stringValue));
            
            int toBeRemove = -1;
            for(int i = 0; i < dataList.Count; i++)
            {
                if(i == 0)
                {
                    EditorGUILayout.Space(5);
                    this.SeperateLine(2, Color.black);
                }
                changed |= TypedBindingEditor(dataList[i], out var removal);
                if(removal) toBeRemove = i;
            }
            
            if(toBeRemove >= 0)
            {
                var removeName = dataList[toBeRemove].FindPropertyRelative("name").stringValue;
                data.DeleteArrayElementAtIndex(toBeRemove);
                changed = true;
            }
            
            return changed;
        }
        
        
        string newBindingName;
        string newBindingFilter;
        bool useAddDataBlock;
        Vector2 offsetAddDataBlock;
        bool AddDataBlock()
        {
            useAddDataBlock = EditorGUILayout.BeginFoldoutHeaderGroup(useAddDataBlock, "添加数据块");
            if(!useAddDataBlock)
            {
                EditorGUILayout.EndFoldoutHeaderGroup();
                return false;
            }
            newBindingName = EditorGUILayout.TextField("名称:", newBindingName);
            newBindingFilter = EditorGUILayout.TextField("搜索:", newBindingFilter);
            offsetAddDataBlock = EditorGUILayout.BeginScrollView(offsetAddDataBlock, GUILayout.Height(100));
            DataBinding newObj = null;
            foreach(var t in bindingTypeCache)
            {
                if(!string.IsNullOrEmpty(newBindingFilter) && !t.Name.ToLower().Contains(newBindingFilter.ToLower())) continue;
                if(GUILayout.Button(t.Name))
                {
                    newObj = Activator.CreateInstance(t) as DataBinding;
                    Debug.Assert(newObj != null);
                }
            }
            EditorGUILayout.EndScrollView();
            if(newObj != null)
            {
                if(string.IsNullOrEmpty(newBindingName)) Debug.LogError("创建 DataBinding 前先填写名字");
                else if(dataBlock[newBindingName] != null) Debug.LogError("不能添加重名条目!!!!");
                else
                {
                    dataBlock.Add(newBindingName, newObj);
                }
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            
            return newObj != null;
        }
        
        
        static GUIStyle _nameStyle;
        static GUIStyle nameStyle
        {
            get
            {
                if(_nameStyle == null)
                {
                    _nameStyle = new GUIStyle(EditorStyles.largeLabel);
                    _nameStyle.fontStyle = FontStyle.Bold;
                }
                return _nameStyle;
            }
        }
        
        bool TypedBindingEditor(SerializedProperty t, out bool removal)
        {
            EditorGUILayout.Space(2);
            
            removal = false;
            var w = GUILayout.Width(200);
            var lname = "";
            var pname = t.FindPropertyRelative("name").stringValue;
            var changed = false;
            var type = t.FindPropertyRelative("type").stringValue;
            var ss = new SerializedData();
            var dataProp = t.FindPropertyRelative("data").FindPropertyRelative("data");
            var cg = null as DataBinding;
            for(int i = 0; i < dataProp.arraySize; i++)
            {
                ss.Push(dataProp.GetArrayElementAtIndex(i).intValue); // 都是 int.
            }
            ss.Reset();
            
            EditorGUILayout.BeginHorizontal();
            this.SetColor(new Color(0.6f, 0.8f, 1f, 1));
            EditorGUILayout.LabelField(pname, nameStyle, GUILayout.MinWidth(100));
            this.ResetColor();
            if(GUILayout.Button("x", GUILayout.Width(20))) removal = true;
            EditorGUILayout.EndHorizontal();
            
            if(type == "Int")
            {
                EditorGUILayout.BeginHorizontal();
                var g = new DataBinding.Int();
                g.Deserialize(ss);
                var originalValue = g.value;
                g.value = EditorGUILayout.IntField(lname, g.value);
                if(g.value != originalValue) cg = g;
                EditorGUILayout.EndHorizontal();
            }
            else if(type == "Float")
            {
                EditorGUILayout.BeginHorizontal();
                var g = new DataBinding.Float();
                g.Deserialize(ss);
                var originalValue = g.value;
                g.value = EditorGUILayout.FloatField(lname, g.value);
                if(g.value != originalValue) cg = g;
                EditorGUILayout.EndHorizontal();
            }
            else if(type == "Color")
            {
                EditorGUILayout.BeginHorizontal();
                var g = new DataBinding.Color();
                g.Deserialize(ss);
                var originalValue = g.value;
                g.value = EditorGUILayout.ColorField(lname, g.value);
                if(g.value != originalValue) cg = g;
                EditorGUILayout.EndHorizontal();
            }
            else if(type == "Vector2")
            {
                EditorGUILayout.BeginHorizontal();
                var g = new DataBinding.Vector2();
                g.Deserialize(ss);
                var originalValue = g.value;
                g.value = EditorGUILayout.Vector2Field(lname, g.value);
                if(g.value != originalValue) cg = g;
                EditorGUILayout.EndHorizontal();
            }
            else if(type == "Vector3")
            {
                EditorGUILayout.BeginHorizontal();
                var g = new DataBinding.Vector3();
                g.Deserialize(ss);
                var originalValue = g.value;
                g.value = EditorGUILayout.Vector3Field(lname, g.value);
                if(g.value != originalValue) cg = g;
                EditorGUILayout.EndHorizontal();
            }
            else if(type == "Vector4")
            {
                EditorGUILayout.BeginHorizontal();
                var g = new DataBinding.Vector4();
                g.Deserialize(ss);
                var originalValue = g.value;
                g.value = EditorGUILayout.Vector3Field(lname, g.value);
                if(g.value != originalValue) cg = g;
                EditorGUILayout.EndHorizontal();
            }
            else if(type == "String")
            {
                EditorGUILayout.BeginHorizontal();
                var g = new DataBinding.String();
                g.Deserialize(ss);
                var originalValue = g.value;
                g.value = EditorGUILayout.TextArea(g.value);
                if(g.value != originalValue) cg = g;
                EditorGUILayout.EndHorizontal();
            }
            else if(type == "Quaternion")
            {
                EditorGUILayout.BeginHorizontal();
                var g = new DataBinding.Quaternion();
                g.Deserialize(ss);
                var originalValue = g.value;
                g.value = EditorGUILayout.Vector4Field(lname, g.value.ToVec4()).ToQuaternion();
                if(g.value != originalValue) cg = g;
                EditorGUILayout.EndHorizontal();
            }
            else if(type == "EulerAngle")
            {
                EditorGUILayout.BeginHorizontal();
                var g = new DataBinding.EulerAngle();
                g.Deserialize(ss);
                var originalValue = g.value;
                g.value = EditorGUILayout.Vector3Field(lname, g.value);
                if(g.value != originalValue) cg = g;
                EditorGUILayout.EndHorizontal();
            }
            else if(type == "Rigidbody2D")
            {
                var g = new DataBinding.Rigidbody2D();
                g.Bind(pname, dataBlock);
                EditorGUILayout.ObjectField("target", g.target, typeof(UnityEngine.Rigidbody2D), true);
            }
            else if(type == "Transform")
            {
                var g = new DataBinding.Transform();
                g.Bind(pname, dataBlock);
                EditorGUILayout.ObjectField("target", g.transform, typeof(UnityEngine.Transform), true);
            }
            else if(type == "Sprite")
            {
                var g = new DataBinding.Sprite();
                g.Bind(pname, dataBlock);
                EditorGUILayout.ObjectField("target", g.target, typeof(UnityEngine.SpriteRenderer), true);
                EditorGUILayout.ObjectField("sprite", g.sprite, typeof(UnityEngine.Sprite), true);
            }
            else
            {
                // 反射枚举所有元素.
                var g = Activator.CreateInstance(DataBinding.types[type]) as DataBinding;
                g.Deserialize(ss);
                foreach(var f in g.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                {
                    var result = this.AnyField(f.Name, g, f);
                    if(!result) EditorGUILayout.LabelField(f.Name);
                }
            }
            
            EditorGUILayout.Space(3);
            this.SeperateLine(1, new Color(.1f, .1f, .1f, 1));
            
            if(cg != null)
            {
                changed = true;
                ss.Clear();
                cg.Serialize(ss);
                dataProp.arraySize = ss.data.Count;
                for(int i = 0; i < ss.data.Count; i++)
                {
                    dataProp.GetArrayElementAtIndex(i).intValue = ss.data[i];
                }
                dataBlock.MakeDirty();
            }
            
            return changed;
        }
        
        
        
    }
    
}
