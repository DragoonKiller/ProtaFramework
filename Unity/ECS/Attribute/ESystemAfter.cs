using System;

namespace Prota.Unity
{
    public class ESystemAfter : Attribute
    {
        public Type[] types;
        public ESystemAfter(params Type[] types) => this.types = types.WithDuplicateRemoved();
    }
}
