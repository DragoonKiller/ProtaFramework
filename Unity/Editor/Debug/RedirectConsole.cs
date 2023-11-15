 
using UnityEngine;
using UnityEditor;

namespace Prota.Editor
{
    public static class RedirectConsoleOutput
    {
        public class DebugLogWriter : System.IO.TextWriter
        {
            [HideInCallstack]
            public override void Write(string value)
            {
                base.Write(value);
                value = value.Replace("\0", "");
                if(string.IsNullOrWhiteSpace(value)
                    || value.StartsWith("[MODES]")
                    || value.StartsWith("[LAYOUT]")
                    || value.StartsWith("[ScriptCompilation]")
                    || value.EndsWith("belong to any assembly definition file.")
                ) return;
                
                if(value.StartsWith("[Exception]") || value.StartsWith("[Error]") || value.Contains("exception: "))
                    Debug.LogError(value);
                else if(value.StartsWith("[Warning]", System.StringComparison.OrdinalIgnoreCase))
                    Debug.LogWarning(value);
                else
                    Debug.Log(value);
            }
            
            public override System.Text.Encoding Encoding => System.Text.Encoding.Default;
        }
        
    
        public class DebugLogErrorWriter : System.IO.TextWriter
        {
            [HideInCallstack]
            public override void Write(string value)
            {
                base.Write(value);
                value = value.Replace("\0", "");
                if(string.IsNullOrWhiteSpace(value)) return;
                Debug.LogError(value);
            }
            
            public override System.Text.Encoding Encoding => System.Text.Encoding.Default;
        }
        
        [InitializeOnLoadMethod]
        public static void Redirect()
        {
            System.Console.SetOut(new DebugLogWriter());
            System.Console.SetError(new DebugLogErrorWriter());
        } 
    }
}
