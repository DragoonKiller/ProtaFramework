using System;
using UnityEngine;
using UnityEditor;
using System.Globalization;
using System.IO;

namespace Prota.Editor
{
    public static class Screenshot
    {
        [MenuItem("ProtaFramework/Tools/Capture Screen")]
        public static void CaptureScreenAndSave()
        {
            var d = new DirectoryInfo("./Screenshots/");
            if(!d.Exists) d.Create();
            var path = $"./Screenshots/{ DateTime.Now.ToString("yy-MM-dd-HH-mm-ss", DateTimeFormatInfo.InvariantInfo) }.png";
            ScreenCapture.CaptureScreenshot(path);
        }
    }
}