using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Prota.Unity
{
    public class EFilter<T> where T : EComponent
    {
        public HashSet<Type> required = new HashSet<Type>();
        public HashSet<Type> excluded = new HashSet<Type>();
        
        IEnumerable<T> anchorList => EComponent.Instances<T>();
        
        public EFilter<T> With<H>() where H : EComponent
        {
            required.Add(typeof(H));
            return this;
        }
        
        public EFilter<T> Without<H>() where H : EComponent
        {
            excluded.Add(typeof(H));
            return this;
        }
        
        public bool Match(ERoot entity)
        {
            if(entity == null) return false;
            if(!entity.HasEntityComponent<T>()) return false;
            if(required.Any(x => !entity.HasEntityComponent(x))) return false;
            if(excluded.Any(x => entity.HasEntityComponent(x))) return false;
            return true;
        }
        
        public IEnumerable<T> result => anchorList.Where(x => Match(x.entity));
    }
}

