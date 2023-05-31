using UnityEngine;

namespace Prota.Unity
{
    public enum AutoDestroyType
    {
        Time,
    }
    
    public class AutoDestroy : MonoBehaviour
    {
        public AutoDestroyType type;
        
        [Header("time")]
        public float time;
        
        void Start()
        {
            switch(type)
            {
                case AutoDestroyType.Time:
                    Invoke("DoActiveDestroy", time);
                    break;
            }
        }
        
        void DoActiveDestroy() => this.gameObject.ActiveDestroy();
    }
}
