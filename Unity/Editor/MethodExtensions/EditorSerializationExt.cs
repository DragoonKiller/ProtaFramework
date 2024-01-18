
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace Prota.Editor
{
    public static partial class UnityMethodExtensions
    {
        public static void SetManagedRefValueNull(this SerializedProperty property)
        {
            property.managedReferenceId = UnityEngine.Serialization.ManagedReferenceUtility.RefIdNull;
        }
        
        public static T GetSerializedReferenceValue<T>(this SerializedObject x, int id)
        {
            var obj = UnityEngine.Serialization.ManagedReferenceUtility.GetManagedReference(x.targetObject, id);
            return (T) obj;
        }
        
        public static bool IsManagedRef(this SerializedProperty x)
        {
            return x.propertyType == SerializedPropertyType.ManagedReference;
        }
        
        public static SerializedProperty SubField(this SerializedProperty x, string name)
        {
            return x.FindPropertyRelative(name);
        }
        
        public static SerializedProperty SubBackingField(this SerializedProperty x, string name)
        {
            return x.FindPropertyRelative(name.ToBackingFieldName());
        }
        
        
        public static IEnumerable<SerializedProperty> EnumerateAsList(this SerializedProperty x)
        {
            var n = x.arraySize;
            for(int i = 0; i < n; i++)
            {
                yield return x.GetArrayElementAtIndex(i);
            }
        }
        
        public static SerializedProperty FindAsList(this SerializedProperty x, Func<SerializedProperty, bool> cond)
        {
            return x.EnumerateAsList().FirstOrDefault(cond);
        }
        
        public static SerializedProperty  AddAsList(this SerializedProperty x)
        {
            var n = x.arraySize;
            x.InsertArrayElementAtIndex(n);
            return x.GetArrayElementAtIndex(n);
        }
        
        public static SerializedProperty RemoveAsList(this SerializedProperty x, Func<SerializedProperty, bool> cond)
        {
            var n = x.arraySize;
            for(int i = 0; i < n; i++)
            {
                var e = x.GetArrayElementAtIndex(i);
                if(cond(e))
                {
                    x.DeleteArrayElementAtIndex(i);
                    return e;
                }
            }
            return null;
        }
        
        public static SerializedProperty RemoveAllAsList(this SerializedProperty x, Func<SerializedProperty, bool> cond)
        {
            var n = x.arraySize;
            for(int i = n - 1; i >= 0; i--)
            {
                var e = x.GetArrayElementAtIndex(i);
                if(cond(e))
                {
                    x.DeleteArrayElementAtIndex(i);
                }
            }
            return null;
        }
    }
}
