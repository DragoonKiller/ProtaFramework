
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
    }
}
