using UnityEngine;
using System;
using System.Collections.Generic;


namespace Prota.Unity
{
    public class GroupBehaviour<T> : MonoBehaviour
        where T: GroupBehaviour<T>
    {
        public readonly static HashSet<GroupBehaviour<T>> instances = new HashSet<GroupBehaviour<T>>();
        
        protected virtual void Awake()
        {
            instances.Add(this);
        }
        
        protected virtual void OnDestroy()
        {
            instances.Remove(this);
        }
        
    }
}