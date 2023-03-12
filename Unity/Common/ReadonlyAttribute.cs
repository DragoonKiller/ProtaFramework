using UnityEngine;
using UnityEditor;

namespace Prota
{
    public class Readonly : PropertyAttribute
    {
        public bool whenPlaying = true;
        public bool whenEditing = true;
    }
 
}
