using UnityEngine;

namespace Prota.Timer
{
    public class TimerManager : MonoBehaviour
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