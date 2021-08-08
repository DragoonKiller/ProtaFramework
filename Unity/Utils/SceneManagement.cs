using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Prota.Unity
{
    public static partial class Utils
    {
        public static class Scene
        {
            public static void ForeachScene(Action<UnityEngine.SceneManagement.Scene> f)
            {
                for(int i = 0; i < SceneManager.sceneCount; i++)
                {
                    var scene = SceneManager.GetSceneAt(i);
                    f(scene);
                }
            }
            
            
            
        }
        
    }
}