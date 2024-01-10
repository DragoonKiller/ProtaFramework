using System;
using UnityEngine;
using UnityEditor;
using System.Linq;
using Prota.Unity;
using UnityEditor.Animations;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Prota.Editor
{
    public class AnimationRebinder : EditorWindow
    {
        [MenuItem("ProtaFramework/Animation/Animation Rebind Window", priority = 600)]
        static void OpenWindow()
        {
            var window = GetWindow<AnimationRebinder>();
            window.titleContent = new GUIContent("Animation Rebind");
            window.Show();
        }
        
        string selectAnimationName;
        
        Vector2 scrollPos;
        
        AnimationClip modified = null;
        
        class Modification
        {
            public EditorCurveBinding originalBinding;
            public string newPath;
            public string newName;
        }
        
        Dictionary<string, Modification> modifications = new Dictionary<string, Modification>();
        
        void OnGUI()
        {
            if(!GetCurrentGameObject(out var g)) return;
            if(!GetAnimator(g, out var animator)) return;
            if(!GetAnimatorController(animator, out var controller)) return;
            if(!GetAnimation(ref selectAnimationName, controller, out var animationClip, out var floatBindings, out var objectBindings)) return;
            
            if(modified != null && animationClip != modified)
            {
                modifications.Clear();
                modified = null;
            }
            
            AssetDatabase.StartAssetEditing();
            try
            {
                Undo.RecordObject(animationClip, "Animation Rebind");
                
                scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.ExpandHeight(true));
                foreach(var binding in floatBindings.Concat(objectBindings))
                {
                    var path = binding.path;
                    var name = binding.propertyName;
                    if(modifications.TryGetValue(binding.path, out var mod))
                    {
                        path = mod.newPath;
                        name = mod.newName;
                    }
                    
                    if(DrawBindingEditor(g, ref path, ref name))
                    {
                        modifications[binding.path] = new Modification
                        {
                            originalBinding = binding,
                            newPath = path,
                            newName = name,
                        };
                        
                        modified = animationClip;
                    }
                }
                EditorGUILayout.EndScrollView();
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
            }
            
            if(modified)
            {
                if(GUILayout.Button("Apply"))
                {
                    List<Action> actions = new();
                    
                    foreach(var mod in modifications.Values)
                    {
                        var newBinding = mod.originalBinding;
                        newBinding.path = mod.newPath;
                        newBinding.propertyName = mod.newName;
                        
                        var curve = AnimationUtility.GetEditorCurve(animationClip, mod.originalBinding);
                        var isFloatCurve = curve != null;
                        var objCurve = AnimationUtility.GetObjectReferenceCurve(animationClip, mod.originalBinding);
                        
                        if (isFloatCurve)
                        {
                            AnimationUtility.SetEditorCurve(animationClip, mod.originalBinding, null);
                            actions.Add(() => AnimationUtility.SetEditorCurve(animationClip, newBinding, curve));
                        }
                        else
                        {
                            AnimationUtility.SetObjectReferenceCurve(animationClip, mod.originalBinding, null);
                            actions.Add(() => AnimationUtility.SetObjectReferenceCurve(animationClip, newBinding, objCurve));
                        }
                    }
                    
                    foreach(var action in actions) action();
                    
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    modified = null;
                }
            }
        }
        
        bool GetCurrentGameObject(out GameObject g)
        {
            g = Selection.activeGameObject;
            if(g == null)
            {
                EditorGUILayout.HelpBox("Please select a GameObject first.", MessageType.Info);
                return false;
            }
            
            using(new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.ObjectField("GameObject", g, typeof(GameObject), true);
            }
            
            return true;
        }
        
        bool GetAnimator(GameObject g, out Animator animator)
        {
            animator = g.GetComponent<Animator>();
            if(animator == null)
            {
                EditorGUILayout.HelpBox("Please select a GameObject with Animator component.", MessageType.Warning);
                return false;
            }
            
            using(new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.ObjectField("Animator", animator, typeof(Animator), true);
            }
            return true;
        }
        
        bool GetAnimatorController(Animator animator, out AnimatorController controller)
        {
            controller = animator.runtimeAnimatorController as AnimatorController;
            if(controller == null)
            {
                EditorGUILayout.HelpBox("There must be an AnimatorController attached to Animator.", MessageType.Warning);
                return false;
            }
            
            using(new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.ObjectField("Animator Controller", controller, typeof(AnimatorController), true);
            }
            return true;
        }
        
        bool GetAnimation(ref string selectAnimationName, AnimatorController controller,
            out AnimationClip animationClip,
            out EditorCurveBinding[] floatBindings,
            out EditorCurveBinding[] objBindings)
        {
            var animationNames = controller.animationClips.Select(c => c.name).ToArray();
            var index = EditorGUILayout.Popup("Animation", Array.IndexOf(animationNames, selectAnimationName), animationNames);
            if(index < 0)
            {
                EditorGUILayout.HelpBox("Please Select an animation.", MessageType.Info);
                selectAnimationName = null;
                animationClip = null;
                floatBindings = null;
                objBindings = null;
                return false;
            }
            selectAnimationName = animationNames[index];
            animationClip = controller.animationClips[index];
            
            using(new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.ObjectField("Animation Clip", animationClip, typeof(AnimationClip), true);
                floatBindings = AnimationUtility.GetCurveBindings(animationClip);
                objBindings = AnimationUtility.GetObjectReferenceCurveBindings(animationClip);
                EditorGUILayout.IntField("", floatBindings.Length, GUILayout.Width(40));
                EditorGUILayout.IntField("", objBindings.Length, GUILayout.Width(40));
                EditorGUILayout.EndHorizontal();
            }
            
            return true;
        }
        
        bool DrawBindingEditor(GameObject g, ref string path, ref string name)
        {
            EditorGUILayout.BeginHorizontal();
            var newPath = EditorGUILayout.TextField(path, GUILayout.MinWidth(400), GUILayout.ExpandWidth(true));
            var curTransform = g.transform.Find(path);
            var curObject = curTransform ? curTransform.gameObject : null;
            var newObject = (GameObject)EditorGUILayout.ObjectField("", curObject, typeof(GameObject), true, GUILayout.Width(100));
            var newName = EditorGUILayout.TextField(name, GUILayout.MinWidth(200));
            if(newObject != curObject) newPath = newObject ? newObject.transform.RelativePath(g.transform) : "";
            EditorGUILayout.EndHorizontal();
            var ret = newPath != path || newName != name;
            path = newPath;
            name = newName;
            return ret;
        }
    }
    
}
