using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;

using Prota.Unity;
using System.Runtime.ConstrainedExecution;

namespace Prota.Editor
{
    [InitializeOnLoad]
    public class ExecutionOrderManager : UnityEditor.Editor
    {
        static ExecutionOrderManager()
        {
            int? parallaxOrder = null;
            int? cinemachineVirtualCameraOrder = null;
            
            foreach (MonoScript monoScript in MonoImporter.GetAllRuntimeMonoScripts())
            {
                if(monoScript.GetClass() == typeof(Parallax))
                    parallaxOrder = MonoImporter.GetExecutionOrder(monoScript);
                    
                if(monoScript.name == "CinemachineVirtualCamera")
                    cinemachineVirtualCameraOrder = MonoImporter.GetExecutionOrder(monoScript);
            }
            
            if(parallaxOrder == null) throw new Exception("Parallax not found.");
            if(cinemachineVirtualCameraOrder != null && cinemachineVirtualCameraOrder.Value >= parallaxOrder.Value)
            {
                throw new Exception("CinemachineVirtualCamera should be executed before Parallax.");
            }
        }
    }
}
