# ECS 编写指南

1. Component 命名以 EC 为前缀; System 命名以 ES 为前缀.
2. Component 是单纯的数据项, 其逻辑只能操作自身的数据, 不可引用其它 Component 和 System (但是可以引用 Unity 原生组件).
3. System 之间不能相互引用, System 之间的交互通过 Component 完成.
4. 当使用异步流程来写每个对象不相同的更新逻辑(而不是用类)时, 需要遵守一些规则.
   1. 在 EComponent 中添加 AsyncControl 对象, 使其控制其自身的逻辑. 所有 await 操作都由 asyncControl 完成.
   2. 由各个 EComponent 对应的 System 调用 AsyncControl.Step() 来控制其执行更新.
   3. GameObject 被销毁时, Component 被删除, AsyncControl 对象被释放, 便不再更新.
   4. 注意对应的更新函数是写在 System 里的.

