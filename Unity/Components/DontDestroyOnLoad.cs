using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine.UIElements;

namespace Prota.Unity
{
    public class DontDestroyOnLoad : MonoBehaviour
    {
        void Awake()
        {
            this.gameObject.SetToDontDestroyScene();
        }
    }
}
