using System;
using System.Collections.Generic;

using Prota;

namespace Prota
{
    public partial class EventQueue
    {
        struct EventA : IEvent { };
        struct EventB : IEvent { };
        
        public static void UnitTest()
        {
            EventQueue.instance.log = true;
            
            EventQueue.instance.AddCallback<EventA>(a => {
                Console.WriteLine("123456");
            });
            
            Console.WriteLine("↓↓↓↓↓↓↓↓");
            EventQueue.instance.Notify<EventA>();       // 123456
            Console.WriteLine("↑↑↑↑↑↑↑↑");
            
            int cnt = 0;
            EventQueue.instance.AddCallback<EventB>(b => {
                cnt += 1;
                Console.WriteLine($"Callback b on cnt={cnt}");
                if(cnt < 5) EventQueue.instance.Notify<EventB>();
            });
            Console.WriteLine("↓↓↓↓↓↓↓↓");
            EventQueue.instance.Notify<EventB>();   // Callback b on cnt=1, 2, 3, 4, 5
            Console.WriteLine("↑↑↑↑↑↑↑↑");
            
            EventQueue.instance.ClearCallback<EventA>();
            EventQueue.instance.ClearCallback<EventB>();
            
            
            Action<EventA> f1 = (a) => { Console.WriteLine("cc a"); };
            Action<EventA> f2 = (a) => { Console.WriteLine("bb a"); };
            Action<EventA> f3 = (a) => { Console.WriteLine("aa a"); };
            EventQueue.instance.AddCallback(f1);
            EventQueue.instance.AddCallback(f2);
            EventQueue.instance.AddCallback(f3);
            EventQueue.instance.AddCallback<EventA>((a) => {
                EventQueue.instance.RemoveCallback(f1);
                EventQueue.instance.RemoveCallback(f2);
                EventQueue.instance.RemoveCallback(f3);
            });
            EventQueue.instance.Notify(new EventA());       // cc a, bb a, aa a must shown.
            EventQueue.instance.Notify(new EventA());       // cc a, bb a, aa a must not shown.
            
            
            EventQueue.instance.ClearAll();
            EventQueue.instance.Notify(new EventA());       // nothing happened.
            EventQueue.instance.Notify(new EventB());       // nothing happened.
            
            
            cnt = 0;
            f1 = (a) => {
                cnt += 1;
                Console.WriteLine($"core cnt={cnt}");
                if(cnt < 5) EventQueue.instance.Notify<EventA>();
            };
            f2 = (a) => {
                Console.WriteLine("side 1");
            };
            f3 = (a) => {
                Console.WriteLine("side 2");
            };
            EventQueue.instance.AddCallback(f1);
            EventQueue.instance.AddCallback(f2);
            EventQueue.instance.AddCallback(f3);
            EventQueue.instance.Notify<EventA>();       // core side1 side2 repeat 5 times.
            
            cnt = 0;
            Action<EventA> f4 = (a) => {
                EventQueue.instance.RemoveCallback(f3);
            };
            EventQueue.instance.AddCallback(f4);
            EventQueue.instance.Notify<EventA>();       // core side1 side2 repeat 5 times. and then remove.
            
            foreach(var s in EventQueue.instance.logString.Split("##"))
                Console.WriteLine(s);
        }
    }
    
}