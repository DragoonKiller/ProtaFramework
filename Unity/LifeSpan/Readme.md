## LifeSpan

LifeSpan 是一个代表着生命周期的对象.

new LifeSpan() 代表周期开始; LifeSpan.Kill 被执行代表生命周期结束.

LifeSpan.OnComplete 方法用于注册生命周期结束时需要做的事情. 通过这一接口来对齐生命周期.

LifeSpan.alive 用于查询生命周期是否结束了.


LifeSpanBinding 是一个用于绑定GameObject生命周期的**组件**.
也就是说: 它使用组件的方式, 让我们能够将任何东西绑定到 GameObject 所对应的 LifeSpan 上.
