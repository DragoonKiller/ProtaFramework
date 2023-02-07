using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Prota.Unity;
namespace Prota.Tween
{
    // h: Tween handle with all tweening information
    // t: current ratio, [0, 1]
    public delegate void ValueTweeningUpdate(TweenHandle h, float t);
    
    
    // TweenId 用来分辨互斥的 tweening.
    // 例如当前对象有一个[从A移动到B]的 tween 正在执行.
    // 如果在中途又创建了一个 tween 叫它[从当前位置移动到C], 那么这两个操作冲突, 原来的 tween 会被删除.
    // 但是如果中途创建了一个旋转自身的 tween, 那么两个 tween 可以同时执行互不干扰.
    public struct TweenId : IEquatable<TweenId>, IComparable<TweenId>
    {
        public readonly int id;
        public readonly string name;

        public TweenId(int id, string name = null)
        {
            this.id = id;
            this.name = name;
        }
        
        public TweenId(string name) : this(0, name) { }
        
        public bool isNone => id == 0 && name == null;
        
        public static TweenId None => new TweenId();
        
        public static TweenId MoveX => 1;
        public static TweenId  MoveY => 2;
        public static TweenId  MoveZ => 3;
         
        public static TweenId  ScaleX => 4;
        public static TweenId  ScaleY => 5;
        public static TweenId  ScaleZ => 6;
         
        public static TweenId  RotateX => 7;
        public static TweenId  RotateY => 8;
        public static TweenId  RotateZ => 9;
         
        public static TweenId  ColorR => 10;
        public static TweenId  ColorG => 11;
        public static TweenId  ColorB => 12;
        public static TweenId  Transparency => 13;
        
        public bool Equals(TweenId i) => i.id == this.id && i.name == this.name;
        public override bool Equals(object obj) => obj is TweenId type && id == type.id && name == type.name;
        public override int GetHashCode() => HashCode.Combine(id, name);
        public int CompareTo(TweenId t) => t.id == id ? id.CompareTo(t.id) : name.CompareTo(t.name);
        public static bool operator<(TweenId a, TweenId b) => a.id == b.id ? a.name.CompareTo(b.name) < 0 : a.id < b.id;
        public static bool operator>(TweenId a, TweenId b) => a.id == b.id ? a.name.CompareTo(b.name) > 0 : a.id > b.id;
        public static bool operator==(TweenId a, TweenId b) => a.id == b.id && a.name == b.name;
        public static bool operator!=(TweenId a, TweenId b) => a.id != b.id || a.name != b.name;
        public static implicit operator TweenId(int a) => new TweenId(a);
        public static implicit operator TweenId(string s) => new TweenId(s);

        public override string ToString() => $"TweenId:[{id}|{name}]";
    }
    
    
}
