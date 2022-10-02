using UnityEngine;

using Prota.Unity;

namespace Prota.Timer
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