
using UnityEngine;
using UnityEditor;
using Prota.Unity;
using Prota;

namespace Prota.Editor
{
    public static partial class ProtaEditorCommands
    {
        [MenuItem("ProtaFramework/Tools/Do All Unit Test")]
        public static void UnitTestAll()
        {
            UnityUnitTest.UnitTestSerializableLinkedList(Debug.Log);
            UnityUnitTest.UnitTestSerializableDictionary();
            IniParser.UnitTest();
            UnitTest.TestCircleArray();
            UnitTest.TestCircleDualPointer();
        }
    }
}
