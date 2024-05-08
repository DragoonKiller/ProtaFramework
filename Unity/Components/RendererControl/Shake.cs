using UnityEngine;
using System;
using System.Collections.Generic;

namespace Prota.Unity
{
    // 实际震动幅度 = 震动幅度 * max(0, 强度 ^ 能量 - 1)
    // 震动会以频率改变移动的目标.
    // 能量会根据衰减和削减减少: 能量 = 能量 * (衰减 ^ dt) - 削减 * dt;
    
    [DisallowMultipleComponent]
    [ExecuteAlways]
    public class Shake : MonoBehaviour
    {
        public float strength = 2;
        
        public float decay = 0.5f;
        
        public float deduce = 0.5f;
        
        public float frequency = 20;
        
        public float amplitude = 1;
        
        public float maxEnergy = 10;
        
        [SerializeField, Readonly] Vector3 oriPos;
        
        [Header("State")]
        [SerializeField] float energy;
        
        float speed = 0;
        
        Vector3 restMove = Vector3.zero;
        
        Vector3 randomMove => Vector2.up.Rotate((0f, 360f).Random()) * amplitude * (strength.Pow(energy) - 1).Max(0);
        
        public void AddEnergy(float e)
        {
            energy += e / energy.Max(1);
            energy = energy.Min(maxEnergy);
        }
        
        void Update()
        {
            if(Application.isPlaying)
            {
                TryNextShakeStep();
                ProcessShakeStep();
                
                energy *= decay.Pow(Time.deltaTime);
                energy -= deduce * Time.deltaTime;
                energy = energy.Max(0);
            }
            else
            {
                oriPos = this.transform.localPosition;
            }
        }
        
        void TryNextShakeStep()
        {
            if(restMove != Vector3.zero) return;
            if(energy <= 0) return;
            NewShakeStep();
        }
        
        void NewShakeStep()
        {
            restMove = this.transform.localPosition.To(oriPos + randomMove);
            var duration = 1 / frequency / (energy + 1);
            speed = restMove.magnitude / duration;
        }
        
        void ProcessShakeStep()
        {
            if(restMove == Vector3.zero) return;
            var curMove = restMove.normalized * speed * Time.deltaTime;
            if(curMove.magnitude > restMove.magnitude) curMove = restMove;
            restMove -= curMove;
            this.transform.localPosition += curMove;
        }
        
    }
}
