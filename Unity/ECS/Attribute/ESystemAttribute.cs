using System;

namespace Prota.Unity
{
    // ECS 根据数据的生产和使用顺序推算出 System 的执行过程.
    // 生产数据的 System 会在使用数据的 System 之前执行.
    // 清理数据的 System 会在使用数据的 System 之后执行.
    
    public class ESystemDataProvider : Attribute
    {
        public Type[] types;
        public ESystemDataProvider(params Type[] types) => this.types = types.WithDuplicateRemoved();
    }
    
    public class ESystemDataAccessor : Attribute
    {
        public Type[] types;
        public ESystemDataAccessor(params Type[] types) => this.types = types.WithDuplicateRemoved();
    }
    
    public class ESystemDataClearer : Attribute
    {
        public Type[] types;
        public ESystemDataClearer(params Type[] types) => this.types = types.WithDuplicateRemoved();
    }
}
