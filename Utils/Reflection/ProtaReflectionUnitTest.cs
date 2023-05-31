using System;
using System.Reflection;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Prota
{
    
    internal static class ProtaReflectionUnitTest
    {
        private class TestClass
        {
            int a;
            protected int b;
            public int c;
            public int pp { get; private set; }
            const int g = 11;
            public int this[int q]
            {
                get => a * b - q;
                
                set
                {
                    a = q;
                    b = value;
                }
            }
            public int getA => a;
        }
        
        internal struct TA<T, G>
        {
            public class TG<X>
            {
                X af;
            }
        }
        
        // [MenuItem("Prota/Test 1")]
        public static void Test()
        {
            {
                new ProtaReflectionType(typeof(List<int>)).code.Log();
                new ProtaReflectionType(typeof(List<(int,int)>)).code.Log();
                new ProtaReflectionType(typeof(List<Dictionary<string, int>>)).code.Log();
                new ProtaReflectionType(typeof(List<Dictionary<string, List<int>>>)).code.Log();
                new ProtaReflectionType(typeof(TA<,>)).code.Log();
                new ProtaReflectionType(typeof(TA<int,TA<int, string>.TG<List<int>>>)).code.Log();
                new ProtaReflectionType(typeof(TA<int,TA<int, string>.TG<List<int>>>.TG<TA<double,double>>)).code.Log();
                
            }
            
            {
                var g = new ProtaReflectionType(typeof(List<int>));
                g.HasMember("Capacity").Assert();
                g.HasProperty("Capacity").Assert();
                (!g.HasField("Capacity")).Assert();
                (!g.HasMethod("Capacity")).Assert();
                g.HasMethod("Add").Assert();
                g.HasMethod("AddRange").Assert();
                g.IsPrivate("_items").Assert();
                g.IsPublic("Add").Assert();
                g.IsPublic("Count").Assert();
                
                g = new ProtaReflectionType(typeof(Math));
                g.IsMethodOverloaded("Abs").Assert();
                g.IsStatic("Abs", typeof(int)).Assert();
                g.IsStatic("Sin", typeof(int)).Assert();
                g.IsConstant("PI").Assert();
            }
            
            
            {
                var c = new List<int>();
                var obj = c.ProtaReflection();
                var ot = obj.type;
                ot.allMembers.Select(x => x.Name).ToArray().Join("\n").Log();
                
                (c.Capacity == (int)obj.Get("Capacity")).Assert();
                obj.Call("Add", 1);
                
                (c.Count == (int)obj.Get("Count")).Assert();
                (c.Count == 1).Assert();
                
                c.Capacity = 100;
                (c.Capacity == (int)obj.Get("Capacity")).Assert();
                ((int)obj.Get("Capacity") == 100).Assert();
                
                ((int)obj["Count"] == 1).Assert();
                ((int)obj["Capacity"] == 100).Assert();
                ((int)obj["Count"] == c.Count).Assert();
                
                var ll = obj["_items"];
                (ll is int[]).Assert();
                ((ll as int[]).Length == 100).Assert();
                
                obj.Call("AddRange", new int[]{1, 2, 3});
                (c.Count == 4).Assert();
                (c[3] == 3).Assert();
                (c[2] == 2).Assert();
                (c[1] == 1).Assert();
                (c[0] == 1).Assert();
                
                var items = (int[])obj.Get("_items");
                (items[0] == 1).Assert();
                (items[1] == 1).Assert();
                (items[2] == 2).Assert();
                (items[3] == 3).Assert();
            }
            
            
            {
                var x = new TestClass().PassValue(out var v).ProtaReflection();
                var type = x.type;
                
                type.allMembers.ToStrings().Join("\n").Log();
                
                (type.GetMember("a") is FieldInfo).Assert();
                (type.GetMember("b") is FieldInfo).Assert();
                (type.GetMember("c") is FieldInfo).Assert();
                (type.GetMember("pp") is PropertyInfo).Assert();
                (type.GetMember("g") is FieldInfo).Assert();
                
                (!type.IsPublic("a")).Assert();
                (!type.IsPublic("b")).Assert();
                type.IsProtected("b").Assert();
                type.IsPublic("c").Assert();
                type.GetProperty("pp").GetGetMethod(true).IsPublic.Assert();
                (!type.GetProperty("pp").GetSetMethod(true).IsPublic).Assert();
                
                (v.getA == 0).Assert();
                ((int)x.Get("a") == 0).Assert();
                x.Set("a", 1);
                (v.getA == 1).Assert();
                ((int)x.Get("a") == 1).Assert();
                
                ((int)x.Get("b") == 0).Assert();
                x.Set("b", 2);
                ((int)x.Get("b") == 2).Assert();
                
                ((int)x.Get("c") == 0).Assert();
                x.Set("c", 3);
                ((int)x.Get("c") == 3).Assert();
                
                x.Set("pp", 9);
                (v.pp == 9).Assert();
                ((int)x.Get("pp") == 9).Assert();
                
                ((int)x.Get("g") == 11).Assert();
                ((int)x.GetIndexer(1) == 1).Assert();        // a = 1, b = 2, a * b - 1 = 1.
                
                x.SetIndexer(3, 2);
                ((int)x.GetIndexer(1) == 5).Assert();        // a = 3, b = 2, a * b - 1 = 5.
                
                ((int)x.Get("b") == 3).Assert();
                (v.getA == 2).Assert();
                
            }
            
        }
    }
}
