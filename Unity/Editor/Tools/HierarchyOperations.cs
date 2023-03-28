using System;
using UnityEngine;
using UnityEditor;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Prota.Editor
{
    public static partial class ProtaEditorCommands
    {
        [MenuItem("GameObject/Prota Framework/Hierarchy Operations/Set Local Position to 0 _u", false, 10)]
        public static void SetLocalPositionToZero()
        {
            var gs = Selection.gameObjects.Where(x => x.scene != null).ToArray();
            Undo.RecordObjects(gs, "Set Local Position to 0");
            foreach(var g in gs)
            {
                g.transform.localPosition = Vector3.zero;
                EditorUtility.SetDirty(g);
            }
        }
        
        [MenuItem("GameObject/Prota Framework/Hierarchy Operations/Set Local Scale to 1 _i", false, 10)]
        public static void SetLocalScaleToOne()
        {
            var gs = Selection.gameObjects.Where(x => x.scene != null).ToArray();
            Undo.RecordObjects(gs, "Set Local Scale to 1");
            foreach(var g in gs)
            {
                g.transform.localScale = Vector3.one;
                EditorUtility.SetDirty(g);
            }
        }
        
        [MenuItem("GameObject/Prota Framework/Hierarchy Operations/Set Local Rotation to Identity _o", false, 10)]
        public static void SetLocalRotationToId()
        {
            var gs = Selection.gameObjects.Where(x => x.scene != null).ToArray();
            Undo.RecordObjects(gs, "Set Local Rotation to Identity");
            foreach(var g in gs)
            {
                g.transform.localRotation = Quaternion.identity;
                EditorUtility.SetDirty(g);
            }
        }
        
        [MenuItem("GameObject/Prota Framework/Hierarchy Operations/Group Selection %g", false, 10)]
        public static void GroupSelection()
        {
            var gs = Selection.gameObjects.Where(x => x.scene != null).ToArray();
            if(gs.Length == 0) return;
            Undo.RecordObjects(gs, "Group Selection");
            var g = new GameObject("Group");
            Undo.RecordObject(g, "Group Selection");
            g.transform.SetParent(gs[0].transform.parent);
            g.transform.localPosition = Vector3.zero;
            g.transform.SetSiblingIndex(gs[0].transform.GetSiblingIndex());
            EditorUtility.SetDirty(g);
            foreach(var gx in gs)
            {
                gx.transform.SetParent(g.transform, true);
                EditorUtility.SetDirty(gx);
            }
            
            Selection.activeGameObject = g;
        }
    }
}
