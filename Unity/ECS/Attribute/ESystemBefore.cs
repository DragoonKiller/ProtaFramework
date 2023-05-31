using System;
namespace Prota.Unity
{
    public class ESystemBefore : Attribute
    {
        public Type[] types;
        public ESystemBefore(params Type[] types) => this.types = types.WithDuplicateRemoved();
    }
}
