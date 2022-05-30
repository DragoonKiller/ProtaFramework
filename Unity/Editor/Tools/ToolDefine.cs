using System;


namespace Prota.Editor
{
    
    public class ProtaToolAttribute : Attribute
    {
        string name;
        string desc;
        public ProtaToolAttribute(string name, string desc)
        {
            this.name = name;
            this.desc = desc;
        }
    }
    
    public class ProtaToolCategoryAttribute : Attribute
    {
        public string catName;
        public ProtaToolCategoryAttribute(string catName) => this.catName = catName;
    }
    
    
    
}