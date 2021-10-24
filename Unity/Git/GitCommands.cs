using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Diagnostics;
using System.IO;

namespace Prota.Unity
{
    public static class GitCommands
    {
        [MenuItem("Assets/Git/Reset", false)]
        static void GitReset()
        {
            var path = AssetDatabase.GetAssetPath(Selection.activeInstanceID);
            
            var start = new ProcessStartInfo();
            var filePath = Path.GetFullPath(path);
            start.FileName = "git";
            start.Arguments = $"checkout HEAD -- \"{ filePath }\"";
            start.WorkingDirectory = Path.GetDirectoryName(filePath);
            start.RedirectStandardError = true;
            start.RedirectStandardOutput = true;
            start.UseShellExecute = false;
            UnityEngine.Debug.Log("cwd: " + start.WorkingDirectory);
            UnityEngine.Debug.Log("cmd: " + start.FileName + " " + start.Arguments);
            var proc = Process.Start(start);
            proc.ErrorDataReceived += (o, e) => UnityEngine.Debug.LogError(e.Data);
            proc.BeginErrorReadLine();
            proc.OutputDataReceived += (o, e) => UnityEngine.Debug.LogWarning(e.Data);
            proc.BeginOutputReadLine();
            proc.WaitForExit();
        }
        
        [MenuItem("Assets/Git/Reset", true)]
        static bool GitResetValid()
        {
            var path = AssetDatabase.GetAssetPath(Selection.activeInstanceID);
            return !string.IsNullOrEmpty(path);
        }
        
    }
    
    
    
}