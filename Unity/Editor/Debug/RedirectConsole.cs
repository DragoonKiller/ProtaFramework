 
using UnityEngine;
using UnityEditor;

namespace Prota.Editor
{
    public static class RedirectConsoleOutput
    {
    
        public class DebugLogWriter : System.IO.TextWriter
        {
            public override void Write(string value)
            {
                base.Write(value);
                value = value.Replace("\0", "");
                if(string.IsNullOrWhiteSpace(value)) return;
                if(value.StartsWith("[Exception]") || value.StartsWith("[Error]"))
                    Debug.LogError(value);
                else
                    Debug.Log(value);
            }
            
            public override System.Text.Encoding Encoding => System.Text.Encoding.Default;
        }
        
        [InitializeOnLoadMethod]
        public static void Redirect()
        {
            System.Console.SetOut(new DebugLogWriter());
        } 
    }
}