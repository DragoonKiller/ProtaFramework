using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

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
