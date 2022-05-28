using Prota.Unity;
using UnityEngine;

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