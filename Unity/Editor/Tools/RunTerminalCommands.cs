using System;
using UnityEngine;
using UnityEditor;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Prota.Editor
{
    public static partial class ProtaEditorCommands
    {
        public static Process ExecuteAsCommand(this string x)
        {
            var info = new ProcessStartInfo();
            info.RedirectStandardError = true;
            info.RedirectStandardInput = true;
            info.RedirectStandardOutput = true;
            info.UseShellExecute = false;
            info.WorkingDirectory = Path.Combine(Application.dataPath, "../");
            var cc = x.Split(" ", 2, StringSplitOptions.None);
            info.FileName = cc[0];
            info.Arguments = cc[1];
            var p = Process.Start(info);
            
            Console.WriteLine($"Execute command: [{ info.FileName }] [{ info.Arguments }]");
            
            Task.Run(async () => {
                while(!p.HasExited && p.StandardOutput != null)
                {
                    var s = await p.StandardOutput.ReadLineAsync();
                    if(s != null) UnityEngine.Debug.Log(s);
                }
            });
            
            Task.Run(async () => {
                while(!p.HasExited && p.StandardOutput != null)
                {
                    var s = await p.StandardError.ReadLineAsync();
                    if(s != null) UnityEngine.Debug.LogError(s);
                }
            });
            
            return p;
        }
    }
}
