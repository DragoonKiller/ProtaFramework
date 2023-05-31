using UnityEngine;

namespace Prota.Unity
{
    public class TimerManager : Singleton<TimerManager>
    {
        void Start()
        {
            Timer.Start();
        }
        
        void Update()
        {
            Timer.Update();
        }
    }
}
