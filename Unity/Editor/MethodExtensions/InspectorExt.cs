using System.Reflection;
using UnityEditor;
using UnityEngine;
using Prota.Unity;
using System;
using UnityEngine.UIElements;
using System.Text;

namespace Prota.Editor
{
    public static partial class UnityMethodExtensions
    {
        public static SerializedObject UpdateAndApply(this SerializedObject x)
        {
            x.Update(); // read value from obj.
            
            // var it = x.GetIterator();
            // var s = new StringBuilder();
            // while(it.Next(true))
            //     if(it.propertyType == SerializedPropertyType.Float)
            //         s.AppendLine(it.propertyPath + " " + it.floatValue);
            // Debug.LogError(s.ToString());
            
            x.ApplyModifiedProperties();    // tell editor that the object is edited.
            return x;
        }
        
        public static void RecordUndo(this UnityEngine.Object x, string info)
        {
            Undo.RecordObject(x, info);
            if(x is GameObject g && g.IsPrefab()) PrefabUtility.RecordPrefabInstancePropertyModifications(g);
        }
        
        public static Type GetActualFieldPropertyType(this PropertyDrawer self, SerializedProperty s)
        {
            if(self.fieldInfo.FieldType.IsConstructedGenericType)
            {
                return self.fieldInfo.FieldType.GetGenericArguments()[0];
            }
            return self.fieldInfo.FieldType;
        }
        
        public static SerializedProperty FindPropertyOfCSProperty(this SerializedObject s, string name)
        {
            return s.FindProperty(name.ToBackingFieldName());
        }
        
        public static string ToBackingFieldName(this string ss)
        {
            return $"<{ss}>k__BackingField";
        }
        
        static Color recordColor;
        public static void SetColor(this EditorWindow w, Color c)
        {
            recordColor = GUI.color;
            GUI.color = c;
        }
        public static void ResetColor(this EditorWindow w)
        {
            GUI.color = recordColor;
        }
        
        public static void SetColor(this UnityEditor.Editor w, Color c)
        {
            recordColor = GUI.color;
            GUI.color = c;
        }
        public static void ResetColor(this UnityEditor.Editor w)
        {
            GUI.color = recordColor;
        }
        
        
        public static void SeperateLine(this EditorWindow w, float height, Color color)
        {
            EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, height), color);
        }
        
        public static void SeperateLine(this UnityEditor.Editor w, float height, Color color)
        {
            EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, height), color);
        }
        
        
        public static bool AnyField(this UnityEditor.EditorWindow editor, string label, object target, FieldInfo f) => AnyFieldInternal(label, target, f);
        
        public static bool AnyField(this UnityEditor.Editor editor, string label, object target, FieldInfo f) => AnyFieldInternal(label, target, f);
        
        public static bool AnyFieldInternal(string label, object target, FieldInfo f)
        {
            if(f.FieldType == typeof(int))
            {
                f.SetValue(target, EditorGUILayout.IntField(label, (int)f.GetValue(target)));
            }
            else if(f.FieldType == typeof(float))
            {
                f.SetValue(target, EditorGUILayout.FloatField(label, (float)f.GetValue(target)));
            }
            else if(f.FieldType == typeof(long))
            {
                f.SetValue(target, EditorGUILayout.LongField(label, (long)f.GetValue(target)));
            }
            else if(f.FieldType == typeof(double))
            {
                f.SetValue(target, EditorGUILayout.DoubleField(label, (double)f.GetValue(target)));
            }
            else if(f.FieldType == typeof(Vector2))
            {
                f.SetValue(target, EditorGUILayout.Vector2Field(label, (Vector2)f.GetValue(target)));
            }
            else if(f.FieldType == typeof(Vector3))
            {
                f.SetValue(target, EditorGUILayout.Vector3Field(label, (Vector3)f.GetValue(target)));
            }
            else if(f.FieldType == typeof(Vector4))
            {
                f.SetValue(target, EditorGUILayout.Vector4Field(label, (Vector4)f.GetValue(target)));
            }
            else if(f.FieldType == typeof(Color))
            {
                f.SetValue(target, EditorGUILayout.ColorField(label, (Color)f.GetValue(target)));
            }
            else if(f.FieldType == typeof(Quaternion))
            {
                f.SetValue(target, EditorGUILayout.Vector4Field(label, ((Quaternion)f.GetValue(target)).ToVec4()).ToQuaternion());
            }
            else if(f.FieldType == typeof(Rect))
            {
                f.SetValue(target, EditorGUILayout.RectField((Rect)f.GetValue(target)));
            }
            else if(typeof(UnityEngine.Object).IsAssignableFrom(f.FieldType))
            {
                EditorGUILayout.ObjectField(label, f.GetValue(target) as UnityEngine.Object, typeof(UnityEngine.Object), true);
            }
            else return false;
            
            return true;
        }
        
    }
    
}
