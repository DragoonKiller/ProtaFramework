using UnityEngine;
using UnityEditor;
using Prota.Unity;
using Prota.Animation2D;
using Prota.Editor;

namespace Prota.Animation2D
{
    public static partial class Animation2DEditor
    {
        
        [MenuItem("Assets/ProtaFramework/动画/比较两个贴图", priority = 5)]
        static void CompareTextures()
        {
            var s = Selection.objects;
            if(s.Length != 2)
            {
                Debug.LogError("请选择恰好两个图片资源.");
                return;
            }
            
            var a = s[0] as Texture2D;
            var b = s[1] as Texture2D;
            if(a == null || b == null)
            {
                Debug.LogError("选择的资源不是图片.");
                return;
            }
            
            Debug.Log("比较贴图: " + AssetDatabase.GetAssetPath(a) + " <=> " + AssetDatabase.GetAssetPath(b));
            
            var dc = 0;
            foreach(var d in a.Compare(b))
            {
                dc++;
                Debug.LogFormat("({0}, {1}) 的像素颜色不同: {2} {3}", d.x, d.y, d.c1, d.c2);
            }
            if(dc == 0) Debug.Log("没有区别!");
        }
    }
}