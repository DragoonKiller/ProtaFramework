using System.Collections.Generic;
using System;
using System.Text;
using System.Buffers.Binary;
using System.Collections;

namespace Prota
{
    // 环形双指针.
    // 左闭右开区间, head 开, front 闭.
    // 使用计数来确定是否满了.
    public class CircleDualPointer
    {
        public readonly int max;
        public int head { get; private set; }
        public int tail { get; private set; }
        public int count { get; private set; }
        public bool isEmpty => count == 0;
        public bool isFull => count == max;
        
        public int this[int offset] => Position(head + offset);

        public CircleDualPointer(int max)
        {
            this.max = max;
        }
        
        public CircleDualPointer(int max, int currentCount)
        {
            this.head = 0;
            this.tail = currentCount;
            this.count = currentCount;
            this.max = max;
        }
        
        
        public void Reset()
        {
            head = tail = count = 0;
        }
        
        public int HeadMoveBack()
        {
            if(isEmpty) throw new Exception("list is empty!");
            head = (head - 1).Repeat(max);
            count++;
            return head;
        }
        
        public int TailMoveBack()
        {
            if(isFull) throw new Exception("list is full!");
            tail = (tail - 1).Repeat(max);
            count--;
            return tail;
        }
        
        public int HeadMoveAhead()
        {
            if(isFull) throw new Exception("list is full!");
            head = (head + 1).Repeat(max);
            count--;
            return head;
        }
        
        public int TailMoveAhead()
        {
            if(isEmpty) throw new Exception("list is empty!");
            tail = (tail + 1).Repeat(max);
            count++;
            return tail;
        }
        
        public int Position(int i) => i.Repeat(max);
        
        
        public static void UnitTest()
        {
            CircleDualPointer g = new CircleDualPointer(6);
            try { g.HeadMoveAhead(); } catch { Console.WriteLine("Success!"); }
            try { g.TailMoveBack(); } catch { Console.WriteLine("Success!"); }
            
            g.isEmpty.Assert();
            (!g.isFull).Assert();
            
            g.HeadMoveBack();
            g.TailMoveAhead();
            (g.head == 5).Assert();
            (g.tail == 1).Assert();
            (g.count == 2).Assert();
            (g[0] == 5).Assert();
            (g[1] == 0).Assert();
            
            g.TailMoveAhead();       // 2
            g.TailMoveAhead();
            g.TailMoveAhead();
            g.TailMoveAhead();       // 5, full.
            g.isFull.Assert();
            (!g.isEmpty).Assert();
            
            (g.count == g.max).Assert();
            g.HeadMoveAhead();
            
            (g.count == g.max - 1).Assert();
            (g.count == 5).Assert();
            
            g.Reset();
            (g.count == 0).Assert();
            (g.head == 0).Assert();
            (g.tail == 0).Assert();
            
            Console.WriteLine("All Done!");
        }
    }




    // 环形数组.
    public class CircleList<T>
    {
        CircleDualPointer pointers;
        
        T[] data;
        
        public CircleList() { }
        
        void Resize()
        {
            if(data == null)
            {
                data = new T[4];
                pointers = new CircleDualPointer(4);
            }
            else
            {
                var oriData = data;
                var oriPointers = pointers;
                data = new T[oriData.Length * 2];
                pointers = new CircleDualPointer(oriPointers.max * 2, oriPointers.count);
                for(int i = 0; i < oriPointers.count; i++)
                {
                    data[i] = oriData[oriPointers[i]];
                }
            }
        }
        
        public T this[int index]
        { 
            get => data[pointers[index]];
            set => data[pointers[index]] = value;
        }

        public int count => pointers.count;

        public bool IsReadOnly => false;


        public void Clear()
        {
            
            pointers = null;
            data = null;
        }
        
        public void Reset() => pointers.Reset();
        
        public void PushFront(T v)
        {
            if(pointers.count == data.Length) Resize();
            pointers.HeadMoveBack();
            data[pointers.head] = v;
        }
        
        public T PopFront()
        {
            var res = data[pointers.head];
            pointers.HeadMoveAhead();
            return res;
        }
        
        public void PushBack(T v)
        {
            if(pointers.count == data.Length) Resize();
            data[pointers.head] = v;
            pointers.TailMoveAhead();
        }
        
        public T PopBack()
        {
            pointers.TailMoveBack();
            var res = data[pointers.head];
            return res;
        }
    }
}
