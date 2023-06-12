using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Prota;
using Prota.Unity;
using UnityEngine;

// 用来存储游戏中单位的"属性".
// 属性的值会根据往属性中填入的修改器(modifier)更改.
// 例如一个攻击力+5的buff会往攻击力属性中填入一个base+5的修改器.

namespace Prota.Unity
{
    public struct ModifierHandle
    {
        public readonly ArrayLinkedListKey key;

        public ModifierHandle(ArrayLinkedListKey key) => this.key = key;
    }

    
    public enum PropertyBehaviour
    {
        Float = 0,     // 标准浮点数.
        Int,        // 限制在整数范围. 如果某些 modifier 让它变成小数, 那么下取整.
        Bool,    // 只能填 0 或 1.
    }
    
    
    public enum ProeprtyDisplay
    {
        Float = 0,          // 保留两位小数的浮点数,
        Int = 1,            // 下取整.
        OneZero = 2,        // 0 或 1.
        Percent = 3,        // 百分比, 保留两位小数.
        TrueOrFalse = 4,    // true 或 false.
    }
    
    // GameProperty 用来存储任意单位的"属性".
    [Serializable]
    public class GameProperty
    {
        // 公式:
        // result = ((base + sum(addBase)) * (1 + sum(mulBase)) + sum(addFinal)) * mul(mulFinal)
        // sum 表示所有 modifier 相加.
        // mul 表示所有 modifier 相乘.
        public struct Modifier
        {
            public readonly float addBase;
            public readonly float mulBase;
            public readonly float addFinal;
            public readonly float mulFinal;
            
            public Modifier(float addBase, float mulBase, float addFinal, float mulFinal)
            {
                this.addBase = addBase;
                this.mulBase = mulBase;
                this.addFinal = addFinal;
                this.mulFinal = mulFinal;
            }
        }

        // ====================================================================================================
        // ====================================================================================================
        
        [field: SerializeField] public string name { get; private set; }
        
        [SerializeField] float _baseValue;
        public float baseValue
        {
            get => _baseValue;
            set
            {
                _baseValue = value;
                UpdateValue();
            }
        }
        
        
        [field: SerializeField] public PropertyBehaviour behaviour { get; private set; }
        
        [field: SerializeField, Readonly] public float value { get; private set; }
        
        // ====================================================================================================
        // ====================================================================================================
        
        public bool boolValue => value != 0;
        
        public int intValue => Mathf.FloorToInt(value);
        
        readonly ArrayLinkedList<Modifier> modifiers = new ArrayLinkedList<Modifier>();
        
        readonly List<Action<float, float>> onPropertyChange = new List<Action<float, float>>();
        
        public Modifier this[ModifierHandle handle] => modifiers[handle.key];
        
        public GameProperty()
        {
            
        }
        
        public GameProperty(string name, float baseValue)
        {
            this.name = name;
            this.baseValue = baseValue;
            UpdateValue();
        }
        
        public ModifierHandle AddModifier(float addBase = 0, float mulBase = 0, float addFinal = 0, float mulFinal = 1)
        {
            var modifier = new Modifier(addBase, mulBase, addFinal, mulFinal);
            var res = modifiers.Take();
            modifiers[res] = modifier;
            UpdateValue();
            return new ModifierHandle(res);
        }
        
        public ModifierHandle AddModifierTrue() => AddModifier(1, 0, 0, 1);
        
        public GameProperty RemoveModifier(ModifierHandle handle)
        {
            modifiers.Release(handle.key);
            UpdateValue();
            return this;
        }
        
        public void UpdateValue()
        {
            var ori = value;
            
            float addBase = 0;
            float mulBase = 0;
            float addFinal = 0;
            float mulFinal = 1;
            
            foreach (var modifier in modifiers)
            {
                addBase += modifier.addBase;
                mulBase += modifier.mulBase;
                addFinal += modifier.addFinal;
                mulFinal *= modifier.mulFinal;
            }
            
            var res = ((baseValue + addBase) * (1 + mulBase) + addFinal) * mulFinal;
            
            this.value = ValueConvert(res);
            
            onPropertyChange.InvokeAll(ori, value);
        }

        public void OnPropertyChange(Action<float, float> f) => onPropertyChange.Add(f);
        public void OnPropertyChangeInt(Action<int, int> f) => onPropertyChange.Add((from, to) => {
            f((int)ValueConvert(from), (int)ValueConvert(to));
        });
        public void OnPropertyChangeBool(Action<bool, bool> f) => onPropertyChange.Add((from, to) => {
            f(ValueConvert(from) != 0, ValueConvert(to) != 0);
        });
        
        float ValueConvert(float x)
        {
            switch(behaviour)
            {
                case PropertyBehaviour.Float: return x;
                case PropertyBehaviour.Int: return x.Floor();
                case PropertyBehaviour.Bool: return x != 0 ? 1 : 0;
                default: throw new Exception($"Unknown behaviour [{behaviour}].");
            }
        }
        
        public override string ToString() => $"GameProperty[{ name }]:[{ value }]";
        
        public string ToString(ProeprtyDisplay display) => ToString(display, value);
        
        public static string ToString(ProeprtyDisplay display, float value)
        {
            switch(display)
            {
                case ProeprtyDisplay.Float:
                    return value.ToString("F2");
                case ProeprtyDisplay.Int:
                    return value.ToString("F0");
                case ProeprtyDisplay.OneZero:
                    return value != 0 ? "1" : "0";
                case ProeprtyDisplay.Percent:
                    return (value * 100).ToString("F2") + "%";
                case ProeprtyDisplay.TrueOrFalse:
                    return value != 0 ? "true" : "false";
                default: throw new Exception($"Unknown display mode [{display}].");
            }
        }
    }
}
