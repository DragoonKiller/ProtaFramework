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
        
        GameObject selectedGameObject;
        
        string selectAnimationName;
        
        Vector2 scrollPos;
        
        AnimationClip modified = null;
        
        class Modification
        {
            public EditorCurveBinding originalBinding;
            public string newPath;
            public string newName;
            public Type type;
        }
        
        Dictionary<string, Modification> modifications = new Dictionary<string, Modification>();
        
        void Update()
        {
            if(Selection.activeGameObject != null && Selection.activeGameObject != selectedGameObject)
            {
                selectedGameObject = Selection.activeGameObject;
                Repaint();
            }
        }
        
        void OnGUI()
        {
            if(!GetCurrentGameObject(out var g)) return;
            if(!GetAnimator(g, out var animator)) return;
            if(!GetAnimatorController(animator, out var controller)) return;
            if(!GetAnimation(ref selectAnimationName, controller, out var animationClip, out var floatBindings, out var objectBindings)) return;
            
            if(modified != null && animationClip != modified) RevertModification();
            
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.ExpandHeight(true));
            foreach(var binding in floatBindings.Concat(objectBindings))
            {
                var path = binding.path;
                var name = binding.propertyName;
                var type = binding.type;
                if(modifications.TryGetValue(binding.path, out var mod))
                {
                    path = mod.newPath;
                    name = mod.newName;
                    type = mod.type;
                }
                
                if(DrawBindingEditor(animator.gameObject, ref type, ref path, ref name))
                {
                    modifications[binding.path] = new Modification
                    {
                        originalBinding = binding,
                        newPath = path,
                        newName = name,
                        type = type
                    };
                    
                    modified = animationClip;
                }
            }
            EditorGUILayout.EndScrollView();
            
            if(modified && GUILayout.Button("Apply")) ApplyModification(animationClip);
            if(modified && GUILayout.Button("Revert")) RevertModification();
        }
        
        void RevertModification()
        {
            modifications.Clear();
            modified = null;
        }
        
        void ApplyModification(AnimationClip clip)
        {
            AssetDatabase.StartAssetEditing();
            try
            {
                Undo.RecordObject(clip, "Animation Rebind");
                
                List<Action> actions = new();
                
                foreach(var mod in modifications.Values)
                {
                    var newBinding = mod.originalBinding;
                    newBinding.path = mod.newPath;
                    newBinding.propertyName = mod.newName;
                    
                    var curve = AnimationUtility.GetEditorCurve(clip, mod.originalBinding);
                    var isFloatCurve = curve != null;
                    var objCurve = AnimationUtility.GetObjectReferenceCurve(clip, mod.originalBinding);
                    
                    if (isFloatCurve)
                    {
                        AnimationUtility.SetEditorCurve(clip, mod.originalBinding, null);
                        actions.Add(() => AnimationUtility.SetEditorCurve(clip, newBinding, curve));
                    }
                    else
                    {
                        AnimationUtility.SetObjectReferenceCurve(clip, mod.originalBinding, null);
                        actions.Add(() => AnimationUtility.SetObjectReferenceCurve(clip, newBinding, objCurve));
                    }
                }
                
                foreach(var action in actions) action();
                
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                modified = null;
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
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
            
            using EditorGUI.DisabledScope __ = new(true);
            EditorGUILayout.ObjectField("GameObject", g, typeof(GameObject), true);
            
            return true;
        }
        
        Animator cachedAnimator;
        bool GetAnimator(GameObject g, out Animator animator)
        {
            g.TryGetComponent<Animator>(out animator);
            if(animator == null && cachedAnimator == null)
            {
                EditorGUILayout.HelpBox("Please select a GameObject with Animator component.", MessageType.Warning);
                return false;
            }
            if(animator == null) animator = cachedAnimator;
            cachedAnimator = animator;
            
            using var _ = new EditorGUI.DisabledScope(true);
            EditorGUILayout.ObjectField("Animator", animator, typeof(Animator), true);
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
            
            using var _ = new EditorGUI.DisabledScope(true);
            EditorGUILayout.ObjectField("Animator Controller", controller, typeof(AnimatorController), true);
            return true;
        }
        
        bool GetAnimation(ref string selectAnimationName, AnimatorController controller,
            out AnimationClip animationClip,
            out EditorCurveBinding[] floatBindings,
            out EditorCurveBinding[] objBindings)
        {
            var animationNames = controller.animationClips.Select(c => c.name).ToArray();
            
            using var _ = new EditorGUILayout.HorizontalScope();
            
            var index = EditorGUILayout.Popup("Animation",
                Array.IndexOf(animationNames, selectAnimationName),
                animationNames,
                GUILayout.ExpandWidth(true)
            );
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
            
            using var __ = new EditorGUI.DisabledScope(true);
            EditorGUILayout.ObjectField("", animationClip, typeof(AnimationClip), true, GUILayout.Width(160));
            floatBindings = AnimationUtility.GetCurveBindings(animationClip);
            objBindings = AnimationUtility.GetObjectReferenceCurveBindings(animationClip);
            EditorGUILayout.IntField("", floatBindings.Length, GUILayout.Width(40));
            EditorGUILayout.IntField("", objBindings.Length, GUILayout.Width(40));
            
            return true;
        }
        
        bool DrawBindingEditor(GameObject g, ref Type type, ref string path, ref string name)
        {
            using var _ = new EditorGUILayout.HorizontalScope();
            
            var newPath = EditorGUILayout.TextField(path, GUILayout.MinWidth(200), GUILayout.ExpandWidth(true));
            var curTransform = g.transform.Find(path);
            var newTransform = EditorGUILayout.ObjectField("", curTransform, typeof(Transform), true, GUILayout.Width(200)) as Transform;
            
            if(curTransform != newTransform)
            {
                newPath = newTransform ? newTransform.RelativePath(g.transform) : "";
            }
            
            var curTarget = curTransform ? curTransform.GetComponent(type) : null;
            var newTarget = (Component)EditorGUILayout.ObjectField("", curTarget, typeof(Component), true, GUILayout.Width(200));
            var newType = type;
            if(newTarget != curTarget)
            {
                newPath = newTarget ? newTarget.transform.RelativePath(g.transform) : "";
                newType = newTarget ? newTarget.GetType() : null;
            }
            
            var newName = EditorGUILayout.TextField(name, GUILayout.Width(200));
            
            var ret = newPath != path || newName != name && newType != type;
            path = newPath;
            name = newName;
            type = newType;
            return ret;
        }
    }
    
}
