using UnityEngine;
using UnityEditor;

namespace Prota.Unity
{
    public class Readonly : PropertyAttribute
    {
        public bool whenPlaying = true;
        public bool whenEditing = true;
        public bool hideWhenEditing = false;
        public bool hideWhenPlaying = false;
    }
 
}
