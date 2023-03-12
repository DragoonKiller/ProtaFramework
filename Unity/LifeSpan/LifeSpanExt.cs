using System;
using System.Collections.Generic;

namespace Prota.Unity
{
    public static partial class LifeSpanExt
    {
        // 绑定生命周期的回调注册接口. 这样就不用在 OnDestory 手动反注册了.
        // **注意不允许重复注册**
        public static EventQueue AddCallback<T>(this EventQueue q, LifeSpan lifeSpan, Action<T> a) where T: IEvent
        {
            q.AddCallback(a);
            lifeSpan.OnKill(() => q.RemoveCallback(a));
            return q;
        }
    }
}
