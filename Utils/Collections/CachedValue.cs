using System;


namespace Prota
{
    public struct CachedValue<T>
    {
        T _value;
        public T value
        {
            get
            {
                if(isDirty && getter != null)
                {
                    _value = getter();
                    isDirty = false;
                    if(isFixed)
                    {
                        getter = null;
                    }
                }
                return _value;
            }
        }
        
        public bool isDirty;
        public bool isFixed { get; private set; }
        
        public Func<T> getter;
        
        public CachedValue(Func<T> getter, bool isFixed = false)
        {
            this.getter = getter;
            _value = default(T);
            isDirty = true;
            this.isFixed = isFixed;
        }
    }
}
