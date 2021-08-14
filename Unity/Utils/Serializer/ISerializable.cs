using System;


namespace Prota.Unity
{
    public interface ISerializable
    {
        public void Serialize(SerializedData s);
        
        public void Deserialize(SerializedData s);
    }
}