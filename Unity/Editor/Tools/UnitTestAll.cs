
using UnityEngine;
using UnityEditor;
namespace Prota.Editor
{
    public static partial class ProtaEditorCommands
    {
        [MenuItem("ProtaFramework/Tools/Do All Unit Test")]
        public static void UnitTestAll()
        {
            UnitTest.UnitTestSerializableLinkedList(Debug.Log);
            UnitTest.UnitTestSerializableDictionary();
            
        }
    }
}
