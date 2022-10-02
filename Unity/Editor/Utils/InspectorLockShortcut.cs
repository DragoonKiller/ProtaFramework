using System;
using System.Reflection;
using UnityEditor;


namespace Prota.Unity
{
    // copied from: https://forum.unity.com/threads/shortcut-key-for-lock-inspector.95815/
    public class InspectorLockToggle
    {
        [MenuItem("ProtaFramework/Tools/Toggle Lock _l")]
        static void ToggleInspectorLock() // Inspector must be inspecting something to be locked
        {
            EditorWindow inspectorToBeLocked = EditorWindow.mouseOverWindow; // "EditorWindow.focusedWindow" can be used instead
        
            if (inspectorToBeLocked != null  && inspectorToBeLocked.GetType().Name == "InspectorWindow")
            {
                Type type = Assembly.GetAssembly(typeof(UnityEditor.Editor)).GetType("UnityEditor.InspectorWindow");
                PropertyInfo propertyInfo = type.GetProperty("isLocked");
                bool value = (bool)propertyInfo.GetValue(inspectorToBeLocked, null);
                propertyInfo.SetValue(inspectorToBeLocked, !value, null);
                inspectorToBeLocked.Repaint();
            }
        }
    }
}